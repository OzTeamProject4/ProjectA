using System;

public class MultiGachaDrawButton : BaseButton
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