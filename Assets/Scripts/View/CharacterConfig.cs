using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterConfig : MonoBehaviour
{
    [Header("Основные данные")]
    public string CharacterName;
    public int HP;
    public int Armor;
    public Sprite Portrait;

    [Header("Способности персонажа")]
    public List<AbilityData> Abilities = new();

    [Header("Модификаторы персонажа")]
    public List<ModificationData> Modifications = new();
}

[Serializable]
public class AbilityData
{
    public string Name;
    public Sprite Icon;
    public List<ModificationType> SupportedTypes;
}

[Serializable]
public class ModificationData
{
    public string Name;
    public ModificationType Type;
}