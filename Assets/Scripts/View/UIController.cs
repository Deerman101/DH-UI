using UnityEngine;

public class UIController : MonoBehaviour // Ну вроде теперь в целом похоже на MVVM \OwO/
{
    public PartyView partyView;
    public CharacterInfoView infoView;
    public AbilitiesView abilitiesView;
    public ModificationsView modsView;

    private CharacterViewModel current;
    private CharacterConfig currentConfig;

    void Start()
    {
        partyView.OnCharacterSelected += OnCharacterSelected;
        partyView.Init();
    }

    void OnCharacterSelected(CharacterViewModel vm, CharacterConfig config)
    {
        if (current != null)
            current.OnDataChanged -= BindAll;

        current = vm;
        currentConfig = config;

        current.OnDataChanged += BindAll;

        BindAll();
    }

    void BindAll()
    {
        infoView.Bind(current, currentConfig);
        abilitiesView.Bind(current);
        modsView.Bind(current);
    }
}