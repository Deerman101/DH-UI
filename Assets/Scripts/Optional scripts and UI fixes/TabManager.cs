using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// Универсальный Tab Manager
/// </summary>
public class TabManager : MonoBehaviour
{
    [Serializable]
    public class Tab
    {
        [Header("UI")]
        public Button Button;
        public TextMeshProUGUI Text;
        public GameObject Panel;

        [Header("Селектер")]
        public GameObject SelectedObject;

        [Header("Image кнопки таба")]
        public Image BackgroundImage;
    }

    [Header("Табы")]
    public List<Tab> Tabs = new();

    [Header("Цвета текста")]
    public Color SelectedColor;
    public Color DefaultColor;

    [Header("Спрайты для кнопок")]
    public Sprite ActiveSprite;
    public Sprite InactiveSprite;

    private int currentIndex/* = 0*/;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        for (int i = 0; i < Tabs.Count; i++)
        {
            int index = i;

            if (Tabs[i].Button != null)
            {
                Tabs[i].Button.onClick.RemoveAllListeners();

                Tabs[i].Button.onClick.AddListener(() =>
                {
                    SelectTab(index);
                });
            }
        }

        if (Tabs.Count > 0)
            SelectTab(1);
    }

    public void SelectTab(int index)
    {
        currentIndex = index;

        HashSet<GameObject> allPanels = new HashSet<GameObject>();

        foreach (var tab in Tabs)
            if (tab.Panel != null)
                allPanels.Add(tab.Panel);

        foreach (var panel in allPanels)
            panel.SetActive(false);

        for (int i = 0; i < Tabs.Count; i++)
        {
            bool isSelected = i == index;
            var tab = Tabs[i];

            if (isSelected && tab.Panel != null)
                tab.Panel.SetActive(true);

            if (tab.Text != null)
                tab.Text.color = isSelected ? SelectedColor : DefaultColor;

            if (tab.SelectedObject != null)
                tab.SelectedObject.SetActive(isSelected);

            if (tab.BackgroundImage != null)
                tab.BackgroundImage.sprite = isSelected ? ActiveSprite : InactiveSprite;
        }
    }
}