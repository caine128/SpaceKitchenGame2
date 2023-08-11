using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvokeWorkStationInfoButton : PanelInvokeButton
{
    [SerializeField] protected GUI_TintScale gUI_TintScale;
    [SerializeField] private TextMeshProUGUI buttonText;

    public void SetButtonText(string buttonText)
    {
        this.buttonText.text = buttonText;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        gUI_TintScale.TintSize();

        ShopData.ShopUpgradesIteration_Dict.TryGetValue(ShopUpgradeType.Type.WorkstationUpgrades, out List<ShopUpgrade> shopUpgrades);
        var selectedWorkersWorkstationType = ((Worker)CharactersInfoPanel_Manager.Instance.SelectedRecipe).workerspecs.workStationPrerequisites[0].type;
        var selectedShopUpgrade = shopUpgrades.FirstOrDefault(su => ((WorkStationUpgrade)su).GetWorkstationType() == selectedWorkersWorkstationType);

        if(selectedShopUpgrade is null)
        {
            Debug.LogError("workstation cannot be found");
        }
        else
        {
            var panelLoadData = new PanelLoadData(mainLoadInfo: selectedShopUpgrade, panelHeader: selectedShopUpgrade.GetName(), tcs_IN: null);
            PanelManager.ActivateAndLoad(invokablePanel_IN: PanelManager.InvokablePanels[typeof(ShopUpgradesInfoPanel_Manager)],
                                         panelLoadAction_IN: () => ShopUpgradesInfoPanel_Manager.Instance.LoadPanel(panelLoadData));
        }
    }
}
