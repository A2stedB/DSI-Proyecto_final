using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using System.IO;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

[System.Serializable]
public class SaveData
{
    public string playerName = "";
    public bool optionToggle = false;
    public float optionSlider = 0f;
}
public class General : MonoBehaviour
{

    //SelectionBox nameEntry;

    private InputAction escape;
    private SelectionBox currentSelected;
    private List<SelectionBox> allTabs;
    private VisualElement currentActiveContent;

    private TextField nameField;

    private Label nameLabel;
    private Toggle toggle;
    private Slider slider;

    static private string saveFilePath;
    private SaveData currentData = new SaveData();

    UIDocument doc;
    VisualElement root;

    private void OnEnable()
    {
        saveFilePath = Application.persistentDataPath + "/settings.json";
        escape = GetComponent<PlayerInput>().currentActionMap["Cancel"];

        escape.performed += context =>
        {
            if (currentSelected != null)
            {
                ExitCurrentTab();
            }
        };


        currentSelected = null;
        doc = GetComponent<UIDocument>();
        root = doc.rootVisualElement;

        //nameEntry = root.Q<SelectionBox>();

        //nameEntry.RegisterCallback<MouseDownEvent>(evt =>
        //{
        //    Debug.Log("Clicked on name entry button");
        //});

        allTabs = root.Query<SelectionBox>().ToList();

        foreach (var tab in allTabs)
        {
            tab.AddToClassList("is-interactable");

            tab.RegisterCallback<PointerDownEvent>(evt => OnTabClicked(tab));
        }

        VisualElement nameEntry = root.Q<VisualElement>(name: "Content-NameEntry");

        nameField = nameEntry.Q<TextField>();
        VisualElement options = root.Q<VisualElement>(name: "Content-Options");
        toggle = options.Q<Toggle>();
        slider = options.Q<Slider>();
        nameLabel = root.Q<Label>(name:"Name");
        LoadSavedSettings();

        nameField.RegisterCallback<ChangeEvent<string>>(NameChange);
        toggle.RegisterCallback<ChangeEvent<bool>>(ToggleChange);
        slider.RegisterCallback<ChangeEvent<float>>(SliderChange);

    }

    private void NameChange(ChangeEvent<string> evt)
    {
        nameLabel.text = evt.newValue;
        currentData.playerName = nameLabel.text;
        SaveToJson();
    }

    private void ToggleChange(ChangeEvent<bool> evt)
    {
        currentData.optionToggle = evt.newValue;
        SaveToJson();
    }

    private void SliderChange(ChangeEvent<float> evt)
    {
        currentData.optionSlider = evt.newValue;
        SaveToJson();
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

        if(tabName == "NameEntry")
        {
            nameField.SetValueWithoutNotify(nameLabel.text);
        }
    }

    private void LoadSavedSettings()
    {
        //if (PlayerPrefs.HasKey("PlayerName"))
        //{
        //    name.text = PlayerPrefs.GetString("PlayerName");
        //}
        //if (PlayerPrefs.HasKey("OptionToggle"))
        //{
        //    bool isToggleOn = PlayerPrefs.GetInt("OptionToggle") == 1;
        //    toggle.SetValueWithoutNotify(isToggleOn);
        //}
        //if (PlayerPrefs.HasKey("OptionSlider"))
        //{
        //    slider.SetValueWithoutNotify(PlayerPrefs.GetFloat("OptionSlider"));
        //}

        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            currentData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Datos cargados correctamente desde: " + saveFilePath);
        }
        else
        {
            //Debug.Log("No se encontró archivo de guardado, usando valores por defecto.");
            SaveToJson();
        }

        if (nameLabel != null) nameLabel.text = currentData.playerName;
        if (toggle != null) toggle.SetValueWithoutNotify(currentData.optionToggle);
        if (slider != null) slider.SetValueWithoutNotify(currentData.optionSlider);
    }
    private void SaveToJson()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(saveFilePath, json);
    }
}
