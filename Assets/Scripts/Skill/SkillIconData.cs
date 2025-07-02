using System;
using UnityEngine;

public enum Skills { Wind, Light, Water, Fire, Dark }

[Serializable]
public class SkillIconData
{
    public Skills Type;
    public Sprite ActiveIcon, DeactiveIcon;

    public Sprite GetIcon() => Type == StaticVars.CurrentSkill ? ActiveIcon : DeactiveIcon;
}
