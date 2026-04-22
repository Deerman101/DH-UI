using UnityEngine;
using System.Collections.Generic;

public class ModificationsView : MonoBehaviour
{
    public Transform Content;
    public GameObject Prefab;

    private List<GameObject> items = new();

    public void Bind(CharacterViewModel vm)
    {
        if (vm == null) return;

        for (int i = Content.childCount - 1; i >= 0; i--)
            Destroy(Content.GetChild(i).gameObject);

        items.Clear();

        foreach (var mod in vm.Mods)
        {
            if (mod == null || mod.Model == null)
                continue;

            var go = Instantiate(Prefab, Content);

            var view = go.GetComponent<ModificationItemView>();

            if (view == null)
            {
                Debug.LogError("На префабе мода нет ModificationItemView!");
                continue;
            }

            view.Bind(mod);

            items.Add(go);
        }
    }
}