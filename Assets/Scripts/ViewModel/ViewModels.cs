using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ViewModel модификатора
/// </summary>
public class ModificationViewModel
{
    public ModificationModel Model;

    public ObservableProperty<bool> Highlight = new();

    public ModificationViewModel(ModificationModel m)
    {
        Model = m;
    }
}

/// <summary>
/// ViewModel способности
/// </summary>
public class AbilityViewModel
{
    public AbilityModel Model;

    public ObservableProperty<bool> Highlight = new();
    public ObservableProperty<ModificationViewModel> Attached = new();

    public AbilityViewModel(AbilityModel m)
    {
        Model = m;
    }

    public bool IsCompatible(ModificationViewModel mod)
    {
        return Model.SupportedTypes.Contains(mod.Model.Type);
    }
}

/// <summary>
/// ViewModel персонажа
/// </summary>
public class CharacterViewModel
{
    public string Name;
    public int HP;
    public int Armor;

    public List<AbilityViewModel> Abilities;
    public List<ModificationViewModel> Mods;

    public ObservableProperty<AbilityViewModel> HoveredAbility = new();
    public ObservableProperty<ModificationViewModel> HoveredMod = new();

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

    public void AssignModification(ModificationViewModel mod, AbilityViewModel ability)
    {
        foreach (var a in Abilities)
        {
            if (a.Attached.Value == mod)
                a.Attached.Value = null;
        }

        ability.Attached.Value = mod;
    }

    void OnAbilityHover(AbilityViewModel ability)
    {
        foreach (var m in Mods)
            m.Highlight.Value = ability != null && ability.IsCompatible(m);
    }

    void OnModHover(ModificationViewModel mod)
    {
        foreach (var a in Abilities)
            a.Highlight.Value = mod != null && a.IsCompatible(mod);
    }
}