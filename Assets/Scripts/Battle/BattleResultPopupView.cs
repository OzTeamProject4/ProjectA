using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum BattleResultChoice
{
    None,
    Return,
    Retry
}

public class BattleResultPopupView : BaseUI
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _isClearedText;
    [SerializeField] private Image _isClearedImage;
    [SerializeField] private Sprite _clearedSprite;
    [SerializeField] private Sprite _failedSprite;
    [SerializeField] private Transform _rewardContainer;
    [SerializeField] private GameObject _rewardSection;
    [SerializeField] private RewardSlotView _rewardSlotPrefab;
    [SerializeField] private Button _returnButton;
    [SerializeField] private Button _retryButton;

    private readonly List<RewardSlotView> _spawnedRewards = new List<RewardSlotView>();

    private UniTaskCompletionSource<BattleResultChoice> _completionSource;
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

    public UniTask<BattleResultChoice> WaitForChoiceAsync(bool isVictory, string stageId)
    {
        BattleResultPopupViewModel viewModel = new BattleResultPopupViewModel(isVictory, stageId);

        RefreshDisplay(viewModel);

        _completionSource = new UniTaskCompletionSource<BattleResultChoice>();

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

        if (null != _retryButton)
        {
            _retryButton.onClick.AddListener(HandleClickRetry);
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

            if (null != _retryButton)
            {
                _retryButton.onClick.RemoveListener(HandleClickRetry);
            }

            _isSubscribed = false;
        }

        // 선택 없이 닫히면 대기 지점이 멈추지 않도록 완료
        Complete(BattleResultChoice.None);
    }

    private void RefreshDisplay(BattleResultPopupViewModel viewModel)
    {
        if (null != _nameText)
        {
            _nameText.text = viewModel.StageName;
        }

        RefreshClearedState(viewModel.IsVictory);
        RefreshResultSections(viewModel.IsVictory);
        RefreshRewards(viewModel);
    }

    private void RefreshResultSections(bool isVictory)
    {
        if (null != _retryButton)
        {
            _retryButton.gameObject.SetActive(!isVictory);
        }

        GameObject rewardRoot = null != _rewardSection ? _rewardSection : GetRewardContainerObject();

        if (null != rewardRoot)
        {
            rewardRoot.SetActive(isVictory);
        }
    }

    private GameObject GetRewardContainerObject()
    {
        if (null == _rewardContainer)
        {
            return null;
        }

        return _rewardContainer.gameObject;
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
        Complete(BattleResultChoice.Return);
    }

    private void HandleClickRetry()
    {
        Complete(BattleResultChoice.Retry);
    }

    private void Complete(BattleResultChoice choice)
    {
        if (null == _completionSource)
        {
            return;
        }

        UniTaskCompletionSource<BattleResultChoice> source = _completionSource;
        _completionSource = null;
        source.TrySetResult(choice);
    }
}
