using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectMapViewModel
{
    private readonly StageProgressModel _progressModel;
    private readonly ScreenStateModel _screenStateModel;
    private readonly CharacterListModel _characterListModel;
    private readonly StagePlayerParty _playerParty;

    private StageInfoPopupViewModel _stageInfoViewModel;

    public event Action<StageInfoPopupViewModel> OnStageInfoPopupOpenRequested;
    public event Action OnStageInfoPopupCloseRequested;

    public StageSelectMapViewModel(StageProgressModel progressModel, ScreenStateModel screenStateModel, StagePlayerParty playerParty, CharacterListModel characterListModel)
    {
        if (null == progressModel || null == screenStateModel || null == playerParty || null == characterListModel)
        {
            Debug.LogError("[StageSelectMapViewModel] 생성자 인자 중 null 이 있습니다.");
        }

        _progressModel = progressModel;
        _screenStateModel = screenStateModel;
        _playerParty = playerParty;
        _characterListModel = characterListModel;
    }

    public void Dispose()
    {
        CloseAllPopups();
    }

    public void CloseAllPopups()
    {
        RequestCloseStageInfoPopup();
    }

    // ===== 몬스터 파티 도달/이탈 =====

    public void HandlePartyReached(string stageId)
    {
        _progressModel.SelectStage(stageId);

        if (null != _playerParty)
        {
            _playerParty.StopMove();
        }

        RequestOpenStageInfoPopup(stageId);
    }

    public void HandlePartyLeft(string stageId)
    {
        if (_progressModel.SelectedStageId != stageId)
        {
            return;
        }

        RequestCloseStageInfoPopup();

        if (null != _playerParty)
        {
            _playerParty.ResumeMove();
        }
    }

    // ===== 스테이지 정보 팝업 =====

    private void RequestOpenStageInfoPopup(string stageId)
    {
        StageData stageData = GetStage(stageId);

        if (null == stageData)
        {
            Debug.LogWarning($"[StageSelectMapViewModel] StageData 를 찾을 수 없습니다. stageId={stageId}");
            return;
        }

        _stageInfoViewModel = new StageInfoPopupViewModel(stageData, GetStageWaves(stageId), _screenStateModel, _progressModel, _characterListModel);
        _stageInfoViewModel.OnCloseRequested += HandleStageInfoCloseRequested;

        OnStageInfoPopupOpenRequested?.Invoke(_stageInfoViewModel);
    }

    private StageData GetStage(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageSelectMapViewModel] GetStage: stageId 가 비어 있습니다.");
            return null;
        }

        GameManager.Instance.DataManager.TryGetData(stageId, out StageData data);
        return data;
    }

    private IReadOnlyList<StageWaveData> GetStageWaves(string stageId)
    {
        if (string.IsNullOrEmpty(stageId))
        {
            Debug.LogWarning("[StageSelectMapViewModel] GetStageWaves: stageId 가 비어 있습니다.");
            return Array.Empty<StageWaveData>();
        }

        if (!GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StageWaveData> table))
        {
            return Array.Empty<StageWaveData>();
        }

        List<StageWaveData> waves = new List<StageWaveData>();

        foreach (StageWaveData wave in table.Values)
        {
            if (wave.StageId == stageId)
            {
                waves.Add(wave);
            }
        }

        waves.Sort(CompareByWaveNumber);

        return waves;
    }

    private static int CompareByWaveNumber(StageWaveData a, StageWaveData b)
    {
        return a.WaveNumber.CompareTo(b.WaveNumber);
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

        if (null != _playerParty)
        {
            _playerParty.ResumeMove();
        }
    }
}
