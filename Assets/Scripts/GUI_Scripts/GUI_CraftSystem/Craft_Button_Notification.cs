using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Craft_Button_Notification : MonoBehaviour, IConfigurablePanel
{
    [SerializeField] private TextMeshProUGUI notificationText; 

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        if (Radial_CraftSlots_Crafter.Instance != null)
        {
            Radial_CraftSlots_Crafter.Instance.onStartCrafting -= SetNotificationText;
            Radial_CraftSlots_Crafter.Instance.onReclaimCrafted -= SetNotificationText;
        }
    }
    //void Start()
    //{
    //    PanelConfig();
    //}
    public void PanelConfig()
    {
        Radial_CraftSlots_Crafter.Instance.onStartCrafting += SetNotificationText;
        Radial_CraftSlots_Crafter.Instance.onReclaimCrafted += SetNotificationText;
        SetNotificationText(null, new Radial_CraftSlots_Crafter.OnCraftingEventArgs { remainingCraftAmount = Radial_CraftSlots_Crafter.Instance.maxCraftSlotsForLevel - Radial_CraftSlots_Crafter.Instance.activeCraftAmount });
    }


    private void SetNotificationText(object sender, Radial_CraftSlots_Crafter.OnCraftingEventArgs e)
    {
        notificationText.text = e.remainingCraftAmount > 0 ? e.remainingCraftAmount.ToString() : "+";
    }


}
