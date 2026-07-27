using TMPro;
using UnityEngine;

public class DialogueView : BaseUI
{
    [Header("Dialogue")]
    [SerializeField] private TMP_Text _speakerNameText;
    [SerializeField] private TMP_Text _dialogueText;

    [Header("CharacterPortrait")]
    [SerializeField] private CharacterPortraitView _leftPortrait;
    [SerializeField] private CharacterPortraitView _centerPortrait;
    [SerializeField] private CharacterPortraitView _rightPortrait;

    [Header("Buttons")]
    [SerializeField] private DialogueButton _nextButton;

    [Header("Choice")]
    [SerializeField] private ChoiceListView _choiceListView;

    private DialogueViewModel _dialogueViewModel;

    private void Awake()
    {
        UnityUtil.ValidateReference(_speakerNameText, nameof(AudioClip), nameof(_speakerNameText));
        UnityUtil.ValidateReference(_dialogueText, nameof(AudioClip), nameof(_dialogueText));
        UnityUtil.ValidateReference(_dialogueText, nameof(AudioClip), nameof(_leftPortrait));
        UnityUtil.ValidateReference(_dialogueText, nameof(AudioClip), nameof(_centerPortrait));
        UnityUtil.ValidateReference(_dialogueText, nameof(AudioClip), nameof(_rightPortrait));
        UnityUtil.ValidateReference(_nextButton, nameof(AudioClip), nameof(_nextButton));
        UnityUtil.ValidateReference(_choiceListView, nameof(AudioClip), nameof(_choiceListView));

        _dialogueViewModel = new DialogueViewModel();
    }

    private void OnEnable()
    {
        _dialogueViewModel.OnEnable();
        _dialogueViewModel.ModelPropertyChanged += OnModelPropertyChanged;

        _dialogueViewModel.Refresh();

        SubscribeEvents();
    }

    private void OnDisable()
    {
        _dialogueViewModel.OnDisable();
        _dialogueViewModel.ModelPropertyChanged -= OnModelPropertyChanged;

        UnsubscribeEvents();
    }

    private void OnDestroy()
    {
        _dialogueViewModel.Dispose();
        _dialogueViewModel = null;
    }

    private void OnModelPropertyChanged(string propertyName)
    {
        switch (propertyName)
        {
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

    private void SubscribeEvents()
    {
        _nextButton.ButtonClicked += HandleNextClicked;
        _choiceListView.ChoiceSelected += HandleChoiceSelected;
    }

    private void UnsubscribeEvents()
    {
        _nextButton.ButtonClicked -= HandleNextClicked;
        _choiceListView.ChoiceSelected -= HandleChoiceSelected;
    }

    private void HandleNextClicked()
    {
        GameManager.Instance.DialogueManager.AdvanceDialogue();
    }

    private void HandleChoiceSelected(string nextDialogueId)
    {
        GameManager.Instance.DialogueManager.SelectChoice(nextDialogueId);
    }
}


    //[Header("Background")]
    //[SerializeField] private Image _backgroundImage; 

    //[Header("Character")]
    //[SerializeField] private Image _leftPortraitImage; 
    //[SerializeField] private Image _centerPortraitImage; 
    //[SerializeField] private Image _rightPortraitImage; 
    


    //[Header("Buttons")]
    //[SerializeField] private Button _skipButton;
    //[SerializeField] private Button _autoButton; 
    //[SerializeField] private Button _logButton; 
    




    //private void Refresh() { _speakerNameText.text = ViewModel.SpeakerName; _dialogueText.text = ViewModel.Dialogue; UpdateBackground(); UpdatePortrait(); UpdateChoice(); }

    //private void Awake()
    //{
    //    _nextButton.onClick.AddListener(OnNextClicked);

    //    _skipButton.onClick.AddListener(OnSkipClicked);

    //    _autoButton.onClick.AddListener(OnAutoClicked);

    //    _logButton.onClick.AddListener(OnLogClicked);
    //}

    //private void OnNextClicked()
    //{
    //    ViewModel.Next();
    //}



    //[SerializeField] private Image _leftCharacter;
    //[SerializeField] private Image _centerCharacter;
    //[SerializeField] private Image _rightCharacter;

    //[SerializeField] private TMP_Text _nameText;
    //[SerializeField] private TMP_Text _dialogueText;

    //[SerializeField] private RectTransform _choiceRoot;

    //[SerializeField] private Image _continueIcon;

//}
//public class DaniTech_DialogueUI : DaniTechUIBase
//{
//    [SerializeField] private GameObject Layout_CharacterName;
//    [SerializeField] private Text Text_Character;
//    [SerializeField] private Text Text_Description;
//    [SerializeField] private DaniTechUIButton Button_Next;

//    private string _currentDialogueId;
//    private Queue<string> _descriptionQueue = new Queue<string>();

//    private void OnEnable()
//    {
//        Button_Next.BindOnClickButtonEvent(OnClick_Next);
//    }

//    // 다이얼로그에서 Next 버튼이 눌러질때 호출된다
//    public void OnClick_Next()
//    {
//        // 다음 대사가 있는지 체크한다
//        bool isNextDescriptionExist = CheckAndSetDescription();

//        if (isNextDescriptionExist)
//        {
//            return;
//        }

//        // 대사가 없다면, 다음으로 이어지는 다이얼로그가 있는지 체크한다
//        bool isNextDialogueExist = CheckAndStartNextDialogue();
//        if (isNextDialogueExist == false)
//        {
//            DaniTechUIManager.Instance.CloseContentUI(DaniTechUIType.DNDialogueUI);
//        }
//    }

//    private bool CheckAndStartNextDialogue()
//    {
//        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(_currentDialogueId);
//        if (dialogueData == null)
//        {
//            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
//            return false;
//        }

//        // 현재 데이터를 기준으로 다음 다이얼로그가 있는지 체크해보고, 있다면 다음 다이얼로그를 시작한다!
//        string nextDialogueId = dialogueData.NextDialogueId;
//        if (string.IsNullOrEmpty(nextDialogueId) == false)
//        {
//            StartDialogue(nextDialogueId);
//            return true;
//        }

//        return false;
//    }

//    // 다이얼로그를 시작하는 메서드 (외부에서 UIManager를 통해 다이얼로그 시작을 요청할때도 쓴다!)
//    public void StartDialogue(string dialogeId)
//    {
//        var dialogueData = DaniTechGameDataManager.Instance.GetDNDialogueData(dialogeId);
//        if (dialogueData == null)
//        {
//            Debug.LogWarning($"다이얼로그 데이터가 존재하지 않습니다 {dialogueData}");
//            return;
//        }

//        // 현재 진행중인 다이얼로그 Id는 다음 다이얼로그가 있는지 체크할 때 쓸 수 있도록 보관한다
//        _currentDialogueId = dialogeId;

//        // 혹시 현재 대사가 너무 길거나 다음 페이지 처리가 필요할 때 <np> 키워드로 잘라주자!
//        if (dialogueData.Description.Contains("<np>"))
//        {
//            string[] dialogueDescriptionList = dialogueData.Description.Split("<np>");
//            foreach (string desc in dialogueDescriptionList)
//            {
//                _descriptionQueue.Enqueue(desc);
//            }
//            CheckAndSetDescription();
//        }
//        else
//        {
//            // Np 태그가 없다면 바로 다이얼로그 UI를 세팅하자
//            SetCurrentDialogueDescription(dialogueData.Description);
//        }

//        SetCharacterName(dialogueData.CharacterDataId);
//    }

//    private bool CheckAndSetDescription()
//    {
//        bool isNextDescriptionExsist = (_descriptionQueue.Count > 0);
//        if (isNextDescriptionExsist)
//        {
//            string desc = _descriptionQueue.Dequeue();
//            SetCurrentDialogueDescription(desc);
//        }

//        return isNextDescriptionExsist;
//    }

//    private void SetCharacterName(string characterDataId)
//    {
//        // 캐릭터 정보가 있다면 말하는 이의 추가 정보를 표기해줄 수 있도록 연동하는 부분
//        bool isActive = (string.IsNullOrEmpty(characterDataId) == false);
//        Layout_CharacterName.SetActive(isActive);

//        if (isActive)
//        {
//            var characterData = DaniTechGameDataManager.Instance.GetCharacterData(characterDataId);
//            if (characterData != null)
//            {
//                Text_Character.text = characterData.Name;
//            }
//        }
//    }

//    private void SetCurrentDialogueDescription(string description)
//    {
//        Text_Description.text = description;
//    }
//}
