using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmationPopupPanel : PopupPanel_Multi_SNG<ContentDisplayFrame>, ITaskHandlerPanel
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    private string defaultPopupHeader = "Dismantle Items";

    public TaskCompletionSource<bool> TCS { get { return tcs; } }
    private TaskCompletionSource<bool> tcs = null;

    //[SerializeField] private ContentDisplayPopup_Generic[] contentDisplays_Sub_Generic;
    //private int amountOfNecessarySubContainers;

    protected sealed override void Start()
    {
        co = new IEnumerator[2];
        base.Start();
    }
    protected override string DefaultPopupHeader()
    {
        return defaultPopupHeader;
    }

    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        var confirmation_LoadData = (PopupPanel_Confirmation_LoadData)panelLoadData;

        tcs = panelLoadData.tcs;
        //amountOfNecessarySubContainers = 0;
        base.LoadPanel(panelLoadData);

        /// Conditional Load behavior according to itss fields availibility
        if (!string.IsNullOrEmpty(confirmation_LoadData.extraDescription))
        {
            descriptionText.text = confirmation_LoadData.extraDescription;
        }
        if (confirmation_LoadData.bluePrintsToLoad is not null)
        {
            EnableAndLoadMainContentDisplays(confirmation_LoadData.bluePrintsToLoad);
        }

        popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.Reject);
        popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.Confirm);
    }

    /*public sealed override void DisplayContainers()
    {
        base.DisplayContainers();
        /*contentDisplays_Sub_Generic.SortContainers(customInitialValues:null,
                                                   secondaryInterpolations: null,
                                                   amountToSort_IN: amountOfNecessarySubContainers, 
                                                   enumeratorIndex: 0,
                                                   parentPanel_IN: this,
                                                   lerpSpeedModifiers: null);
    }*/
  
    public void HandleTask(bool isTrue)
    {
        if (tcs != null && tcs.TrySetResult(isTrue))
        {
            tcs = null;
        }
    }

    public void Confirm()  /// HANDLE TASK IS COMMENTED OUT DONT FORGET !!!!
    {
        HandleTask(true);
    }

    public void Reject()
    {
        HandleTask(false);
    }

    /*public sealed override void UnloadAndDeallocate()
    {
        base.UnloadAndDeallocate();
        //GUI_CentralPlacement.DeactivateUnusedContainers(0, contentDisplays_Sub_Generic);
    }*/


}
