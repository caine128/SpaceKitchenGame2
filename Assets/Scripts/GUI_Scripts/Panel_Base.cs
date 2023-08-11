
using UnityEngine;

public abstract class Panel_Base : MonoBehaviour
{
    public event System.Action<PanelState> OnPanelMoved;


    public enum PanelState
    {
        Inactive = 0,
        Active = 1,
        Deactivating = 2,
        Activating = 4,
    }

    public void FireOnPanelMovedEvent(PanelState panelState)
    {
        OnPanelMoved?.Invoke(panelState);
    }
}
