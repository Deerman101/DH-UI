using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Типы модификаторов
/// </summary>
public enum ModificationType // По-хорошему, это всё нужно было разбить на разные файлы, но для удобства проверки лучше сделать так.
{
    Psyker,
    Dot,
    Attack,
    Buff,
    Debuff // в задании ни слова, но на референсе есть.
}

/// <summary>
/// Данные способности
/// </summary>
public class AbilityModel
{
    public string Name;
    public Sprite Icon;
    public List<ModificationType> SupportedTypes = new();
    public ModificationModel AssignedModification;
}

/// <summary>
/// Данные модификатора
/// </summary>
public class ModificationModel
{
    public string Name;
    public ModificationType Type;
    public bool IsUsed;
    public AbilityModel AssignedTo;
}

/// <summary>
/// Данные персонажа
/// </summary>
public class CharacterModel
{
    public string Name;
    public int HP;
    public int Armor;

    public List<AbilityModel> Abilities = new();
    public List<ModificationModel> Modifications = new();
}