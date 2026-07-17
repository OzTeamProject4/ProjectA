using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ExpItemSlotView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private const float HoldDelaySeconds = 1.5f;
    private const float RepeatIntervalSeconds = 0.15f;

    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _countText;
    [SerializeField] private Button _selectButton;
    [SerializeField] private CanvasGroup _canvasGroup;

    [SerializeField] private float _unusableAlpha = 0.4f;

    private ExpItemSlotViewModel _viewModel;
    private bool _isSubscribed;
    private string _loadedSpriteKey;
    private CancellationTokenSource _holdCts;

    public event Action<string> OnClicked;

    private void OnDisable()
    {
        Unsubscribe();
        CancelHold();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        CancelHold();
        ReleaseSprite();
    }

    public void Bind(ExpItemSlotViewModel viewModel)
    {
        if (null == viewModel)
        {
            Debug.LogError("Bind: ExpItemSlotViewModel 이 null 입니다.");
            return;
        }

        Unsubscribe();

        _viewModel = viewModel;
        _nameText.text = _viewModel.Name;

        LoadIconAsync().Forget();

        Subscribe();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (null == _viewModel || !_viewModel.IsUsable)
        {
            return;
        }

        TriggerUse();
        StartHoldAsync().Forget();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        CancelHold();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CancelHold();
    }

    private void RefreshDisplay()
    {
        if (null == _viewModel)
        {
            return;
        }

        _countText.text = _viewModel.OwnedCount.ToString();

        bool isUsable = _viewModel.IsUsable;

        if (null != _canvasGroup)
        {
            _canvasGroup.alpha = isUsable ? 1f : _unusableAlpha;
        }

        if (null != _selectButton)
        {
            _selectButton.interactable = isUsable;
        }
        
        if (!isUsable)
        {
            CancelHold();
        }
    }

    private void Subscribe()
    {
        if (_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnChanged += HandleChanged;
        _isSubscribed = true;

        _viewModel.Initialize();
        RefreshDisplay();
    }

    private void Unsubscribe()
    {
        if (!_isSubscribed || null == _viewModel)
        {
            return;
        }

        _viewModel.OnChanged -= HandleChanged;
        _viewModel.Dispose();
        _isSubscribed = false;
    }

    private void HandleChanged()
    {
        RefreshDisplay();
    }

    private void TriggerUse()
    {
        OnClicked?.Invoke(_viewModel.DataId);
    }

    private async UniTaskVoid LoadIconAsync()
    {
        if (null == _iconImage)
        {
            return;
        }

        string spritePath = _viewModel.SpritePath;

        if (string.IsNullOrEmpty(spritePath))
        {
            _iconImage.enabled = false;
            return;
        }

        try
        {
            Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(spritePath, destroyCancellationToken);

            if (null == sprite)
            {
                _iconImage.enabled = false;
                return;
            }

            ReleaseSprite();
            _loadedSpriteKey = spritePath;

            _iconImage.enabled = true;
            _iconImage.sprite = sprite;
        }
        catch (OperationCanceledException)
        {
            // 오브젝트 파괴로 취소됨
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"[ExpItemSlotView] 아이콘 로드 실패. spritePath={spritePath}\n{exception}");
        }
    }

    private async UniTaskVoid StartHoldAsync()
    {
        CancelHold();

        _holdCts = new CancellationTokenSource();
        CancellationToken token = _holdCts.Token;

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(HoldDelaySeconds), cancellationToken: token);

            while (!token.IsCancellationRequested)
            {
                if (null == _viewModel || !_viewModel.IsUsable)
                {
                    break;
                }

                TriggerUse();

                await UniTask.Delay(TimeSpan.FromSeconds(RepeatIntervalSeconds), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            // 손을 떼거나 오브젝트 파괴로 취소됨
        }
    }

    private void CancelHold()
    {
        if (null == _holdCts)
        {
            return;
        }

        _holdCts.Cancel();
        _holdCts.Dispose();
        _holdCts = null;
    }

    private void ReleaseSprite()
    {
        if (string.IsNullOrEmpty(_loadedSpriteKey))
        {
            return;
        }

        GameManager.Instance.ResourceManager.ReleaseAsset(_loadedSpriteKey);
        _loadedSpriteKey = null;
    }
}