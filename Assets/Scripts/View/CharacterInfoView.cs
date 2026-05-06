using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Отображает всю инфу о персонаже
/// </summary>
public class CharacterInfoView : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI HPText;
    public TextMeshProUGUI ArmorText;
    public Image Portrait;

    public void Bind(CharacterViewModel vm, CharacterConfig config)
    {
        NameText.text = vm.Name;
        HPText.text = $"{vm.HP}/{vm.HP}";
        ArmorText.text = $"{vm.Armor}/{vm.Armor}";

        Portrait.sprite = config.Portrait;
    }
}