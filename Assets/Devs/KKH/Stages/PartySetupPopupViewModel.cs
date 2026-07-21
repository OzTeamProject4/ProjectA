using System;
using UnityEngine;

public class PartySetupPopupViewModel
{
    private readonly ScreenStateModel _screenStateModel;

    public event Action OnCloseRequested;

    public PartySetupPopupViewModel(ScreenStateModel screenStateModel)
    {
        if (null == screenStateModel)
        {
            Debug.LogError("[PartySetupPopupViewModel] screenStateModel 이 null 입니다.");
        }

        _screenStateModel = screenStateModel;
    }

    public void StartBattleCommand()
    {
        if (null == _screenStateModel)
        {
            return;
        }

        _screenStateModel.ChangeScreen(ScreenType.Battle);
    }

    public void CloseCommand()
    {
        OnCloseRequested?.Invoke();
    }

    // TODO: 파티 편성 로직 구현 시 슬롯별 캐릭터 선택/해제 커맨드 추가
}