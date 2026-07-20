using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSlotView : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Button _selectButton;

    [SerializeField] private List<GameObject> _starObjectList;

    private CharacterSlotViewModel _characterSlotViewModel;

    public event Action<string> OnClicked;

    public void OnEnable()
    {
        _selectButton.onClick.AddListener(HandleClickSelect);
    }

    public void OnDisable()
    {
        _selectButton.onClick.RemoveAllListeners();
    }

    public void Bind(string characterId)
    {
        _characterSlotViewModel = new CharacterSlotViewModel(characterId);
        _characterSlotViewModel.ModelPropertyChanged += OnModelPropertyChanged;
        _characterSlotViewModel.Init();
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_characterSlotViewModel.IconPath):
                SetIcon(_characterSlotViewModel.IconPath);
                break;
            case nameof(_characterSlotViewModel.Name):
                SetName(_characterSlotViewModel.Name);
                break;
            case nameof(_characterSlotViewModel.Star):
                SetStar(_characterSlotViewModel.Star);
                break;
        }
    }

    private void SetIcon(string name)
    {
        Sprite image = Resources.Load<Sprite>(name);
        _portraitImage.sprite = image;
    }

    private void SetName(string name)
    {
        _nameText.text = name;
    }

    private void SetStar(int count)
    {
        for (int i = 0; i < _starObjectList.Count; i++)
        {
            _starObjectList[i].SetActive(i < count);
        }
    }

    private void HandleClickSelect()
    {
        OnClicked?.Invoke(_characterSlotViewModel.Id);
    }
}