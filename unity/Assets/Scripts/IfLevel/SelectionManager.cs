using UnityEngine;

public enum IfObjectId
{
    None = 0,
    Cube = 1,
    Sphere = 2,
    Cylinder = 3,
}

public class SelectionManager : MonoBehaviour
{
    [SerializeField] private SelectableObject initialSelection;

    private SelectableObject currentSelection;

    public SelectableObject CurrentSelection => currentSelection;
    public IfObjectId CurrentSelectedId => currentSelection != null ? currentSelection.ObjectId : IfObjectId.None;
    public bool HasSelection => currentSelection != null;

    private void Start()
    {
        if (initialSelection != null)
        {
            SetSelection(initialSelection);
        }
    }

    public void SetSelection(SelectableObject newSelection)
    {
        if (newSelection == currentSelection)
        {
            currentSelection?.SetSelected(true);
            return;
        }

        if (currentSelection != null)
        {
            currentSelection.SetSelected(false);
        }

        currentSelection = newSelection;

        if (currentSelection != null)
        {
            currentSelection.SetSelected(true);
        }
    }

    public IfObjectId GetCurrentSelectedId()
    {
        return CurrentSelectedId;
    }
}
