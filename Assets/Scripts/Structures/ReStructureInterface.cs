
using UnityEngine; 
using UnityEngine.UI;

public class ReStructureInterface : MonoBehaviour
{
    public static ReStructureInterface Instance { get; private set; }
    public StructureBlock Selection { get; private set; }

    [SerializeField] private GameObject content;
    [SerializeField] private Text selectionText;
    [SerializeField] private InputField structName;
    [SerializeField] private InputField length;
    [SerializeField] private InputField height;
    [SerializeField] private UnityEngine.UI.Button set;
     

    public void Awake()
    {
        Initialize();
        RefreshSelectionText();
    }

    public void SetSelection(StructureBlock sb)
    {
        Selection = sb;
        structName.text = sb.structName;
        length.text = sb.L.ToString();
        height.text = sb.H.ToString();
    }

    private void RefreshSelectionText()
    {
        if (Selection)
        {
            selectionText.text = string.Format(INLocalization.Instance.GetText("Structure_Selected"),
                Selection.GetHashCode()
                    .ToString(),
                Selection.CoordX.ToString(),
                Selection.CoordY.ToString());
        }
        else
        {
            selectionText.text = INLocalization.Instance.GetText("Structure_Unselected");
        }
    }

    private void SetStructureBlock()
    {
        if (!Selection)
        {
            return;
        }

        Selection.structName = structName.text.Trim();
        if (int.TryParse(length.text, out int l))
        {
            if (int.TryParse(height.text, out int h))
            {
                Selection.SetSize(l,h );
                return;
            }
        }//maybe we need to tell them what's wrong
    }

    private void Initialize()
    {
        Instance = this;
        Selection = null;
        set.onClick.AddListener(SetStructureBlock);
    }
}