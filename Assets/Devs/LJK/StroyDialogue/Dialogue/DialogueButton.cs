using System;

public class DialogueButton : BaseButton
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