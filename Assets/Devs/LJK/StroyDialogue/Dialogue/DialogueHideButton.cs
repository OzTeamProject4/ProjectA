using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueHideButton : BaseButton
{
    [SerializeField] private Image _buttonImage;
    [SerializeField] private List<Sprite> _hideButtonSprite;

    public event Action ButtonClicked;

    public void UpdateButtonSprite(bool _isVisible)
    {
        Sprite sprite = _isVisible ? _hideButtonSprite[0] : _hideButtonSprite[1];
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