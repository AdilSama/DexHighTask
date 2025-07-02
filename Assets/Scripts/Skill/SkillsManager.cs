using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillsManager : MonoBehaviour, ITick
{
    [SerializeField] RectTransform SkillBtn;
    [SerializeField] RectTransform SkillBtnOnAnchor, SkillBtnOffAnchor;
    [SerializeField] CanvasGroup SkillsUICG;

    [SerializeField] List<Ability> Abilities= new();
    [SerializeField] List<RectTransform> AbilitiesOnAnchors = new List<RectTransform>();
    [SerializeField] List<RectTransform> AbilitiesOffAnchors = new List<RectTransform>();
    [SerializeField] RectTransform Center;
    [SerializeField] List<float> AbilitiesOnScale = new List<float>();
    [SerializeField] List<int> AbilitiesIndex = new List<int>();
    [SerializeField] TMP_Text AbilityTxt;

    bool SkillUIActive=> SkillsUICG.gameObject.activeSelf;

    public static Action OnChangeAbility;

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            StartCoroutine(ChangeAbility());
    }

    IEnumerator ChangeAbility()
    {
        EventSystemHelper.DisableInput();
        int skillCount = Enum.GetValues(typeof(Skills)).Length;
        StaticVars.CurrentSkill = (Skills)((((int)StaticVars.CurrentSkill) + 1) % skillCount);
        SetAbilitiesIndex();
        HighlightActiveAbility(0.25f);
        yield return new WaitForSeconds(0.25f);
        EventSystemHelper.EnableInput();
        OnChangeAbility?.Invoke();
    }

    public void SkillBtnOnClick()
    {
        EventSystemHelper.DisableInput();
        if (SkillUIActive)
            Close();
        else
            Open();
    }

    void Open()
    {
        SkillsUICG.gameObject.SetActive(true);
        SetAbilitiesIndex();
        StartCoroutine(DelayedOpenTweens());
        ProcessingUpdate.Instance.Subscribe(this);
    }

    IEnumerator DelayedOpenTweens()
    {
        yield return null;

        var duration = 0.25f;
        Tween.Move(SkillBtn, SkillBtnOnAnchor.anchoredPosition, duration);
        Tween.Scale(SkillBtn.gameObject, Vector3.one * 1.75f, duration);
        var clockwise = IsClockwise();
        HighlightActiveAbility(duration, clockwise);
        Tween.Fade(SkillsUICG, 1f, duration).SetOnComplete(EventSystemHelper.EnableInput);
    }

    void Close()
    {
        var duration = 0.25f;
        Tween.Move(SkillBtn, SkillBtnOffAnchor.anchoredPosition, duration);
        Tween.Scale(SkillBtn.gameObject, Vector3.one, duration);
        var clockwise = !IsClockwise();

        var length = Abilities.Count;
        for (int i = 0; i < length; i++)
        { 
            Tween.MoveCircular(Abilities[i].rt, AbilitiesOffAnchors[i], Center, duration, clockwise);
            Tween.Scale(Abilities[i].gameObject, Vector3.one, duration);
        }
        
        Tween.Fade(SkillsUICG, 0f, duration).SetOnComplete(() => 
        { 
            SkillsUICG.gameObject.SetActive(false);
            EventSystemHelper.EnableInput();
        });
        ProcessingUpdate.Instance.Unsubscribe(this);
    }

    void SetAbilitiesIndex()
    {
        AbilitiesIndex.Clear();
        var currentSkill = (int)StaticVars.CurrentSkill;
        int skillCount = Enum.GetValues(typeof(Skills)).Length;
        for (int i = 0; i < skillCount; i++)
            AbilitiesIndex.Add((i + currentSkill) % skillCount);
    }

    bool IsClockwise()
    {
        if((int)StaticVars.CurrentSkill > 1)
            return true;
        return false;
    }

    void HighlightActiveAbility(float duration, bool clockwise = true)
    {
        var length = Abilities.Count;
        for (int i = 0; i < length; i++)
        {
            Tween.MoveCircular(Abilities[AbilitiesIndex[i]].rt, AbilitiesOnAnchors[i], Center, duration, clockwise);
            Tween.Scale(Abilities[AbilitiesIndex[i]].gameObject, Vector3.one * AbilitiesOnScale[i], duration);
        }
        AbilityTxt.text = StaticVars.CurrentSkill.ToString();
    }

}
