using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class PartyView : MonoBehaviour
{
    [Header("Кнопки персонажей")]
    public Button[] Buttons;

    [Header("Рамки выделения")]
    public GameObject[] SelectionFrames;

    private List<CharacterViewModel> characters = new();

    public Action<CharacterViewModel, CharacterConfig> OnCharacterSelected;

    public void Init()
    {
        characters.Clear();

        for (int i = 0; i < Buttons.Length; i++)
        {
            int index = i;

            var config = Buttons[i].GetComponent<CharacterConfig>();
            var model = CreateModel(config);
            var vm = new CharacterViewModel(model);

            characters.Add(vm);

            Buttons[i].onClick.AddListener(() =>
            {
                UpdateSelection(index);
                OnCharacterSelected?.Invoke(characters[index], config);
            });
        }

        if (characters.Count > 0)
        {
            UpdateSelection(0);

            var firstConfig = Buttons[0].GetComponent<CharacterConfig>();
            OnCharacterSelected?.Invoke(characters[0], firstConfig);
        }
    }

    void UpdateSelection(int selectedIndex)
    {
        for (int i = 0; i < SelectionFrames.Length; i++)
            if (SelectionFrames[i] != null)
                SelectionFrames[i].SetActive(i == selectedIndex);
    }

    CharacterModel CreateModel(CharacterConfig config)
    {
        var model = new CharacterModel
        {
            Name = config.CharacterName,
            HP = config.HP,
            Armor = config.Armor
        };

        foreach (var a in config.Abilities)
        {
            model.Abilities.Add(new AbilityModel
            {
                Name = a.Name,
                Icon = a.Icon,
                SupportedTypes = a.SupportedTypes
            });
        }

        foreach (var m in config.Modifications)
        {
            model.Modifications.Add(new ModificationModel
            {
                Name = m.Name,
                Type = m.Type
            });
        }

        return model;
    }
}