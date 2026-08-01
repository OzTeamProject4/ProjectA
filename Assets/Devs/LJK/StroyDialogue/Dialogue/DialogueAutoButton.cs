using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueAutoButton : BaseButton
{
    [SerializeField] private Image _buttonImage;

    public event Action ButtonClicked;

    public void UpdateButtonSprite(bool isAuto)
    {
        Color color = isAuto ? Color.softBlue :  Color.white;
        _buttonImage.color = color;
    }

    protected override void OnButtonClick()
    {
        if (ButtonClicked == null)
        {
            return;
        }

        ButtonClicked.Invoke();
    }
}