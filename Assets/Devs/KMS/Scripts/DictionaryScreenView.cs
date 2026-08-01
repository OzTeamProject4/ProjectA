using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DictionaryScreenView : BaseUI
{
    [SerializeField] private Button _backToLobbyButton;
    [SerializeField] private Transform _transform_SlotRoot;
    [SerializeField] private string _dictionarySlotId;
    [SerializeField] private GameObject _aboutScreen;
    [SerializeField] private Image _aboutImage;
    [SerializeField] private TMP_Text _aboutName;
    [SerializeField] private TMP_Text _aboutPersonality;
    [SerializeField] private TMP_Text _aboutBackground;
    [SerializeField] private TMP_Text _aboutLikes;
    [SerializeField] private TMP_Text _aboutDislikes;

    private CancellationTokenSource _disableCts;


    private List<GameObject> _dictionarySlotList = new List<GameObject>();

    private void OnEnable()
    {
        ResetRectTransform();
        _disableCts = new CancellationTokenSource();

        if (_backToLobbyButton == null)
        {
            Debug.LogError("BackToLobbyButton is not assigned.");
            return;
        }

        _backToLobbyButton.onClick.AddListener(OnBackToLobbyButtonClicked);
        ReadEnemyListAndCreateSlot().Forget();
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        if (_backToLobbyButton == null)
        {
            return;
        }

        _backToLobbyButton.onClick.RemoveListener(OnBackToLobbyButtonClicked);
        ClearSlots();
    }


    private void OnBackToLobbyButtonClicked()
    {

        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null.");
            return;
        }

        if (GameManager.Instance.UIManager == null)
        {
            Debug.LogError("UIManager is null.");
            return;
        }

        GameManager.Instance.UIManager.CloseDictionaryScreen();
    }

    private void ResetRectTransform()
    {
        RectTransform rectTransform = transform as RectTransform;

        if (rectTransform == null)
        {
            Debug.LogError("DictionaryScreen RectTransform was not found.");
            return;
        }

        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.localScale = Vector3.one;
        rectTransform.localRotation = Quaternion.identity;
    }

    private async UniTaskVoid ReadEnemyListAndCreateSlot()
    {
        ClearSlots();

        if (GameManager.Instance.DataManager.TryGetDataTable(out Dictionary<string, StudentData> table))
        {
            foreach (StudentData studentData in table.Values) {
                if (null == studentData)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(studentData.DataId))
                {
                    continue;
                }
                await AddEnemyBookSlot(studentData);
            }
            return;
        }
    }

    private async UniTask AddEnemyBookSlot(StudentData studentData)
    {
        GameObject Slot = await GameManager.Instance.ObjectManager.SpawnAsync(_dictionarySlotId, _transform_SlotRoot, Vector3.zero, Quaternion.identity);
        if (Slot != null)
        {
            Slot.transform.localScale = Vector3.one;
            _dictionarySlotList.Add(Slot);

            if (Slot.TryGetComponent(out DictionarySlotView dictionarySlotView))
            {
                dictionarySlotView.BindSlotViewModel(studentData);
                dictionarySlotView.OnSlotClicked += OnDictionarySlotClicked; // ±¸µ¶

            }
        }
    }

    private void ClearSlots()
    {
        foreach (var slot in _dictionarySlotList)
        {
            if (slot != null)
            {
                GameManager.Instance.ObjectManager.Despawn(slot);
            }
        }
        _dictionarySlotList.Clear();
    }
    private void OnDictionarySlotClicked(StudentData studentData)
    {
        _aboutScreen.SetActive(true);

        if (studentData == null)
        {
            return;
        }
        //SpriteLoader.LoadIntoAsync(_aboutImage, studentData.StandImagePath, _disableCts.Token).Forget();

        _aboutName.text = studentData.Name;

        _aboutPersonality.text = studentData.Personality;
        _aboutBackground.text = studentData.Background;
        _aboutLikes.text = studentData.Likes;
        _aboutDislikes.text = studentData.Dislikes;

        
    }
}