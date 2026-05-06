using UnityEngine;
using System.Collections.Generic;

public class AbilitiesView : MonoBehaviour
{
    public Transform Content;
    public GameObject Prefab;

    private readonly List<GameObject> items = new();

    public void Bind(CharacterViewModel vm)
    {
        for (int i = Content.childCount - 1; i >= 0; i--)
            Destroy(Content.GetChild(i).gameObject);

        items.Clear();

        foreach (var ability in vm.Abilities)
        {
            var go = Instantiate(Prefab, Content);
            items.Add(go);

            var view = go.GetComponent<AbilityItemView>();
            view.Bind(ability, vm);
        }
    }
}