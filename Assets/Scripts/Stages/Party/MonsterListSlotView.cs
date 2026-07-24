using UnityEngine;
using UnityEngine.UI;

public class MonsterListSlotView : MonoBehaviour
{
    [SerializeField] private Image _portraitImage;

    // TODO: MonsterId 로 몬스터 초상화 스프라이트 로드
    public void Bind(string monsterId)
    {
        if (null == _portraitImage)
        {
            return;
        }

        _portraitImage.enabled = false;
    }
}