using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectMapViewModel
{
    private StageProgressModel _progressModel;
    private ScreenStateModel _screenStateModel;
    private CharacterListModel _characterListModel;

    private StageInfoPopupViewModel _stageInfoViewModel;

    public event Action<StageInfoPopupViewModel> OnStageInfoPopupOpenRequested;
    public event Action OnStageInfoPopupCloseRequested;

    public StageSelectMapViewModel()
    {
        StageSession session = StageSession.Instance;

        if (null == session)
        {
            Debug.LogError("[StageSelectMapViewModel] StageSession.Instance 가 null 입니다.");
        }
        else
        {
            _progressModel = session.Progress;
            _screenStateModel = session.ScreenState;
        }

        if (null == NetworkManagerTemp.Instance)
        {
            Debug.LogError("[StageSelectMapViewModel] NetworkManagerTemp.Instance 가 null 입니다.");
        }
        else
        {
            _characterListModel = NetworkManagerTemp.Instance.GetcharacterListModel();
        }
    }

    public void Dispose()
    {
        CloseAllPopups();

        _progressModel = null;
        _screenStateModel = null;
        _characterListModel = null;
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

    // ===== 플레이어 이동 제어 (세션 경유) =====

    private void StopPlayer()
    {
        StageSession session = StageSession.Instance;

        if (null == session || null == session.Player)
        {
            return;
        }

        session.Player.StopMove();
    }

    private void ResumePlayer()
    {
        StageSession session = StageSession.Instance;

        if (null == session || null == session.Player)
        {
            return;
        }

        session.Player.ResumeMove();
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

        ResumePlayer();
    }
}
