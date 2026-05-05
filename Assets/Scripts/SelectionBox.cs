using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SelectionBox : VisualElement
{
    private void OnEnable()
    {
        //doc = GetComponent<UIDocument>();
        //rootve = doc.rootVisualElement;

        //selectionBoxes = rootve.Query<VisualElement>(name: "selectionBoxTemplate").ToList();


    }

    private Label m_Label;

    // Propiedad p¨²blica para leer/escribir el texto.
    // Esto se comunica directamente con el componente Label de la UI.
    [UxmlAttribute]
    public string ButtonText
    {
        get => m_Label?.text;
        set
        {
            if (m_Label != null)
            {
                m_Label.text = value;
            }
        }
    }

    public SelectionBox()
    {
        var visualTree = Resources.Load<VisualTreeAsset>("SelectionBoxTemplate");

        if (visualTree != null)
        {
            visualTree.CloneTree(this);
        }
        else
        {
            Debug.LogError("No se pudo cargar selectionBoxTemplate.uxml. Aseg¨²rate de que est¨¦ en una carpeta Resources.");
        }

        m_Label = this.Q<Label>("Label");
    }
}
