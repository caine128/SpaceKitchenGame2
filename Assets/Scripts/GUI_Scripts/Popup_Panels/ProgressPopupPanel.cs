using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPopupPanel : PopupPanel_Single_SNG<ProgressPopupPanel>, IBrowsablePanel<IRushable>,IVariableButtonPanel, IQuickUnloadable
{
    [SerializeField] private Image _progressBar_FG;
    [SerializeField] private TextMeshProUGUI _textField;
    [SerializeField] private TextMeshProUGUI _progressTextField;
    [SerializeField] private GUI_LerpMethods_Float progressBar_LerpMethod;

    public BrowserButton<IRushable>[] BrowserButtons { get => _browserButtons; }
    [SerializeField] BrowserButton<IRushable>[] _browserButtons;
    public int CurrentIndice { get => _currentIndice; }
    private int _currentIndice = 0;
    public List<IRushable> ListToIterate { get; private set; } = new();


    public RectTransform[] PopupButtons_RT { get; private set; }
    public Vector2[] PopupButtons_OriginalLocations { get; private set; }     
    private static string _PopupHeader;
    public void ConfigureVariableButtons()
    {
        var arrayLength = popupButtons.Length;
        PopupButtons_RT = new RectTransform[arrayLength];
        PopupButtons_OriginalLocations = new Vector2[arrayLength];

        for (int i = 0; i < arrayLength; i++)
        {
            PopupButtons_RT[i] = popupButtons[i].GetComponent<RectTransform>();
            PopupButtons_OriginalLocations[i] = PopupButtons_RT[i].anchoredPosition;
        }
    }

    protected sealed override void Start()
    {
        base.Start();
        InitialConfigBrowserButtons();
    }

    public void InitialConfigBrowserButtons()
    {
        foreach (var browserButton in _browserButtons)
        {
            browserButton.ButtonConfig(Instance);
        }
    }


    public sealed override void LoadPanel(PanelLoadData panelLoadData)
    {
        if(panelLoadData is ProgressPanelLoadData progressPanelLoadData)
        {
            if (ListToIterate.Count != 0 ) //|| onProgressTickedActions.Count != 0 || currentProgresses.Count != 0)
            {
                ListToIterate.Clear();
            }
            foreach (var rushable in progressPanelLoadData.rushableITems)
            {
                ListToIterate.Add(rushable);
            }

            _currentIndice = progressPanelLoadData.clickedObjectIndex;
            _PopupHeader = string.IsNullOrEmpty(panelLoadData.panelHeader) ? string.Empty : panelLoadData.panelHeader;
            LoadSingleItem(ListToIterate[_currentIndice]);
        }
    }

    private void LoadSingleItem(IRushable rushableBlueprint)
    {
        var variablePanelInterface = (IVariableButtonPanel)this;
        var browsablePanelInterface = (IBrowsablePanel<IRushable>)this;

        ListToIterate[_currentIndice].OnProgressTicked -= UpdateByProgress; // Unsubscribing from the previous item's delegate before deciding the new item's indice.

        _currentIndice = browsablePanelInterface.SetCurrentIndice(rushableBlueprint);
        browsablePanelInterface.SetVisibilityBrowserButtons();

        base.LoadPanel(new ProgressPanelLoadData(mainLoadInfo: ListToIterate[_currentIndice].BluePrint, 
                                                 panelHeader: DefaultPopupHeader(), tcs_IN: null, rushableItemsData: null, clickedObjectIndex: default));

        var currentProgress = ListToIterate[_currentIndice].CurrentProgress;
        _textField.text = ListToIterate[_currentIndice].BluePrint.GetName();
        progressBar_LerpMethod.ClearQueue();
        _progressBar_FG.fillAmount = currentProgress;

        if(currentProgress == 1)
        {
            variablePanelInterface.SetButtonLayout(1);
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.ProgressPanel_BackToShop);
            _progressTextField.text = "Complete";

        }
        else if(rushableBlueprint is IRushableWithEnergy productRecipe)
        {
            variablePanelInterface.SetButtonLayout(2);
            
            popupButtons[0].SetupButton(ButtonFunctionType.PopupPanel.ProgressPanel_RushByEnergy);
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.ProgressPanel_RushByGem);
            popupButtons[0].ModifyButtonValue(productRecipe.GetCurrentRushCostEnergy);
            popupButtons[1].ModifyButtonValue(productRecipe.GetCurrentRushCostGem);
            ListToIterate[_currentIndice].OnProgressTicked += UpdateByProgress;
        }
        else
        {
            variablePanelInterface.SetButtonLayout(1);

           
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.ProgressPanel_RushByGem);
            popupButtons[1].ModifyButtonValue(rushableBlueprint.GetCurrentRushCostGem);
            ListToIterate[_currentIndice].OnProgressTicked += UpdateByProgress;
        }
    }

    private void UpdateByProgress(float valueInitial, float valueFinal)
    {
        progressBar_LerpMethod.UpdateBarCall(valueInitial, valueFinal, lerpSpeedModifier: 1, queueRequest: false);
      
        var rushable = ListToIterate[_currentIndice]; Debug.LogWarning(rushable.CurrentProgress);
        if (rushable.CurrentProgress >= 1)
        {
            rushable.OnProgressTicked -= UpdateByProgress;
            var variablePanelInterface = (IVariableButtonPanel)this;
            variablePanelInterface.SetButtonLayout(1);
            popupButtons[1].SetupButton(ButtonFunctionType.PopupPanel.ProgressPanel_BackToShop);
            _progressTextField.text = "Complete";

        }
        else
        {
            _progressTextField.text = ConvertTime.ToHourMinSec(rushable.RemainingDuration);
            if (rushable is IRushableWithEnergy rushableWithEnergy)
            {
                popupButtons[0].ModifyButtonValue(rushableWithEnergy.GetCurrentRushCostEnergy);
                popupButtons[1].ModifyButtonValue(rushableWithEnergy.GetCurrentRushCostGem);
            }
            else // Implies that is Irushable (with gem only)
            {
                popupButtons[1].ModifyButtonValue(rushable.GetCurrentRushCostGem);
            }
        }       
    }


    public void BrowseInfo(IRushable blueprint_IN)
    {
        LoadSingleItem(blueprint_IN);
        DisplayContainers();
    }

    protected override string DefaultPopupHeader()
    {
        return NativeHelper.BuildString_Append(_PopupHeader + " " + (_currentIndice + 1).ToString() + " / " + ListToIterate.Count.ToString());
    }

    public void QuickUnload()
    {
        ListToIterate[_currentIndice].OnProgressTicked -= UpdateByProgress;
    }
}
