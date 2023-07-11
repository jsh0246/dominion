using System.Collections.Generic;
using Unity.VisualScripting;

public class SelectionManager
{
    private static SelectionManager _instance;
    
    public static SelectionManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new SelectionManager();
            }

            return _instance;
        }

        private set
        {
            _instance = value;
        }
    }

    public HashSet<SelectableUnit> SelectedUnits = new HashSet<SelectableUnit>();
    public List<SelectableUnit> AvailableUnits = new List<SelectableUnit>();

    private SelectionManager() { }

    public void Select(SelectableUnit Unit)
    {
        SelectedUnits.Add(Unit);
        Unit.Onselected();
    }

    public void Deselect(SelectableUnit Unit)
    {
        Unit.OnDeselected();
        SelectedUnits.Remove(Unit);
    }

    public void DeselectAll()
    {
        foreach(SelectableUnit Unit in SelectedUnits)
        {
            Unit.OnDeselected();
        }

        SelectedUnits.Clear();
    }

    public bool IsSelected(SelectableUnit Unit)
    {
        return SelectedUnits.Contains(Unit);
    }
}
