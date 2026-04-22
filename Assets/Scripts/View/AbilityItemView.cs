using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class AbilityItemView : MonoBehaviour,
    IDropHandler,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    [Header("UI")]
    public Image Background;
    public Image Icon;
    public TextMeshProUGUI Name;

    [Header("Outline")]
    public Outline Outline;

    [Header("Модификатор в способности")]
    public GameObject AssignedModRoot;
    public Image ModBackground;
    public Image ModIcon;

    [System.Serializable]
    public class TypeVisual
    {
        public ModificationType Type;
        public Sprite Background;
        public Sprite Icon;
        public Color BackgroundColor = Color.white;
    }

    public List<TypeVisual> TypeVisuals;

    private AbilityViewModel vm;

    void Awake()
    {
        HoverContext.OnHoverChanged += UpdateHighlight;
    }

    void OnDestroy()
    {
        HoverContext.OnHoverChanged -= UpdateHighlight;
    }

    public void Bind(AbilityViewModel vm)
    {
        this.vm = vm;

        Name.text = vm.Model.Name;
        Icon.sprite = vm.Model.Icon;

        UpdateAssignedMod();
        UpdateHighlight();
    }

    void UpdateAssignedMod()
    {
        var mod = vm.Model.AssignedModification;

        if (mod == null)
        {
            AssignedModRoot.SetActive(false);
            return;
        }

        AssignedModRoot.SetActive(true);

        var visual = TypeVisuals.Find(v => v.Type == mod.Type);

        if (visual != null)
        {
            ModBackground.sprite = visual.Background;
            ModBackground.color = visual.BackgroundColor;
            ModIcon.sprite = visual.Icon;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (DragContext.DraggedMod == null)
            return;

        var mod = DragContext.DraggedMod.Model;

        if (!vm.Model.SupportedTypes.Contains(mod.Type))
            return;

        var command = new AssignModificationCommand(mod, vm.Model);
        command.Execute();

        DragContext.WasDropped = true;

        FindObjectOfType<UIController>().Refresh();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        HoverContext.SetAbility(vm);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HoverContext.Clear();
    }

    void UpdateHighlight()
    {
        if (vm == null) return;

        if (Outline != null)
            Outline.enabled = false;

        if (HoverContext.HoveredMod != null)
        {
            bool compatible =
                vm.Model.SupportedTypes.Contains(HoverContext.HoveredMod.Model.Type);

            if (Outline != null)
            {
                Outline.enabled = true;
                Outline.effectColor = compatible ? Color.green : Color.red;
            }
            return;
        }

        if (HoverContext.HoveredAbility != null)
        {
            var hovered = HoverContext.HoveredAbility.Model;

            if (hovered.AssignedModification != null)
            {
                var modType = hovered.AssignedModification.Type;

                if (hovered == vm.Model)
                {
                    if (Outline != null)
                    {
                        Outline.enabled = true;
                        Outline.effectColor = Color.white;
                    }
                    return;
                }

                bool compatible = vm.Model.SupportedTypes.Contains(modType);

                if (compatible && Outline != null)
                {
                    Outline.enabled = true;
                    Outline.effectColor = Color.green;
                }

                return;
            }

            return;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (vm == null || vm.Model == null)
            return;

        if (vm.Model.AssignedModification == null)
            return;

        var command = new RemoveModificationCommand(vm.Model);
        command.Execute();

        FindObjectOfType<UIController>().Refresh();
    }
}