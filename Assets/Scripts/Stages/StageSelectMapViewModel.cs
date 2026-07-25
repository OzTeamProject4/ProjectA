using System;
using UnityEngine;

public class StageSelectMapViewModel
{
    private StageProgressModel _progressModel;
    private ScreenStateModel _screenStateModel;
    private CharacterListModel _characterListModel;
    private StageSelectPlayer _player;
    private StageDataModel _stageDataModel;

    private StageInfoPopupViewModel _stageInfoViewModel;

    public event Action<StageInfoPopupViewModel> OnStageInfoPopupOpenRequested;
    public event Action OnStageInfoPopupCloseRequested;

    public StageSelectMapViewModel(StageProgressModel progressModel, ScreenStateModel screenStateModel, CharacterListModel characterListModel, StageSelectPlayer player, StageDataModel stageDataModel)
    {
        if (null == progressModel)
        {
            Debug.LogError("[StageSelectMapViewModel] progressModel 이 null 입니다.");
        }

        if (null == screenStateModel)
        {
            Debug.LogError("[StageSelectMapViewModel] screenStateModel 이 null 입니다.");
        }

        if (null == characterListModel)
        {
            Debug.LogError("[StageSelectMapViewModel] characterListModel 이 null 입니다.");
        }

        if (null == player)
        {
            Debug.LogError("[StageSelectMapViewModel] player 가 null 입니다.");
        }

        if (null == stageDataModel)
        {
            Debug.LogError("[StageSelectMapViewModel] stageDataModel 이 null 입니다.");
        }

        _progressModel = progressModel;
        _screenStateModel = screenStateModel;
        _characterListModel = characterListModel;
        _player = player;
        _stageDataModel = stageDataModel;
    }

    public void Dispose()
    {
        CloseAllPopups();

        _progressModel = null;
        _screenStateModel = null;
        _characterListModel = null;
        _player = null;
        _stageDataModel = null;
    }

    public void CloseAllPopups()
    {
        RequestCloseStageInfoPopup();
    }

    // ===== 몬스터 파티 도달/이탈 =====

    public void HandlePartyReached(string stageId)
    {
        if (null == _progressModel)
        {
            return;
        }

        _progressModel.SelectStage(stageId);

        StopPlayer();

        RequestOpenStageInfoPopup(stageId);
    }

    public void HandlePartyLeft(string stageId)
    {
        if (null == _progressModel || _progressModel.SelectedStageId != stageId)
        {
            return;
        }

        RequestCloseStageInfoPopup();

        ResumePlayer();
    }

    // ===== 플레이어 이동 제어 =====

    private void StopPlayer()
    {
        if (null == _player)
        {
            return;
        }

        _player.StopMove();
    }

    private void ResumePlayer()
    {
        if (null == _player)
        {
            return;
        }

        _player.ResumeMove();
    }

    // ===== 스테이지 정보 팝업 =====

    private void RequestOpenStageInfoPopup(string stageId)
    {
        if (null == _stageDataModel)
        {
            return;
        }

        StageData stageData = _stageDataModel.GetStage(stageId);

        if (null == stageData)
        {
            Debug.LogWarning($"[StageSelectMapViewModel] StageData 를 찾을 수 없습니다. stageId={stageId}");
            return;
        }

        _stageInfoViewModel = new StageInfoPopupViewModel(stageData, _stageDataModel.GetStageWaves(stageId), _screenStateModel, _progressModel, _characterListModel);
        _stageInfoViewModel.OnCloseRequested += HandleStageInfoCloseRequested;

        OnStageInfoPopupOpenRequested?.Invoke(_stageInfoViewModel);
    }

    private void RequestCloseStageInfoPopup()
    {
        if (null == _stageInfoViewModel)
        {
            return;
        }

        UnsubscribeStageInfoViewModel();

        OnStageInfoPopupCloseRequested?.Invoke();
    }

    private void UnsubscribeStageInfoViewModel()
    {
        if (null == _stageInfoViewModel)
        {
            return;
        }

        _stageInfoViewModel.OnCloseRequested -= HandleStageInfoCloseRequested;

        _stageInfoViewModel.Dispose();
        _stageInfoViewModel = null;
    }

    private void HandleStageInfoCloseRequested()
    {
        RequestCloseStageInfoPopup();

        ResumePlayer();
    }
}
