using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GUI_PlayerStats_Manager : MonoBehaviour
{
    private static GUI_PlayerStats_Manager _instance;
    public static GUI_PlayerStats_Manager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    //public const float LERP_DURATION = .3f;

    [SerializeField] private Canvas canvas;

    [SerializeField] private GUI_LerpMethods_Int levelBar_ExperienceExistingAmount;
    [SerializeField] private GUI_LerpMethods_Int levelBar_ExperienceMaxAmount;
    [SerializeField] private GUI_LerpMethods_Int energyBar_ExistingAmount;
    [SerializeField] private GUI_LerpMethods_Int energyBar_MaxAmount;
    [SerializeField] private GUI_LerpMethods_Int goldBar_ExistingAmount;
    [SerializeField] private GUI_LerpMethods_Int gemBar_ExistingAmount;

    [Space]
    [Space]
    [Space]

    [SerializeField] private TextMeshProUGUI text_LevelBar_LevelText;
    [SerializeField] private TextMeshProUGUI text_LevelBar_ExperienceExistingAmount; // TODO : THOSE WILL BE NEEDED FOR LOAD GAME 
    [SerializeField] private TextMeshProUGUI text_LevelBar_ExperienceMaxAmount;
    [SerializeField] private TextMeshProUGUI text_EnergyBar_ExistingAmount;
    [SerializeField] private TextMeshProUGUI text_EnergyBar_MaxAmount;
    [SerializeField] private TextMeshProUGUI text_GoldBar_ExistingAmount;
    [SerializeField] private TextMeshProUGUI text_GemBar_ExistingAmount;
    [SerializeField] private TextMeshProUGUI text_CharacterLevel;
    [SerializeField] private TextMeshProUGUI text_ReplacableStat_Amount;

    [Space]
    [Space]
    [Space]

    [SerializeField] private GUI_LerpMethods_Float experienceArea_Progressbar;
    [SerializeField] private GUI_LerpMethods_Float energyBar_Progressbar_BG;

    [Space]
    [Space]
    [Space]

    [SerializeField] private Text_Color_Setter EnergyBar_ExistingAmount;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = this;
                    // TODO : LATER TO MAKE DONTDESTROY ON LOAD OBJECT AND ARRANGE THE VARIABLE REFERENCES 
                }
            }
        }
    }


    public void SetStat(StatName.Stat statName_IN, int initialValue_IN, int finalValue_IN = 0, float lerpSpeedModifier_IN = 1, float initialProgressAmount_IN = 0, float finalProgressAmount_IN = 0, int maxValue_IN = 0)
    {
        SelectStatOperationType(statName_IN, initialValue_IN, finalValue_IN, lerpSpeedModifier_IN, initialProgressAmount_IN, finalProgressAmount_IN, maxValue_IN);
    }

    private void SelectStatOperationType(StatName.Stat statName, int initialValue, int finalValue, float lerpSpeedModifier, float initialProgressAmount, float finalProgressAmount, int maxValue)
    {
        switch (statName)
        {
            case (StatName.Stat.level):
                SetStatFlat(text_LevelBar_LevelText, initialValue);
                break;
            case (StatName.Stat.experienceExisting):
                SetStatAnimate(levelBar_ExperienceExistingAmount, initialValue, finalValue, lerpSpeedModifier);
                SetProgressBarAnimate(experienceArea_Progressbar, initialProgressAmount, finalProgressAmount, lerpSpeedModifier);
                break;
            case StatName.Stat.experienceMax:
                SetStatAnimate(levelBar_ExperienceMaxAmount, initialValue, finalValue, lerpSpeedModifier);
                break;
            case StatName.Stat.energyExisting:
                SetStatAnimate(energyBar_ExistingAmount, initialValue, finalValue, lerpSpeedModifier);
                SetProgressBarAnimate(energyBar_Progressbar_BG, initialProgressAmount, finalProgressAmount, lerpSpeedModifier);
                SetStatColor(EnergyBar_ExistingAmount);
                break;
            case StatName.Stat.energyMax:
                SetStatAnimate(energyBar_MaxAmount, initialValue, finalValue, lerpSpeedModifier);
                break;
            case StatName.Stat.gold:
                SetStatAnimate(goldBar_ExistingAmount, initialValue, finalValue, lerpSpeedModifier, toScreenFormatter: ISpendable.ToScreenFormat);
                break;
            case StatName.Stat.gem:
                SetStatAnimate(gemBar_ExistingAmount, initialValue, finalValue, lerpSpeedModifier, toScreenFormatter: ISpendable.ToScreenFormat);
                break;
            case StatName.Stat.researchPoint:
                if (PanelManager.SelectedPanels.TryPeek(out InvokablePanelController scrollRelatedPanel) &&
                    (scrollRelatedPanel.MainPanel is CraftPanel_Manager or RecipeInfoPanel_Manager or ResearchPopupPanel)) SetStatFlat(text_ReplacableStat_Amount, initialValue); 
                break;
            case StatName.Stat.inventoryCapacity:
                if (PanelManager.SelectedPanels.TryPeek(out InvokablePanelController inventoryCapRelatedPanel) &&
                    (inventoryCapRelatedPanel.MainPanel is InventoryPanel_Manager or GameItemInfoPanel_Manager or EnhanceItemPopupPanel or DeleteItemPopupPanel or ConfirmationPopupPanel)) SetStatFlat(text_ReplacableStat_Amount, initialValue, maxValue);
                break;
            case StatName.Stat.shopCapacity:
                if (PanelManager.SelectedPanels.TryPeek(out InvokablePanelController shopCapRelatedPanel) &&
                    (shopCapRelatedPanel.MainPanel is ShopPanel_Manager or ShopUpgradesPanel_Manager or ConfirmationPopupPanel or BuildOptionsPanel_Manager)) SetStatFlat(text_ReplacableStat_Amount, initialValue, maxValue);
                break;
            default: break;
        }
    }
    private void SetStatFlat(TextMeshProUGUI statToSet, params int[] initialAndMaxValues)
    {
        statToSet.text = initialAndMaxValues.Length switch
        {
            1 => initialAndMaxValues[0].ToString(),
            2 => string.Format("{0}/ {1}", initialAndMaxValues[0].ToString(), initialAndMaxValues[1].ToString()),
            _ => initialAndMaxValues[0].ToString(),
        };
        //statToSet.text = newValue.ToString();
    }

    private void SetStatAnimate(GUI_LerpMethods_Int statToLerp, int initialValue, int finalValue, float lerpSpeedModifier, Func<int,string> toScreenFormatter = null)
    {
        statToLerp.UpdatTextCall(initialValue, finalValue, lerpSpeedModifier, toScreenFormatter);
    }
    private void SetProgressBarAnimate(GUI_LerpMethods_Float progressBar, float initialProgressAmount, float finalProgressAmount, float lerpSpeedModifier)
    {
        progressBar.UpdateBarCall(initialProgressAmount, finalProgressAmount, lerpSpeedModifier, queueRequest: true);
    }

    private void SetStatColor(Text_Color_Setter statToColor)
    {
        statToColor.SetColor();
    }
}
