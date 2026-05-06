using System;
using System.Collections.Generic;
using System.Linq;

public enum HighlightState
{
    None,
    Compatible,
    Incompatible,
    Selected
}
/////// <summary>
/////// ViewModel персонажа
/////// </summary>
public class CharacterViewModel
{
    public string Name;
    public int HP;
    public int Armor;

    public List<AbilityViewModel> Abilities;
    public List<ModificationViewModel> Mods;

    public ObservableProperty<AbilityViewModel> HoveredAbility = new();
    public ObservableProperty<ModificationViewModel> HoveredMod = new();
    private bool highlightCompatibleOnlyForDraggedAssignedMod;

    public event Action OnDataChanged;

    public CharacterViewModel(CharacterModel model)
    {
        Name = model.Name;
        HP = model.HP;
        Armor = model.Armor;

        Abilities = model.Abilities.Select(a => new AbilityViewModel(a)).ToList();
        Mods = model.Modifications.Select(m => new ModificationViewModel(m)).ToList();

        HoveredAbility.OnChanged += OnAbilityHover;
        HoveredMod.OnChanged += OnModHover;
    }

    public void SetHoveredAbility(AbilityViewModel ability)
    {
        HoveredAbility.Value = ability;
        HoveredMod.Value = null;
    }

    public void SetHoveredMod(ModificationViewModel mod)
    {
        HoveredMod.Value = mod;
        HoveredAbility.Value = null;
    }

    public void ClearHover()
    {
        HoveredAbility.Value = null;
        HoveredMod.Value = null;
    }

    public void BeginAssignedModDragHighlight(ModificationViewModel mod)
    {
        highlightCompatibleOnlyForDraggedAssignedMod = true;
        SetHoveredMod(mod);
    }

    public void EndAssignedModDragHighlight()
    {
        highlightCompatibleOnlyForDraggedAssignedMod = false;
        ClearHover();
    }

    public void NotifyDataChanged()
    {
        OnDataChanged?.Invoke();
    }

    void OnAbilityHover(AbilityViewModel ability)
    {
        foreach (var m in Mods)
        {
            if (ability == null)
            {
                m.Highlight.Value = HighlightState.None;
                continue;
            }

            if (ability.Model.AssignedModification != null)
            {
                if (ability.Model.AssignedModification == m.Model)
                    m.Highlight.Value = HighlightState.Selected;
                else
                    m.Highlight.Value = HighlightState.None;

                continue;
            }

            bool compatible = ability.IsCompatible(m);

            m.Highlight.Value = compatible
                ? HighlightState.Compatible
                : HighlightState.Incompatible;
        }
    }

    void OnModHover(ModificationViewModel mod)
    {
        foreach (var a in Abilities)
        {
            if (mod == null)
            {
                a.Highlight.Value = HighlightState.None;
                continue;
            }

            bool compatible = a.IsCompatible(mod);

            if (compatible)
                a.Highlight.Value = HighlightState.Compatible;
            else
                a.Highlight.Value = highlightCompatibleOnlyForDraggedAssignedMod
                    ? HighlightState.None
                    : HighlightState.Incompatible;
        }
    }
}

///// <summary>
///// ViewModel способности
///// </summary>
public class AbilityViewModel
{
    public AbilityModel Model;

    public ObservableProperty<HighlightState> Highlight = new();

    public ObservableProperty<ModificationModel> AssignedMod = new();

    public AbilityViewModel(AbilityModel m)
    {
        Model = m;
        AssignedMod.Value = Model.AssignedModification;
    }

    public bool IsCompatible(ModificationViewModel mod)
    {
        return Model.SupportedTypes.Contains(mod.Model.Type);
    }
}

///// <summary>
///// ViewModel модификации
///// </summary>
public class ModificationViewModel
{
    public ModificationModel Model;

    public ObservableProperty<HighlightState> Highlight = new();

    public ModificationViewModel(ModificationModel m)
    {
        Model = m;
    }
}