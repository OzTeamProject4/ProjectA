using System;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoPopupViewModel
{
    private const int PartySlotCount = 3;

    private readonly StageData _stageData;
    private readonly ScreenStateModel _screenStateModel;
    private readonly StageProgressModel _progressModel;
    private readonly CharacterListModel _characterListModel;
    private readonly IReadOnlyList<StageWaveData> _waves;

    private readonly CharacterModel[] _partySlots = new CharacterModel[PartySlotCount];

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

    public StageInfoPopupViewModel(StageData stageData, IReadOnlyList<StageWaveData> waves, ScreenStateModel screenStateModel, StageProgressModel progressModel, CharacterListModel characterListModel)
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

        CharacterModel character = _partySlots[slotIndex];

        if (null == character)
        {
            return null;
        }

        return character.IconPath;
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

        foreach (CharacterModel character in _partySlots)
        {
            if (null == character)
            {
                continue;
            }

            if (string.IsNullOrEmpty(character.Id))
            {
                continue;
            }

            partyIds.Add(character.Id);
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

        IReadOnlyList<CharacterModel> candidates = GetCandidateCharacters();

        _partySelectViewModel = new PartySelectPopupViewModel(candidates);
        _partySelectViewModel.OnCharacterSelected += HandleCharacterSelected;
        _partySelectViewModel.OnCloseRequested += HandlePartySelectCloseRequested;

        OnPartySelectOpenRequested?.Invoke(_partySelectViewModel);
    }

    private IReadOnlyList<CharacterModel> GetCandidateCharacters()
    {
        if (null == _characterListModel)
        {
            return Array.Empty<CharacterModel>();
        }

        return _characterListModel.CharacterIdList;
    }

    private void HandleCharacterSelected(CharacterModel character)
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
