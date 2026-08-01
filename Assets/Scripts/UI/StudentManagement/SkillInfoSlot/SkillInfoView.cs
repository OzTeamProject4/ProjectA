using Cysharp.Threading.Tasks;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillInfoView : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _nameText;
    [SerializeField] private TMP_Text _descText;

    private void Awake()
    {
        UnityUtil.ValidateReference(_iconImage, nameof(SkillInfoView), nameof(_iconImage));
        UnityUtil.ValidateReference(_nameText, nameof(SkillInfoView), nameof(_nameText));
        UnityUtil.ValidateReference(_descText, nameof(SkillInfoView), nameof(_descText));
    }

    public void SetSkill(CharacterSkillData skillData, CancellationToken cancellationToken)
    {
        gameObject.SetActive(true);

        _nameText.text = skillData.Name;
        _descText.text = BuildDescription(skillData);

        SpriteLoader.LoadIntoAsync(_iconImage, skillData.IconPath, cancellationToken).Forget();
    }

    public void Clear()
    {
        gameObject.SetActive(false);
    }

    private static string BuildDescription(CharacterSkillData skillData)
    {
        if (skillData.Type == CharacterSkillType.HealBuff)
        {
            return $"회복량 {skillData.HealAmount} / 이동속도 +{skillData.MoveSpeedBuff}";
        }

        if (skillData.Category == CharacterSkillCategory.Ultimate)
        {
            return $"피해량 {skillData.DamageCoefficient}%";
        }

        return $"피해량 {skillData.DamageCoefficient}% / 쿨타임 {skillData.Cooldown}초";
    }
}
