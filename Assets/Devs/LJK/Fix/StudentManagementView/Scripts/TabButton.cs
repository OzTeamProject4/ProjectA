using System;
using UnityEngine;

public class TabButton : BaseButton
{
    [SerializeField] private GameObject _contentPanel;

    public event Action OnTabClicked;

    protected override void OnButtonClick()
    {
        if (OnTabClicked == null)
        {
            return;
        }

        OnTabClicked.Invoke();
    }

    public void SetPanelActive(bool isActive)
    {
        _contentPanel.SetActive(isActive);
    }
}