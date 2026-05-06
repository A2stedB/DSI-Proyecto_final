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
    public string optionDropdown = "8x";
}
public class General : MonoBehaviour
{

    //SelectionBox nameEntry;

    private InputAction escape;
    private SelectionBox currentSelected;
    private List<SelectionBox> allTabs;
    private VisualElement currentActiveContent;

    private TextField nameField;
    private Label charCounterLabel;

    private Label nameLabel;
    private Toggle toggle;
    private Slider slider;
    private DropdownField dropdown;

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

        allTabs = root.Query<SelectionBox>().ToList();

        foreach (var tab in allTabs)
        {
            tab.AddToClassList("is-interactable");

            tab.RegisterCallback<PointerDownEvent>(evt => OnTabClicked(tab));
        }

        VisualElement nameEntry = root.Q<VisualElement>(name: "Content-NameEntry");
        charCounterLabel = nameEntry.Q<Label>(name: "CharCounter");

        nameField = nameEntry.Q<TextField>();
        VisualElement options = root.Q<VisualElement>(name: "Content-Options");
        toggle = options.Q<Toggle>();
        slider = options.Q<Slider>();
        dropdown = options.Q<DropdownField>();
        nameLabel = root.Q<Label>(name:"Name");
        LoadSavedSettings();

        UpdateCharacterCounter(currentData.playerName.Length);

        nameField.RegisterCallback<ChangeEvent<string>>(NameChange);
        toggle.RegisterCallback<ChangeEvent<bool>>(ToggleChange);
        slider.RegisterCallback<ChangeEvent<float>>(SliderChange);
        dropdown.RegisterCallback<ChangeEvent<string>>(DropdownChange);


    }

    private void NameChange(ChangeEvent<string> evt)
    {
        nameLabel.text = evt.newValue;
        UpdateCharacterCounter(evt.newValue.Length);
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

    private void DropdownChange(ChangeEvent<string> evt)
    {
        currentData.optionDropdown = evt.newValue;
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
        }
        else
        {
            SaveToJson();
        }

        if (nameLabel != null) nameLabel.text = currentData.playerName;
        if (toggle != null) toggle.SetValueWithoutNotify(currentData.optionToggle);
        if (slider != null) slider.SetValueWithoutNotify(currentData.optionSlider);
        if (dropdown != null && !string.IsNullOrEmpty(currentData.optionDropdown))
        {
            dropdown.SetValueWithoutNotify(currentData.optionDropdown);
        }
    }
    private void SaveToJson()
    {
        string json = JsonUtility.ToJson(currentData, true);
        File.WriteAllText(saveFilePath, json);
    }

    private void UpdateCharacterCounter(int currentLength)
    {
        if (charCounterLabel != null)
        {
            charCounterLabel.text = $"{currentLength}/16";
        }
    }
}
