using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Container<T> : MonoBehaviour
{
    public T bluePrint { get; protected set; }
    [SerializeField] protected Image containerImage;
    [SerializeField] protected AdressableImage mainImageContainer;
    public abstract RectTransform rt { get; }
    [SerializeField] private  GUI_LerpMethods_Scale gUI_LerpMethods_Scale;
    [SerializeField] private GUI_TintScale gUI_TintScale;
    public abstract void LoadContainer(T newRecipe_IN);
       
    public abstract void UnloadContainer();

    public void ScaleWithRoutine(bool isVisible)
    {
        Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
        gUI_LerpMethods_Scale.Rescale(customInitialValue:null,
                                      secondaryInterpolation :null,
                                      finalScale: scale);
    }

    public void ScaleDirect(bool isVisible)
    {
        Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
        gUI_LerpMethods_Scale.RescaleDirect(finalScale: scale,
                                            finalValueOperations: null);
    }

    public abstract void MatchContainerDynamicInfo();


    public void Tintsize()
    {
        gUI_TintScale.TintSize();
    }

    public void SetState_ImageRaycast(ScrollablePanel.PanelState panelState)
    {
        switch (panelState)
        {

            case ScrollablePanel.PanelState.Active:
                if (containerImage.raycastTarget != true) containerImage.raycastTarget = true;
                break;
            case ScrollablePanel.PanelState.Deactivating:
                if (containerImage.raycastTarget != false) containerImage.raycastTarget = false;
                break;
            case ScrollablePanel.PanelState.Activating:
            case ScrollablePanel.PanelState.Inactive:
            default:
                break;
        }
    }
}
