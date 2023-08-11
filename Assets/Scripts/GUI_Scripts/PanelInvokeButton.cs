
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class PanelInvokeButton : Button_Base, ISinglePanelInvokeButton  //MonoBehaviour, IPointerDownHandler, IPointerUpHandler,ISinglePanelInvokeButton    // CHANGE THE NAME TO PANELBUTTON !!
{
    [SerializeField] private UnityEngine.GameObject notificationBubble;
    public InvokablePanelController PanelToInvoke { get { return panelToInvoke; } }
    [SerializeField] private InvokablePanelController panelToInvoke;

    public virtual void Awake()
    {
        if (panelToInvoke?.MainPanel is not null && panelToInvoke?.MainPanel is Panel_Base panel)
        {
            panel.OnPanelMoved += SetState_ImageRaycast;
        }
        else Debug.LogError($"{panelToInvoke.name} panel doesnt have any mainpanel assigned yet!");
    }


    protected virtual void SetState_ImageRaycast(ScrollablePanel.PanelState panelState)
    {
        switch (panelState)
        {
            case ScrollablePanel.PanelState.Inactive:
                if (buttonImage_Adressable.raycastTarget != true) buttonImage_Adressable.raycastTarget = true;
                break;
            case ScrollablePanel.PanelState.Activating:

                if (buttonImage_Adressable.raycastTarget != false) buttonImage_Adressable.raycastTarget = false;
                break;
            case ScrollablePanel.PanelState.Deactivating:
            case ScrollablePanel.PanelState.Active:
            default:
                break;
        }
    }
}


