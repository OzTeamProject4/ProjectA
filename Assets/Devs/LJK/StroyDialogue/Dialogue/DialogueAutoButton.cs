using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueAutoButton : BaseButton
{
    [SerializeField] private Image _buttonImage;
    [SerializeField] private List<Sprite> _autoButtonSprite;

    public event Action ButtonClicked;

    public void UpdateButtonSprite(bool isAuto)
    {
        Sprite sprite = isAuto ? _autoButtonSprite[0] : _autoButtonSprite[1];
        _buttonImage.sprite = sprite;
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