using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class InvokablePanelcontroller_Pop : InvokablePanelController<GUI_LerpMethods_Scale>
{
    public override void PanelSetup()
    {
        for (int i = 0; i < panelLerpScripts.Length; i++)
        {
            panelLerpScripts[i].RescaleDirect(finalScale: Vector3.zero,
                                              finalValueOperations:null);
        }
        base.PanelSetup();
    }


    public override void PlacePanels(Action panelLoadAction, params Action[] extraLoadActions)
    {
        base.PlacePanels(panelLoadAction);

        for (int i = 0; i < panelLerpScripts.Length; i++)
        {
            if (panelLerpScripts.Length > 1 && i != panelLerpScripts.Length - 1) panelLerpScripts[i].Rescale(customInitialValue:null,
                                                                                                             secondaryInterpolation:null,
                                                                                                             finalScale: Vector3.one, 
                                                                                                             lerpSpeedModifier: 1.2f); // there is with SWITC Hstatement a doubling of the same command (which is rescale )
            
            extraLoadActions = (MainPanel is IAanimatedPanelController animatedPanelController)
                ? extraLoadActions.Append(() => 
                {
                    MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Active);
                    animatedPanelController.DisplayContainers();
                }).ToArray()
                : extraLoadActions.Append(() => MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Active)).ToArray();
             
            //var appendedExtraLoadActions = AppendExtraLoadActions(extraLoadActions);


            if (i == panelLerpScripts.Length - 1) //&& MainPanel is ScrollablePanel scrollablePanel)

                switch (MainPanel)
                {
                    case ScrollablePanel scrollablePanel:
                        panelLerpScripts[i].Rescale(customInitialValue:null,
                                                    secondaryInterpolation: null,
                                                    finalScale: Vector3.one, 
                                                    lerpSpeedModifier: 1.2f, 
                                                    followingAction_IN: () => StartCoroutine(scrollablePanel.ExtraLoadActionsExecutionRoutine(extraLoadActions)));
                        break;

                    default:
                        panelLerpScripts[i].Rescale(customInitialValue: null,
                                                    secondaryInterpolation: null,
                                                    finalScale: Vector3.one, 
                                                    lerpSpeedModifier: 1.2f, 
                                                    followingAction_IN: extraLoadActions[0]);
                        break;
                }
        }
    }

    public override async Task DisplacePanels(bool isInterpolated, Action unloadAction)
    {
        unloadAction?.Invoke();

        await CheckInterfacesOnDisplace();

        if (isInterpolated)
        {
            for (int i = 0; i < panelLerpScripts.Length; i++)
            {

                if (i == panelLerpScripts.Length - 1)
                {
                    Action followingAction = () => ActivateDisableCountDown();
                    followingAction += () => MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Inactive);
                    

                    panelLerpScripts[i].Rescale(customInitialValue:null,
                                                secondaryInterpolation: null,
                                                finalScale: Vector3.zero, 
                                                lerpSpeedModifier: 1.2f, 
                                                followingAction_IN: followingAction);
                }
                else
                {
                    panelLerpScripts[i].Rescale(customInitialValue:null,
                                                secondaryInterpolation: null,
                                                finalScale: Vector3.zero, 
                                                lerpSpeedModifier: 1.2f);
                }

            }
        }
        else
        {
            for (int i = 0; i < panelLerpScripts.Length; i++)
            {
                panelLerpScripts[i].RescaleDirect(finalScale: Vector3.zero, 
                                                  finalValueOperations:null);

                if (i == panelLerpScripts.Length - 1)
                {
                    MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Inactive);
                }
            }
            ActivateDisableCountDown();
        }
    }

}
