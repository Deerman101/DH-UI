using UnityEngine;

public class UIController : MonoBehaviour // Вот тут MVVM окончательно и заканчивается... чистый MVVM не получился, увы \O_O/
{
    public PartyView partyView;
    public CharacterInfoView infoView;
    public AbilitiesView abilitiesView;
    public ModificationsView modsView;

    private CharacterConfig currentConfig;

    [SerializeField] private CharacterViewModel current;

    void Start()
    {
        //Debug.Log("UIController запущен");

        partyView.OnCharacterSelected += OnCharacterSelected;

        partyView.Init();
    }
    void OnCharacterSelected(CharacterViewModel vm, CharacterConfig config)
    {
        current = vm;
        currentConfig = config;

        infoView.Bind(vm, config);
        abilitiesView.Bind(vm);
        modsView.Bind(vm);
    }

    /// <summary>
    /// Обновит UI для текуего персонажа
    /// </summary>
    public void Refresh()
    {
        if (current == null)
            return;

        //infoView.Bind(current, null); // можно оставить без configa если уже есть данные внутри VM (если реализовать рандом или захардкодить)

        infoView.Bind(current, currentConfig);
        abilitiesView.Bind(current);
        modsView.Bind(current);
    }
}