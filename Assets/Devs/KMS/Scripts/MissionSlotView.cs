using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionSlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _contentText;
    [SerializeField] private Button _receiveButton;

    public void Bind(MissionData mission)
    {
        string rewardText = "보상 없음";

        if (mission.Rewards != null && mission.Rewards.Count > 0)
        {
            MissionRewardData reward = mission.Rewards[0];
                rewardText = $"{GetRewardName(reward.ItemId)} × {reward.Amount}";
        }

        _contentText.text =
            $"{mission.Title}\n" +
            $"{mission.Description}\n" +
            $"진행도: 0 / {mission.TargetCount}\n" +
            $"보상: {rewardText}";

        // 진행도 기능(임시 비활성)
        _receiveButton.interactable = false;
    }

    private static string GetRewardName(string rewardId)
    {
        if (GameManager.Instance.DataManager.TryGetDataTable(
                out Dictionary<string, ItemData> itemTable))
        {
            foreach (ItemData item in itemTable.Values)
            {
                if (item.ForeignKey == rewardId)
                {
                    return item.Name;
                }
            }
        }

        return rewardId;
    }
}
