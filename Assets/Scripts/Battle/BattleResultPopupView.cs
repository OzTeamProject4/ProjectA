using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleResultPopupView : BaseUI
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _isClearedText;
    [SerializeField] private Image _isClearedImage;
    [SerializeField] private Sprite _clearedSprite;
    [SerializeField] private Sprite _failedSprite;
    [SerializeField] private Transform _rewardContainer;
    [SerializeField] private RewardSlotView _rewardSlotPrefab;
    [SerializeField] private Button _returnButton;

    private readonly List<RewardSlotView> _spawnedRewards = new List<RewardSlotView>();

    private UniTaskCompletionSource _completionSource;
    private bool _isSubscribed;

    private void OnDisable()
    {
        Unsubscribe();
    }

    private void OnDestroy()
    {
        Unsubscribe();
        ClearRewards();
    }

    public UniTask WaitForReturnAsync(bool isVictory, string stageId)
    {
        BattleResultPopupViewModel viewModel = new BattleResultPopupViewModel(isVictory, stageId);

        RefreshDisplay(viewModel);

        _completionSource = new UniTaskCompletionSource();

        Subscribe();

        return _completionSource.Task;
    }

    private void Subscribe()
    {
        if (_isSubscribed)
        {
            return;
        }

        if (null != _returnButton)
        {
            _returnButton.onClick.AddListener(HandleClickReturn);
        }

        _isSubscribed = true;
    }

    private void Unsubscribe()
    {
        if (_isSubscribed)
        {
            if (null != _returnButton)
            {
                _returnButton.onClick.RemoveListener(HandleClickReturn);
            }

            _isSubscribed = false;
        }

        // 선택 없이 닫히면 대기 지점이 멈추지 않도록 완료
        Complete();
    }

    private void RefreshDisplay(BattleResultPopupViewModel viewModel)
    {
        if (null != _nameText)
        {
            _nameText.text = viewModel.StageName;
        }

        RefreshClearedState(viewModel.IsVictory);
        RefreshRewards(viewModel);
    }

    private void RefreshClearedState(bool isVictory)
    {
        if (isVictory)
        {
            if (null != _isClearedText)
            {
                _isClearedText.text = "클리어";
            }

            if (null != _isClearedImage && null != _clearedSprite)
            {
                _isClearedImage.sprite = _clearedSprite;
            }

            return;
        }

        if (null != _isClearedText)
        {
            _isClearedText.text = "실패";
        }

        if (null != _isClearedImage && null != _failedSprite)
        {
            _isClearedImage.sprite = _failedSprite;
        }
    }

    private void RefreshRewards(BattleResultPopupViewModel viewModel)
    {
        ClearRewards();

        if (null == _rewardContainer || null == _rewardSlotPrefab)
        {
            return;
        }

        foreach (RewardSlotViewModel rewardViewModel in viewModel.GetRewards())
        {
            RewardSlotView slot = Instantiate(_rewardSlotPrefab, _rewardContainer);
            slot.Bind(rewardViewModel);

            _spawnedRewards.Add(slot);
        }
    }

    private void ClearRewards()
    {
        foreach (RewardSlotView slot in _spawnedRewards)
        {
            if (null == slot)
            {
                continue;
            }

            Destroy(slot.gameObject);
        }

        _spawnedRewards.Clear();
    }

    private void HandleClickReturn()
    {
        Complete();
    }

    private void Complete()
    {
        if (null == _completionSource)
        {
            return;
        }

        UniTaskCompletionSource source = _completionSource;
        _completionSource = null;
        source.TrySetResult();
    }
}
