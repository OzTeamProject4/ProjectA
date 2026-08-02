using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueView : BaseUI
{
    [Header("Background")]
    [SerializeField] private Image _backgroundImage; 

    [Header("Dialogue")]
    [SerializeField] private GameObject _dialogueRoot;
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _dialogueText;
    [SerializeField] private DialogueNextButton _nextButton;

    [Header("CharacterPortrait")]
    [SerializeField] private CharacterPortraitView _leftPortrait;
    [SerializeField] private CharacterPortraitView _centerPortrait;
    [SerializeField] private CharacterPortraitView _rightPortrait;

    [Header("Choice")]
    [SerializeField] private ChoiceListView _choiceListView;

    [Header("Options")]
    [SerializeField] private GameObject _hideGroupRoot;
    [SerializeField] private DialogueSkipButton _skipButton;
    [SerializeField] private DialogueAutoButton _autoButton;
    [SerializeField] private DialogueLogButton _logButton;
    [SerializeField] private DialogueHideButton _hideButton;

    private bool _isInitalized;
    private bool _isDialogueVisible;

    private DialogueViewModel _dialogueViewModel;

    private CancellationTokenSource _loadCts;

    private UniTaskCompletionSource _initializeTask;

    private void Awake()
    {
        UnityUtil.ValidateReference(_speakerNameText, nameof(DialogueView), nameof(_speakerNameText));
        UnityUtil.ValidateReference(_dialogueText, nameof(DialogueView), nameof(_dialogueText));
        UnityUtil.ValidateReference(_dialogueText, nameof(DialogueView), nameof(_leftPortrait));
        UnityUtil.ValidateReference(_dialogueText, nameof(DialogueView), nameof(_centerPortrait));
        UnityUtil.ValidateReference(_dialogueText, nameof(DialogueView), nameof(_rightPortrait));
        UnityUtil.ValidateReference(_nextButton, nameof(DialogueView), nameof(_nextButton));
        UnityUtil.ValidateReference(_choiceListView, nameof(DialogueView), nameof(_choiceListView));
        UnityUtil.ValidateReference(_skipButton, nameof(DialogueView), nameof(_skipButton));
        UnityUtil.ValidateReference(_autoButton, nameof(DialogueView), nameof(_autoButton));
        UnityUtil.ValidateReference(_logButton, nameof(DialogueView), nameof(_logButton));
        UnityUtil.ValidateReference(_hideButton, nameof(DialogueView), nameof(_hideButton));

        _dialogueViewModel = new DialogueViewModel();
    }

    private void OnEnable()
    {
        _initializeTask = new UniTaskCompletionSource();
        _isInitalized = false;
        _isDialogueVisible = true;

        _dialogueViewModel.OnEnable();
        _dialogueViewModel.ModelPropertyChanged += OnModelPropertyChanged;

        _dialogueViewModel.Refresh();
        UpdateHideButtonSprite();

        SubscribeEvents();
    }

    private void OnDisable()
    {
        if (_loadCts != null)
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
            _loadCts = null;
        }

        _dialogueViewModel.OnDisable();
        _dialogueViewModel.ModelPropertyChanged -= OnModelPropertyChanged;

        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        _dialogueViewModel.Dispose();
        _dialogueViewModel = null;
    }

    public UniTask WaitUntilInitializedAsync()
    {
        return _initializeTask.Task;
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(_dialogueViewModel.Background):
                LoadBackgroundImageAsync().Forget();
                break;
            case nameof(_dialogueViewModel.SpeakerNameText):
                UpdateSpeakerNameText();
                break;
            case nameof(_dialogueViewModel.DialogueText):
                UpdateDialogueText();
                break;
            case nameof(_dialogueViewModel.LeftCharacterId):
                UpdateLeftPortrait();
                break;
            case nameof(_dialogueViewModel.CenterCharacterId):
                UpdateCenterPortrait();
                break;
            case nameof(_dialogueViewModel.RightCharacterId):
                UpdateRightPortrait();
                break;
            case nameof(_dialogueViewModel.ActiveCharacterId):
                UpdatePortraitState();
                break;
            case nameof(_dialogueViewModel.Choices):
                RefreshChoices();
                break;
            case nameof(_dialogueViewModel.IsChoiceOpen):
                UpdateChoiceListVisibility();
                break;
            case nameof(_dialogueViewModel.IsAutoMode):
                UpdateAutoButtonSprite();
                break;
        }
    }

    private async UniTask LoadBackgroundImageAsync()
    {
        if (_loadCts != null)
        {
            _loadCts.Cancel();
            _loadCts.Dispose();
        }

        _loadCts = new CancellationTokenSource();

        Sprite backgroundSprite = await LoadSpriteAsync(_dialogueViewModel.Background, _loadCts.Token);

        _backgroundImage.sprite = backgroundSprite;

        if (!_isInitalized)
        {
            _isInitalized = true;

            _initializeTask.TrySetResult();
        }
    }

    private void UpdateSpeakerNameText()
    {
        _speakerNameText.text = _dialogueViewModel.SpeakerNameText;
    }

    private void UpdateDialogueText()
    {
        _dialogueText.text = _dialogueViewModel.DialogueText;
    }

    private void UpdateLeftPortrait()
    {
        _leftPortrait.SetPortrait(_dialogueViewModel.LeftCharacterId);
    }

    private void UpdateCenterPortrait()
    {
        _centerPortrait.SetPortrait(_dialogueViewModel.CenterCharacterId);
    }

    private void UpdateRightPortrait()
    {
        _rightPortrait.SetPortrait(_dialogueViewModel.RightCharacterId);
    }

    private void UpdatePortraitState()
    {
        _leftPortrait.UpdateState(_dialogueViewModel.ActiveCharacterId);
        _centerPortrait.UpdateState(_dialogueViewModel.ActiveCharacterId);
        _rightPortrait.UpdateState(_dialogueViewModel.ActiveCharacterId);
    }

    private void RefreshChoices()
    {
        _choiceListView.BindChoices(_dialogueViewModel.Choices);
    }

    private void UpdateChoiceListVisibility()
    {
        _choiceListView.gameObject.SetActive(_dialogueViewModel.IsChoiceOpen);
    }

    private void UpdateAutoButtonSprite()
    {
        _autoButton.UpdateButtonSprite(_dialogueViewModel.IsAutoMode);
    }

    private void UpdateHideButtonSprite()
    {
        _hideButton.UpdateButtonSprite(_isDialogueVisible);
    }

    private void SubscribeEvents()
    {
        _nextButton.ButtonClicked += HandleNextClicked;
        _choiceListView.ChoiceSelected += HandleChoiceSelected;
        _autoButton.ButtonClicked += HandleAutoButtonClicked;
        _skipButton.ButtonClicked += HandleSkipButtonClicked;
        _logButton.ButtonClicked += HandleLogButtonClicked;
        _hideButton.ButtonClicked += HandleHideButtonClicked;
    }

    private void UnsubscribeEvents()
    {
        _nextButton.ButtonClicked -= HandleNextClicked;
        _choiceListView.ChoiceSelected -= HandleChoiceSelected;
        _autoButton.ButtonClicked -= HandleAutoButtonClicked;
        _skipButton.ButtonClicked -= HandleSkipButtonClicked;
        _logButton.ButtonClicked -= HandleLogButtonClicked;
        _hideButton.ButtonClicked -= HandleHideButtonClicked;
    }

    private void HandleNextClicked()
    {
        GameManager.Instance.DialogueManager.AdvanceDialogue();
    }

    private void HandleChoiceSelected(string nextDialogueId)
    {
        GameManager.Instance.DialogueManager.SelectChoice(nextDialogueId);
    }

    private void HandleAutoButtonClicked()
    {
        GameManager.Instance.DialogueManager.RequestToggleAutoMode();
    } 

    private void HandleSkipButtonClicked()
    {
        GameManager.Instance.DialogueManager.SkipDialogue();
    } 

    private void HandleLogButtonClicked()
    {
        GameManager.Instance.DialogueManager.OpenHistoryDialogue();
    }

    private void HandleHideButtonClicked()
    {
        _isDialogueVisible = !_isDialogueVisible;

        if (!_isDialogueVisible)
        {
            GameManager.Instance.DialogueManager.HideDialogue();
        }

        _dialogueRoot.SetActive(_isDialogueVisible);
        _hideGroupRoot.SetActive(_isDialogueVisible);
        UpdateHideButtonSprite();
    }

    private async UniTask<Sprite> LoadSpriteAsync(string key, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            Debug.LogError($"[{nameof(DialogueView)}:{nameof(LoadSpriteAsync)}] 전달된 Sprite key가 null이거나 빈 문자열입니다.");
            return null;
        }

        Sprite sprite = await GameManager.Instance.ResourceManager.LoadAssetAsync<Sprite>(key, cancellationToken);

        if (sprite == null)
        {
            Debug.LogError($"[{nameof(DialogueView)}:{nameof(LoadSpriteAsync)}] '{key}' Sprite를 찾을 수 없습니다.");
            return null;
        }

        return sprite;
    }
}