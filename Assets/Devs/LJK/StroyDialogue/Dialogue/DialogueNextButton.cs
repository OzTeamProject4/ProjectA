using System;

public class DialogueNextButton : BaseButton
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