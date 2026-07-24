using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StudentManagementInfoView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private Image _elementIcon;
    [SerializeField] private TMP_Text _requiredGradeUpItemCountText;
    [SerializeField] private Button _gradeUpButton;
    [SerializeField] private List<GameObject> _starIconList;

    public event Action OnGradeUpClicked;

    private void Awake()
    {
        UnityUtil.ValidateReference(_nameText, nameof(StudentManagementInfoView), nameof(_nameText));
        UnityUtil.ValidateReference(_elementIcon, nameof(StudentManagementInfoView), nameof(_elementIcon));
        UnityUtil.ValidateReference(_requiredGradeUpItemCountText, nameof(StudentManagementInfoView), nameof(_requiredGradeUpItemCountText));
        UnityUtil.ValidateReference(_gradeUpButton, nameof(StudentManagementInfoView), nameof(_gradeUpButton));
    }

    private void OnEnable()
    {
        _gradeUpButton.onClick.AddListener(HandleGradeUpButtonClicked);
    }

    private void OnDisable()
    {
        _gradeUpButton.onClick.RemoveAllListeners();
    }

    public void UpdateName(string studentName)
    {
        _nameText.text = studentName;
    }

    public void UpdateRequiredGradeUpItemText(int ownedItemCount, int requiredItemCount)
    {
        _requiredGradeUpItemCountText.text = $"{ownedItemCount} / {requiredItemCount}";
    }

    public void UpdateStars(int starCount)
    {
        for (int i = 0; i < _starIconList.Count; i++)
        {
            bool isActive = i < starCount;
            _starIconList[i].SetActive(isActive);
        }
    }

    public void InterectiveGradeUp(bool interective)
    {
        _gradeUpButton.interactable = interective;
    }

    private void HandleGradeUpButtonClicked()
    {
        if (OnGradeUpClicked == null)
        {
            return;
        }

        OnGradeUpClicked.Invoke();
    }
}