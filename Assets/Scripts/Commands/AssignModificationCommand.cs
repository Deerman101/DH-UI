using UnityEngine;
using System.Collections;

public class AssignModificationCommand : ICommand
{
    private ModificationModel mod;
    private AbilityModel ability;

    public AssignModificationCommand(ModificationModel mod, AbilityModel ability)
    {
        this.mod = mod;
        this.ability = ability;
    }

    public void Execute()
    {
        if (mod == null || ability == null)
            return;

        if (mod.AssignedTo != null)
            mod.AssignedTo.AssignedModification = null;

        if (ability.AssignedModification != null)
        {
            ability.AssignedModification.IsUsed = false;
            ability.AssignedModification.AssignedTo = null;
        }
        
        ability.AssignedModification = mod;
        mod.IsUsed = true;
        mod.AssignedTo = ability;

        Debug.Log($"Мод '{mod.Name}' ({mod.GetHashCode()}) назначен на способность '{ability.Name}' ({ability.GetHashCode()})");
    }
}