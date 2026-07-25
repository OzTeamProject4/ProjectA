using System;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoPopupViewModel
{
    private const int PartySlotCount = 3;

    private readonly StageData _stageData;
    private readonly ScreenStateModel _screenStateModel;
    private readonly StageProgressModel _progressModel;
    private readonly StudentListModel _characterListModel;
    private readonly IReadOnlyList<StageWaveData> _waves;

    private readonly StudentModel[] _partySlots = new StudentModel[PartySlotCount];

    private PartySelectPopupViewModel _partySelectViewModel;
    private int _selectingSlotIndex = -1;

    public int SlotCount
    {
        get { return PartySlotCount; }
    }

    public string StageName
    {
        get
        {
            if (null == _stageData)
            {
                return string.Empty;
            }

            return _stageData.StageName;
        }
    }

    public event Action OnCloseRequested;
    public event Action<PartySelectPopupViewModel> OnPartySelectOpenRequested;
    public event Action OnPartySelectCloseRequested;
    public event Action<int> OnPartySlotChanged;

    public StageInfoPopupViewModel(StageData stageData, IReadOnlyList<StageWaveData> waves, ScreenStateModel screenStateModel, StageProgressModel progressModel, StudentListModel characterListModel)
    {
        if (null == stageData)
        {
            Debug.LogError("[StageInfoPopupViewModel] stageData 가 null 입니다.");
        }

        if (null == screenStateModel)
        {
            Debug.LogError("[StageInfoPopupViewModel] screenStateModel 이 null 입니다.");
        }

        if (null == progressModel)
        {
            Debug.LogError("[StageInfoPopupViewModel] progressModel 이 null 입니다.");
        }

        if (null == characterListModel)
        {
            Debug.LogError("[StageInfoPopupViewModel] characterListModel 이 null 입니다.");
        }

        _stageData = stageData;
        _waves = waves;
        _screenStateModel = screenStateModel;
        _progressModel = progressModel;
        _characterListModel = characterListModel;
    }

    public string GetSlotIconPath(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _partySlots.Length)
        {
            return null;
        }

        StudentModel student = _partySlots[slotIndex];

        if (null == student)
        {
            return null;
        }

        return student.PortraitKey;
    }

    public void StartBattleCommand()
    {
        if (null == _screenStateModel || null == _progressModel)
        {
            return;
        }

        List<string> selectedPartyIds = CollectSelectedPartyIds();

        if (selectedPartyIds.Count == 0)
        {
            Debug.LogWarning("[StageInfoPopupViewModel] 편성된 캐릭터가 없어 전투를 시작할 수 없습니다.");
            return;
        }

        _progressModel.SetSelectedPartyIds(selectedPartyIds);

        _screenStateModel.ChangeScreen(ScreenType.Battle);
    }

    private List<string> CollectSelectedPartyIds()
    {
        List<string> partyIds = new List<string>();

        foreach (StudentModel student in _partySlots)
        {
            if (null == student)
            {
                continue;
            }

            if (string.IsNullOrEmpty(student.DataId))
            {
                continue;
            }

            partyIds.Add(student.DataId);
        }

        return partyIds;
    }

    public void CloseCommand()
    {
        OnCloseRequested?.Invoke();
    }

    public void Dispose()
    {
        ClosePartySelect();
    }

    public IReadOnlyList<string> GetMonsterIds()
    {
        List<string> result = new List<string>();

        if (null == _waves)
        {
            return result;
        }

        foreach (StageWaveData wave in _waves)
        {
            if (null == wave)
            {
                continue;
            }

            foreach (string monsterId in wave.MonsterIds)
            {
                if (string.IsNullOrEmpty(monsterId))
                {
                    continue;
                }

                if (result.Contains(monsterId))
                {
                    continue;
                }

                result.Add(monsterId);
            }
        }

        return result;
    }

    // ===== 캐릭터 슬롯 =====

    public void SelectSlotCommand(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= _partySlots.Length)
        {
            return;
        }

        UnsubscribePartySelectViewModel();

        _selectingSlotIndex = slotIndex;

        IReadOnlyList<StudentModel> candidates = GetCandidateCharacters();

        _partySelectViewModel = new PartySelectPopupViewModel(candidates);
        _partySelectViewModel.OnCharacterSelected += HandleCharacterSelected;
        _partySelectViewModel.OnCloseRequested += HandlePartySelectCloseRequested;

        OnPartySelectOpenRequested?.Invoke(_partySelectViewModel);
    }

    private IReadOnlyList<StudentModel> GetCandidateCharacters()
    {
        if (null == _characterListModel)
        {
            return Array.Empty<StudentModel>();
        }

        return _characterListModel.StudentList;
    }

    private void HandleCharacterSelected(StudentModel character)
    {
        if (_selectingSlotIndex >= 0 && _selectingSlotIndex < _partySlots.Length)
        {
            _partySlots[_selectingSlotIndex] = character;

            OnPartySlotChanged?.Invoke(_selectingSlotIndex);
        }

        ClosePartySelect();
    }

    private void HandlePartySelectCloseRequested()
    {
        ClosePartySelect();
    }

    private void ClosePartySelect()
    {
        if (null == _partySelectViewModel)
        {
            return;
        }

        UnsubscribePartySelectViewModel();

        OnPartySelectCloseRequested?.Invoke();
    }

    private void UnsubscribePartySelectViewModel()
    {
        if (null == _partySelectViewModel)
        {
            return;
        }

        _partySelectViewModel.OnCharacterSelected -= HandleCharacterSelected;
        _partySelectViewModel.OnCloseRequested -= HandlePartySelectCloseRequested;
        _partySelectViewModel = null;
        _selectingSlotIndex = -1;
    }
}
