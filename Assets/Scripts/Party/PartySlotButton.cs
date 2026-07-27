using System;
using UnityEngine;
using UnityEngine.UI;

public class PartySlotButton : MonoBehaviour
{
    [SerializeField] private int _index;
    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;

    public int Index
    {
        get { return _index; }
    }

    public event Action<int> OnClicked;

    private void OnEnable()
    {
        if (null != _button)
        {
            _button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDisable()
    {
        if (null != _button)
        {
            _button.onClick.RemoveListener(HandleClick);
        }
    }

    public void SetIcon(Sprite sprite)
    {
        if (null == _iconImage)
        {
            return;
        }

        if (null == sprite)
        {
            _iconImage.enabled = false;
            return;
        }

        _iconImage.sprite = sprite;
        _iconImage.enabled = true;
    }

    private void HandleClick()
    {
        OnClicked?.Invoke(_index);
    }
}
