using System;
using UnityEngine;

public class StageSelectMapViewModel
{
    private StageProgressModel _progressModel;
    private ScreenStateModel _screenStateModel;
    private StudentListModel _characterListModel;
    private PlayerMoveLockModel _moveLockModel;
    private StageDataModel _stageDataModel;

    private StageInfoPopupViewModel _stageInfoViewModel;

    private StageSelectHudViewModel _hudViewModel;

    public event Action<StageInfoPopupViewModel> OnStageInfoPopupOpenRequested;
    public event Action OnStageInfoPopupCloseRequested;

    public bool IsVisible
    {
        get
        {
            return null != _screenStateModel && _screenStateModel.CurrentScreen == ScreenType.StageSelect;
        }
    }

    public event Action<bool> OnVisibleChanged;
    public event Action<StageSelectHudViewModel> OnHudOpenRequested;
    public event Action OnHudCloseRequested;
    public event Action<string> OnStageCleared;

    public bool IsStageCleared(string stageId)
    {
        if (null == _progressModel)
        {
            return false;
        }

        return _progressModel.IsCleared(stageId);
    }

    public StageSelectMapViewModel(StageProgressModel progressModel, ScreenStateModel screenStateModel, StudentListModel characterListModel, PlayerMoveLockModel moveLockModel, StageDataModel stageDataModel)
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

        if (null == moveLockModel)
        {
            Debug.LogError("[StageSelectMapViewModel] moveLockModel 이 null 입니다.");
        }

        if (null == stageDataModel)
        {
            Debug.LogError("[StageSelectMapViewModel] stageDataModel 이 null 입니다.");
        }

        _progressModel = progressModel;
        _screenStateModel = screenStateModel;
        _characterListModel = characterListModel;
        _moveLockModel = moveLockModel;
        _stageDataModel = stageDataModel;

        _hudViewModel = new StageSelectHudViewModel(screenStateModel, moveLockModel);

        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged += HandleScreenChanged;
        }

        if (null != _progressModel)
        {
            _progressModel.OnStageCleared += HandleStageCleared;
        }
    }

    public void Refresh()
    {
        ApplyScreenState();
    }

    public void Dispose()
    {
        if (null != _screenStateModel)
        {
            _screenStateModel.OnScreenChanged -= HandleScreenChanged;
        }

        if (null != _progressModel)
        {
            _progressModel.OnStageCleared -= HandleStageCleared;
        }

        CloseAllPopups();

        ResumePlayer();

        if (null != _hudViewModel)
        {
            _hudViewModel.Dispose();
            _hudViewModel = null;
        }

        _progressModel = null;
        _screenStateModel = null;
        _characterListModel = null;
        _moveLockModel = null;
        _stageDataModel = null;
    }

    public void CloseAllPopups()
    {
        RequestCloseStageInfoPopup();
    }

    // ===== 화면 상태 =====

    private void HandleScreenChanged(ScreenType screen)
    {
        ApplyScreenState();
    }

    private void HandleStageCleared(string stageId)
    {
        if (null != _progressModel && _progressModel.SelectedStageId == stageId)
        {
            RequestCloseStageInfoPopup();
        }

        OnStageCleared?.Invoke(stageId);
    }

    private void ApplyScreenState()
    {
        bool isVisible = IsVisible;

        OnVisibleChanged?.Invoke(isVisible);

        if (isVisible)
        {
            OnHudOpenRequested?.Invoke(_hudViewModel);
            return;
        }

        CloseAllPopups();
        ResumePlayer();


        OnHudCloseRequested?.Invoke();
    }

    // ===== 몬스터 파티 도달/이탈 =====

    public void HandlePartyReached(string stageId)
    {
        if (null == _progressModel)
        {
            return;
        }

        if (_progressModel.IsCleared(stageId))
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
        if (null == _moveLockModel)
        {
            return;
        }

        _moveLockModel.Lock(MoveLockReason.StageInfoPopup);
    }

    private void ResumePlayer()
    {
        if (null == _moveLockModel)
        {
            return;
        }

        _moveLockModel.Unlock(MoveLockReason.StageInfoPopup);
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
