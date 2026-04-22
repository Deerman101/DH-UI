using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Хранит текущее наведение мыши
/// </summary>
public static class HoverContext
{
    public static AbilityViewModel HoveredAbility;
    public static ModificationViewModel HoveredMod;
    public static Action OnHoverChanged;

    public static void SetAbility(AbilityViewModel ability)
    {
        HoveredAbility = ability;
        HoveredMod = null;

        OnHoverChanged?.Invoke();
    }

    public static void SetMod(ModificationViewModel mod)
    {
        HoveredMod = mod;
        HoveredAbility = null;

        OnHoverChanged?.Invoke();
    }

    public static void Clear()
    {
        HoveredAbility = null;
        HoveredMod = null;

        OnHoverChanged?.Invoke();
    }
}