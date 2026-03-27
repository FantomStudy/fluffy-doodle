using UnityEngine;
using System.Collections.Generic;

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

    private readonly List<SelectableObject> selectedObjects = new List<SelectableObject>();
    private SelectableObject currentSelection;

    public SelectableObject CurrentSelection => currentSelection;
    public IfObjectId CurrentSelectedId => currentSelection != null ? currentSelection.ObjectId : IfObjectId.None;
    public bool HasSelection => selectedObjects.Count > 0;

    private void Start()
    {
        if (initialSelection != null)
        {
            SetSelection(initialSelection);
        }
    }

    public void SetSelection(SelectableObject newSelection)
    {
        if (newSelection == null)
        {
            return;
        }

        if (selectedObjects.Contains(newSelection))
        {
            currentSelection = newSelection;
            newSelection.SetSelected(true);
            return;
        }

        selectedObjects.Add(newSelection);
        currentSelection = newSelection;
        currentSelection.SetSelected(true);
    }

    public void ToggleSelection(SelectableObject selection)
    {
        if (selection == null)
        {
            return;
        }

        if (selectedObjects.Contains(selection))
        {
            RemoveSelection(selection);
            return;
        }

        SetSelection(selection);
    }

    public void RemoveSelection(SelectableObject selection)
    {
        if (selection == null)
        {
            return;
        }

        if (!selectedObjects.Remove(selection))
        {
            return;
        }

        selection.SetSelected(false);
        currentSelection = selectedObjects.Count > 0 ? selectedObjects[selectedObjects.Count - 1] : null;
    }

    public bool IsSelected(SelectableObject selection)
    {
        return selection != null && selectedObjects.Contains(selection);
    }

    public bool HasSelectedId(IfObjectId objectId)
    {
        if (objectId == IfObjectId.None)
        {
            return false;
        }

        for (int i = 0; i < selectedObjects.Count; i++)
        {
            SelectableObject selection = selectedObjects[i];
            if (selection != null && selection.ObjectId == objectId)
            {
                return true;
            }
        }

        return false;
    }

    public IfObjectId GetCurrentSelectedId()
    {
        return CurrentSelectedId;
    }
}
