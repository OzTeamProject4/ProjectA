using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private Button _receiveButton;
    [SerializeField] private Image _rewardIconImage;

    [SerializeField] private Sprite _buttonSpriteA;
    [SerializeField] private Sprite _buttonSpriteB;
    [SerializeField] private Sprite _buttonSpriteC;

    private int _buttonSpriteIndex;

    public void Bind(MissionData mission)
    {
        _contentText.text =
            $"{mission.Title}\n" +
            $"{mission.Description}\n";

        // 버튼 클릭 활성화
        _receiveButton.interactable = true;

        // 중복 등록 방지 후 클릭 이벤트 등록
        _receiveButton.onClick.RemoveListener(OnReceiveButtonClicked);
        _receiveButton.onClick.AddListener(OnReceiveButtonClicked);

        // 처음에는 이미지 A
        _buttonSpriteIndex = 0;
        ApplyReceiveButtonSprite();

        // 보상이 없으면 아이콘 숨김
        if (mission.Rewards == null || mission.Rewards.Count == 0)
        {
            if (_rewardIconImage != null)
            {
                _rewardIconImage.enabled = false;
            }

            return;
        }

        MissionRewardData reward = mission.Rewards[0];
        string iconKey = GetRewardIconKey(reward.ItemId);

        SpriteLoader.LoadIntoAsync(
            _rewardIconImage,
            iconKey,
            destroyCancellationToken).Forget();
    }

    private void OnReceiveButtonClicked()
    {
        _buttonSpriteIndex = (_buttonSpriteIndex + 1) % 3;

        ApplyReceiveButtonSprite();
    }

    private void ApplyReceiveButtonSprite()
    {
        Sprite currentSprite;

        switch (_buttonSpriteIndex)
        {
            case 1:
                currentSprite = _buttonSpriteB;
                break;

            case 2:
                currentSprite = _buttonSpriteC;
                break;

            default:
                currentSprite = _buttonSpriteA;
                break;
        }

        if (_receiveButton.image != null && currentSprite != null)
        {
            _receiveButton.image.sprite = currentSprite;
        }
    }

    private static string GetRewardIconKey(string rewardId)
    {
        if (GameManager.Instance.DataManager.TryGetDataTable(
                out Dictionary<string, ItemData> itemTable))
        {
            foreach (ItemData item in itemTable.Values)
            {
                if (item.ForeignKey == rewardId)
                {
                    return item.IconKey;
                }
            }
        }

        return null;
    }
}