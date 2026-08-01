using TMPro;
using UnityEngine;

public class DialogueHistorySlotView : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _dialoguetext;

    public void UpdateSlot(DialogueData dialogueData)
    {
        _nameText.text = dialogueData.SpeakerName;
        _dialoguetext.text = dialogueData.Text;
    }

    public void SetActive(bool active)
    {
        if (gameObject.activeSelf == active)
        {
            return;
        }

        gameObject.SetActive(active);
    }
}
