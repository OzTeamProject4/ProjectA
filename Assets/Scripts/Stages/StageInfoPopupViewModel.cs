using System;
using System.Collections.Generic;
using UnityEngine;

public class StageInfoPopupViewModel
{
    private const int PartySlotCount = 3;

    private readonly StageData _stageData;
    private readonly ScreenStateModel _screenStateModel;
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

    public StageInfoPopupViewModel(StageData stageData, IReadOnlyList<StageWaveData> waves, ScreenStateModel screenStateModel)
    {
        if (null == stageData)
        {
            Debug.LogError("[StageInfoPopupViewModel] stageData 가 null 입니다.");
        }

        if (null == screenStateModel)
        {
            Debug.LogError("[StageInfoPopupViewModel] screenStateModel 이 null 입니다.");
        }

        _stageData = stageData;
        _waves = waves;
        _screenStateModel = screenStateModel;
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
        if (null == _screenStateModel)
        {
            return;
        }

        // TODO: 선택된 파티(_partySlots)를 BattleManager 로 전달하는 배선 필요
        _screenStateModel.ChangeScreen(ScreenType.Battle);
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
        if (null == NetworkManagerTemp.Instance)
        {
            Debug.LogError("[StageInfoPopupViewModel] NetworkManagerTemp.Instance 가 null 입니다.");
            return Array.Empty<CharacterModel>();
        }

        CharacterListModel listModel = NetworkManagerTemp.Instance.GetcharacterListModel();

        if (null == listModel)
        {
            return Array.Empty<CharacterModel>();
        }

        return listModel.CharacterIdList;
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
