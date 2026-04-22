using UnityEngine;
using System.Collections;

public class RemoveModificationCommand : ICommand
{
    private AbilityModel ability;

    public RemoveModificationCommand(AbilityModel ability)
    {
        this.ability = ability;
    }

    public void Execute()
    {
        if (ability == null)
            return;

        var mod = ability.AssignedModification;

        if (mod == null)
            return;

        mod.IsUsed = false;
        mod.AssignedTo = null;
        ability.AssignedModification = null;

        Debug.Log($"Мод '{mod.Name}' ({mod.GetHashCode()}) снят с способности '{ability.Name}' ({ability.GetHashCode()})");
    }
}