using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public abstract class InvokablePanelController : MonoBehaviour
{
    [SerializeField] protected Panel_Base[] panels;             // MAYBE THIS CAN BE SIMPLY "THIS" INSTAD OF ASSIGNING FROM INSPECTPR !!!

    protected Vector2[] panelInScreenAnchors;


    public Panel_Base MainPanel
    {
        get
        {
            if(panels.Length > 0)
            {
                return panels[0];
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }


    private IEnumerator runningCoroutine_Countdown = null;

    public virtual void PanelSetup()
    {
        ActivateDisableCountDown(seconds: 1);
    }


    public virtual void PlacePanels(Action loadAction, params Action[] extraLoadActions)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            AbortDisableCountDown();

            if (panels[i] is IRefreshablePanel refreshablePanel)
            {
                refreshablePanel.RefreshPanel();
            }

            if (panels[i] is IAanimatedPanelController_Cancellable modalPanel)
            {
                modalPanel.IsAnimating = true;
            }

            if (panels[i] is AscensionLadderPanel_Manager ascensionLadderPanel_Manager)
            {
#if UNITY_EDITOR
                if (loadAction?.GetInvocationList().Length > 0) Debug.LogError("loadaction already exists for ascensionLaadderpanelManager");
#endif
                loadAction = ()=>ascensionLadderPanel_Manager.SortByEquipmentType(
                                                                mainType_IN: AscensionLadderPanel_Manager.activeSelection_MainType, 
                                                                buttonIN: null);
            }

            panels[i].FireOnPanelMovedEvent(ScrollablePanel.PanelState.Activating);

            if (i == 0)
            {
                loadAction?.Invoke();
            }          
           
        }
    }


    public abstract Task DisplacePanels(bool isInterpolated, Action unloadAction);

    protected async Task CheckInterfacesOnDisplace ()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] is ITaskHandlerPanel taskHandlerPanel)
            {
                taskHandlerPanel.HandleTask(false);
            }
            if (panels[i] is IReassignablePanel reassignablePanel)
            {
                reassignablePanel.AssignToOriginalPanelLayout();
            }
            if(panels[i] is IQuickUnloadable quickUnloadablePanel)
            {
                quickUnloadablePanel.QuickUnload();
            }
            if (panels[i] is IAanimatedPanelController aanimatedPanelController)
            {
                aanimatedPanelController.HideContainers();

                if (aanimatedPanelController is BuildOptionsPanel_Manager bpm)
                {
                    Debug.Log("awaiting the Task");
                    Debug.Log(bpm.Tcs.Task.Result);
                    await bpm.Tcs.Task;
                }

                //aanimatedPanelController.HideContainers(); ///

              
            }
            panels[i].FireOnPanelMovedEvent(ScrollablePanel.PanelState.Deactivating);
        }
    }

    /*protected Action[] AppendExtraLoadActions(Action[] incomingExtraLoadACtions)
    {
        incomingExtraLoadACtions.Append(() => MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Active));
        
        if(MainPanel is IAanimatedPanelController animatedPanelController)
        {
            incomingExtraLoadACtions.Append(() => animatedPanelController.DisplayContainers());
        }

        return incomingExtraLoadACtions;
    }*/


    protected void ActivateDisableCountDown(int seconds = TimeTickSystem.PERIOD_DISABLEPANEL_DEFAULT)
    {
        if (PanelManager.SelectedPanels.Contains(this))
        {
            return;
        }
        else
        {
            if (runningCoroutine_Countdown != null) StopCoroutine(runningCoroutine_Countdown);

            runningCoroutine_Countdown = CountdownRoutine(seconds);
            StartCoroutine(runningCoroutine_Countdown);
        }    
    }

    private void AbortDisableCountDown()                   /// TODO : PUT THIS ON ENABLING TO STOP UNLOAD COUNTDOWN
    {
        if (runningCoroutine_Countdown != null)
        {
            StopCoroutine(runningCoroutine_Countdown);
            runningCoroutine_Countdown = null;
        }
    }

    private IEnumerator CountdownRoutine(int seconds)
    {
        int counter = seconds;
        while (counter > 0)
        {
            yield return TimeTickSystem.WaitForSeconds_One;
            counter--;
        }

        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i] is IDeallocatable deallocatablePanel)
            {
                deallocatablePanel.UnloadAndDeallocate();
            }
        }
      
        runningCoroutine_Countdown = null;
        this.gameObject.SetActive(false);
    }


}



public abstract class InvokablePanelController<T_GUILerpMethod> : InvokablePanelController
    where T_GUILerpMethod: GUI_LerpMethods
{
    [SerializeField] protected T_GUILerpMethod[] panelLerpScripts;

}
