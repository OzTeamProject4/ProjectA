using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHudSlot : BaseUI
{
    [Header("UI Components")]
    [SerializeField] private TMP_Text _text_Name;
    [SerializeField] private TMP_Text _text_Level;
    [SerializeField] private Image _image_HpBarFill;
    [SerializeField] private GameObject _uiRoot; 

    [Header("Tracking Settings")]
    [SerializeField] private Vector2 _screenOffset = new Vector2(0, 30f);

    private EnemyViewModel _vm;
    [SerializeField] private Transform _targetHead;
    private Camera _mainCamera;
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        _mainCamera = Camera.main;

        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_HUD;
            _vm.PropertyChanged += OnPropertyChanged_HUD;
        }
    }

    public void BindEnemyViewModel(EnemyViewModel vm, Transform targetHead)
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_HUD;
        }

        _vm = vm;
        _targetHead = targetHead;

        if (_vm != null)
        {
            _vm.PropertyChanged += OnPropertyChanged_HUD;
            _vm.InvokeOnceOnInit();
        }
    }

   

    private void LateUpdate()
    {
        if (_targetHead == null || !_targetHead.gameObject.activeInHierarchy)
        {
            if (_uiRoot.activeSelf) _uiRoot.SetActive(false);
            return;
        }


        Vector3 viewportPos = _mainCamera.WorldToViewportPoint(_targetHead.position);

        // z < 0 : 카메라 뒤쪽
        // x, y가 0~1 범위를 벗어남 : 화면 왼쪽/오른쪽/위/아래 밖으로 나감
        bool isOffScreen = viewportPos.z < 0 ||
                           viewportPos.x < 0f || viewportPos.x > 1f ||
                           viewportPos.y < 0f || viewportPos.y > 1f;

        if (isOffScreen)
        {
            if (_uiRoot.activeSelf) _uiRoot.SetActive(false);
            return;
        }

        if (!_uiRoot.activeSelf) _uiRoot.SetActive(true);

        Vector3 screenPos = _mainCamera.WorldToScreenPoint(_targetHead.position);
        _rectTransform.position = (Vector2)screenPos + _screenOffset;
    }

    private void OnPropertyChanged_HUD(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(EnemyViewModel.IsActive):
                // 몬스터가 비활성화(Despawn) 되었을 때!
                if (!_vm.IsActive)
                {
                    Close();
                }
                break;

            case nameof(EnemyViewModel.Name):
                if (_text_Name != null) _text_Name.text = _vm.Name;
                break;

            case nameof(EnemyViewModel.CurrentLevel):
                if (_text_Level != null) _text_Level.text = $"Lv.{_vm.CurrentLevel}";
                break;

            case nameof(EnemyViewModel.CurrentHp):
            case nameof(EnemyViewModel.MaxHp): // MaxHp 변화에도 대응
                if (_image_HpBarFill != null && _vm.MaxHp > 0)
                {
                    _image_HpBarFill.fillAmount = (float)_vm.CurrentHp / _vm.MaxHp;
                }
                break;
        }
    }

    private void Close()
    {
        if (_vm != null)
        {
            _vm.PropertyChanged -= OnPropertyChanged_HUD;
        }

        var parentHud = GetComponentInParent<EnemyHud>();
        if (parentHud != null)
        {
           

            parentHud.RemoveHudSlot(this.gameObject);
        }
    }


}