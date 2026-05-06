using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class General : MonoBehaviour
{

    SelectionBox nameEntry;
    SelectionBox options;

    private SelectionBox currentSelected;
    private List<SelectionBox> allTabs;
    private VisualElement contentZone;
    private VisualElement currentActiveContent;

    UIDocument doc;
    VisualElement root;

    private void OnEnable()
    {
        currentSelected = null;
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        nameEntry = root.Q<SelectionBox>();

        nameEntry.RegisterCallback<MouseDownEvent>(evt =>
        {
            Debug.Log("Clicked on name entry button");
        });

        contentZone = root.Q<VisualElement>("Content");
        allTabs = root.Query<SelectionBox>().ToList();

        foreach (var tab in allTabs)
        {
            tab.AddToClassList("is-interactable");

            tab.RegisterCallback<PointerDownEvent>(evt => OnTabClicked(tab));
        }
    }

    private void OnTabClicked(SelectionBox clickedTab)
    {
        if (currentSelected != null) return;
        currentSelected = clickedTab;

        foreach (var tab in allTabs)
        {
            tab.RemoveFromClassList("is-interactable");
            if (tab == clickedTab)
            {
                tab.AddToClassList("is-selected");
            }
        }

        ShowContent(clickedTab.name);
    }

    private void Update()
    {
        if (currentSelected != null && Input.GetKeyDown(KeyCode.Escape))
        {
            ExitCurrentTab();
        }
    }

    private void ExitCurrentTab()
    {
        foreach (var tab in allTabs)
        {
            tab.AddToClassList("is-interactable");
            tab.RemoveFromClassList("is-selected");
        }

        currentSelected = null;

        if (currentActiveContent != null)
        {
            currentActiveContent.style.display = DisplayStyle.None;
            currentActiveContent = null;
        }
    }

    private void ShowContent(string tabName)
    {
        string contentNameToFind = "Content-" + tabName;

        currentActiveContent = root.Q<VisualElement>(contentNameToFind);

        if (currentActiveContent != null)
        {
            currentActiveContent.style.display = DisplayStyle.Flex;
        }
        else
        {
            Debug.LogWarning($"No se encontró el contenido: {contentNameToFind}. Asegúrate de nombrarlo correctamente en UI Builder.");
        }
    }
}
