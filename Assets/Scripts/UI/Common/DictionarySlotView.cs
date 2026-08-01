using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DictionarySlotView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _text_Name;
    [SerializeField] private Image _iconImage;
    [SerializeField] private GameObject _uiRoot;

    public event Action<StudentData> OnSlotClicked;

    private CancellationTokenSource _disableCts;
    private StudentData _boundData;
    private Button _button;



    public void BindSlotViewModel(StudentData studentData)
    {
        _boundData = studentData;
        _text_Name.text = studentData.Name;

        SpriteLoader.LoadIntoAsync(_iconImage,studentData.CharacterIconPath, _disableCts.Token).Forget();
    }


    private void Awake()
    {
        _button = GetComponent<Button>();
    }
  
    private void OnEnable()
    {
        _disableCts = new CancellationTokenSource();


        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnDisable()
    {
        if (_disableCts != null)
        {
            _disableCts.Cancel();
            _disableCts.Dispose();
            _disableCts = null;
        }

        _button.onClick.RemoveListener(OnButtonClicked);
        OnSlotClicked = null;
        _boundData = null;
    }

    private void OnButtonClicked()
    {
        OnSlotClicked?.Invoke(_boundData);
    }
}
