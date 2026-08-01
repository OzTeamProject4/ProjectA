using System;

public class DialogueLogButton : BaseButton
{
    public event Action ButtonClicked;

    protected override void OnButtonClick()
    {
        if (ButtonClicked == null)
        {
            return;
        }

        ButtonClicked.Invoke();
    }
}