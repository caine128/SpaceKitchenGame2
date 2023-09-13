using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class InvokablePanelController_Slide : InvokablePanelController<GUI_LerpMethods_Movement>
{
    [SerializeField] protected RectTransform[] panelInScreenAnchorsTemp;

    public override void PanelSetup()
    {
        CalculateInScreenAnchors();
        base.PanelSetup();
    }

    private void CalculateInScreenAnchors()
    {
        panelInScreenAnchors = new Vector2[panelInScreenAnchorsTemp.Length];

        for (int i = 0; i < panelInScreenAnchorsTemp.Length; i++)
        {
            panelInScreenAnchors[i] = panelInScreenAnchorsTemp[i].anchoredPosition;
            Destroy(panelInScreenAnchorsTemp[i].gameObject);
        }
    }


    public override void PlacePanels(Action panelLoadAction, params Action[] extraLoadActions)
    {
        base.PlacePanels(panelLoadAction);

        for (int i = 0; i < panelLerpScripts.Length; i++)
        {
            if(panelLerpScripts.Length > 1 && i != panelLerpScripts.Length - 1) panelLerpScripts[i].InitialCall(targetPos: panelInScreenAnchors[i]);

            extraLoadActions = (MainPanel is IAnimatedPanelController animatedPanelController)
                ? extraLoadActions.Append(() =>
                {
                    MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Active);
                    animatedPanelController.DisplayContainers();
                }).ToArray()
                : extraLoadActions.Append(() => MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Active)).ToArray();
            //extraLoadActions = extraLoadActions.Append(() => MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Active)).ToArray();
            //extraLoadActions = AppendExtraLoadActions(extraLoadActions);

            if (i == panelLerpScripts.Length - 1) // && MainPanel is ScrollablePanel scrollablePanel)
            {
                switch (MainPanel)
                {
                    case ScrollablePanel scrollablePanel:
                        panelLerpScripts[i].InitialCall(targetPos: panelInScreenAnchors[i], lerpSpeedModifier: 1.2f, followingAction: () => StartCoroutine(scrollablePanel.ExtraLoadActionsExecutionRoutine(extraLoadActions)));
                        break;
                    default:
                        panelLerpScripts[i].InitialCall(targetPos: panelInScreenAnchors[i], lerpSpeedModifier: 1.2f, followingAction: extraLoadActions[0]);
                        break;
                }
            }
        }
    }





    public override void DisplacePanels(bool isInterpolated, Action unloadAction)
    {
        unloadAction?.Invoke();

        CheckInterfacesOnDisplace();

        if (isInterpolated)
        {
            for (int i = 0; i < panelLerpScripts.Length; i++)
            {
                if (i == panelLerpScripts.Length - 1)
                {
                    Action followingAction = () => ActivateDisableCountDown();
                    followingAction += () => MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Inactive);

                    panelLerpScripts[i].FinalCall(followingAction: followingAction);
                }
                else
                {
                    panelLerpScripts[i].FinalCall();
                }

            }
        }
        else
        {
            for (int i = 0; i < panelLerpScripts.Length; i++)
            {
                panelLerpScripts[i].FinalCallDirect();

                if (i == panelLerpScripts.Length - 1)
                {
                    MainPanel.FireOnPanelMovedEvent(ScrollablePanel.PanelState.Inactive);
                }
            }
            ActivateDisableCountDown();
        }
    }

}
