using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.Hardware;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class TabPanel_GameItemInfo : TabPanel_Animated<Tab.GameItemInfoTabs>
{
    //[SerializeField] private TextMeshProUGUI valueText; // TO REMOVE BOtH FROM CODE AND INSPECTOR OBJEC
    //[SerializeField] private Image valueFieldBG;  // TO REMOVE BOtH FROM CODE AND INSPECTOR OBJECT 
    //[SerializeField] private Image valueIcon;     // TO REMOVE BOtH FROM CODE AND INSPECTOR OBJEC   
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private ContentDisplayPopup_Generic[] contentDisplays;
    private int amountOfNecessaryContentDisplays = 0;
    private Vector2[] contentDisplaysOriginalPositions;
    public override Tab.GameItemInfoTabs TabType { get { return _tabType; } }
    [SerializeField] private Tab.GameItemInfoTabs _tabType;


    private void Awake()
    {
        co = new IEnumerator[1];
        contentDisplaysOriginalPositions = contentDisplays.Select(ct=> ct.RT.anchoredPosition).ToArray();
    }

    public override void LoadInfo()
    {
        amountOfNecessaryContentDisplays = 0;
        IEnumerable<ContentDisplayInfo_PopupGeneric> contentDisplayData = Enumerable.Empty<ContentDisplayInfo_PopupGeneric>();
        var selectedItem = GameItemInfoPanel_Manager.Instance.SelectedRecipe;

        if (selectedItem is IStatable statable)
        {
            var statUpgradable = statable as IStatUpgradable;
            contentDisplayData = statable.GetStatBonuses().ConvertEnumerable(converter: msb => new ContentDisplayInfo_PopupGeneric(spriteRef_IN: msb.atlasedSpriteRef,
                                                                                                                                   contentTitle: msb.statType.ToString(),
                                                                                                                                   contentValue: msb.statBonus.ToString(),
                                                                                                                                   isValueModified: statUpgradable?.IsStatModified(msb.statType) ?? null,
                                                                                                                                   GetTooltipText: statUpgradable != null
                                                                                                                                                        ? () => statUpgradable.GetTooltipTextForStatModifiers(statType: msb.statType,
                                                                                                                                                                                                              header: $"{msb.statType} Modifiers",
                                                                                                                                                                                                              footer: "test purposes only")
                                                                                                                                                        : null));
                                                                                                                                   /*statable is IStatUpgradable statUpgradable
                                                                                                                                                        ? () => statUpgradable.GetTooltipTextForStatModifiers(statType: msb.statType,
                                                                                                                                                                                                              header: $"{msb.statType} Modifiers",
                                                                                                                                                                                                              footer: "test purposes only")
                                                                                                                                                        : null));*/
        }          
                                                                                                                                                                                                                                                                     
        if (selectedItem is IValuable valuable)
        {
            var valueUpgradable = valuable as IValueUpgradable;
            contentDisplayData = contentDisplayData.AddToEnumerable(factor: () => new ContentDisplayInfo_PopupGeneric(spriteRef_IN: ImageManager.SelectSprite("ValueIncrease"),
                                                                                                                     contentTitle: "Value",
                                                                                                                     contentValue: ISpendable.ToScreenFormat(valuable.GetValue()), //.ToString(),
                                                                                                                     isValueModified: valueUpgradable?.IsValueModified ?? null,
                                                                                                                     GetTooltipText: valueUpgradable != null
                                                                                                                                           ? () => valueUpgradable.GetToolTipTextForValueModifiers(header: "Value Modifiers",
                                                                                                                                                                                                   footer: "test footer")
                                                                                                                                           : null));
        }
           

        amountOfNecessaryContentDisplays = contentDisplayData.Count();

        contentDisplays.PlaceContainersMatrix(positions: contentDisplaysOriginalPositions.Skip(count: contentDisplays.Length - amountOfNecessaryContentDisplays).ToArray());
        contentDisplays.LoadContainers(contentDisplayData, hideAtInit: true);
    
        descriptionText.text = GameItemInfoPanel_Manager.Instance.SelectedRecipe.GetDescription();
        amountText.text = GameItemInfoPanel_Manager.Instance.SelectedRecipe.GetAmount().ToString();
        Debug.LogWarning("new amount " + GameItemInfoPanel_Manager.Instance.SelectedRecipe.GetAmount().ToString());
    }

    

    public override void DisplayContainers()
    {
        contentDisplays.SortContainers(customInitialValues: null,
                                       secondaryInterpolations: null,
                                       amountToSort_IN: amountOfNecessaryContentDisplays,
                                       enumeratorIndex: 0,
                                       parentPanel_IN: this,
                                       lerpSpeedModifiers: null);
    }

    public override void HideContainers()
    {
        contentDisplays.HideContainers();
    }
    public override void UnloadInfo()
    {
        //valueText.text = descriptionText.text = amountText.text = string.Empty;
        foreach (var contentDisplay in contentDisplays)
        {
            contentDisplay.Unload();
        }
        base.UnloadInfo();
    }

}
