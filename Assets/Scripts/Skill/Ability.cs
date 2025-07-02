using UnityEngine;
using UnityEngine.UI;

public class Ability : MonoBehaviour
{
    [SerializeField] SkillIconData skill;
    [SerializeField] Image Icon;
    public RectTransform rt;

    private void Start() => SetAbilityIcon();
    private void OnEnable() => SkillsManager.OnChangeAbility += SetAbilityIcon;
    private void OnDisable() => SkillsManager.OnChangeAbility -= SetAbilityIcon;

    private void SetAbilityIcon() => Icon.sprite = skill.GetIcon();

}
