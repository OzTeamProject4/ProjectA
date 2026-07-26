using System;

public class PopupBackgroundButton : BaseButton
{
    public event Action OnBackgroundClicked;

    protected override void OnButtonClick()
    {
        if (OnBackgroundClicked == null)
        {
            return;
        }

        OnBackgroundClicked.Invoke();
    }
}
