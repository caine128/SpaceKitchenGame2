using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[RequireComponent(typeof(GUI_LerpMethods_Scale))]
public class EnhanceButtons : MultiPurposeButton<ButtonFunctionType.GameItemInfoPanel>,IMultiPanelInvokeButton, IGUI_Animatable  //, IReassigner, ISinglePanelInvokeButton,
{
    [SerializeField] private Image buttonBG;
    //[SerializeField] private Adressable_SpriteDisplay imageContainer_NotificationBG;
    [SerializeField] private AdressableImage imageContainer_Notification_Adressable;

    [SerializeField] private TextMeshProUGUI amountNotificationText;

    public EnhancementType.Type EnhancementType { get { return enhancementType; } }
    [SerializeField] private EnhancementType.Type enhancementType;

    private static IReassignablePanel _reassignablePanel;

    public InvokablePanelController[] InvokablePanels => _invokablePanels;
    [SerializeField] private InvokablePanelController[] _invokablePanels;

    private GUI_LerpMethods_Scale gUI_LerpMethods_Scale;

    public void Awake()                 // LATER TO TAKE UP
    {
        gUI_LerpMethods_Scale = GetComponent<GUI_LerpMethods_Scale>();
        buttonNames = new string[] { "Runestone", "Element", "Spirit", "Hire" };
       _reassignablePanel = (IReassignablePanel)_invokablePanels[0].MainPanel;
    }


    public void AnimateWithRoutine(Vector3? customInitialValue,
                                   (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation, 
                                   bool isVisible, 
                                   float lerpSpeedModifier,
                                   Action followingAction_IN)
    {
        Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
        gUI_LerpMethods_Scale.Rescale(customInitialValue: customInitialValue, 
                                      secondaryInterpolation: secondaryInterpolation,
                                      finalScale: scale, 
                                      followingAction_IN: followingAction_IN);
    }

    public void ScaleDirect(bool isVisible, (Func<RectTransform, bool> finalValueChecker, Action<RectTransform> finalValueSetter)? finalValueOperations)
    {
        Vector3 scale = isVisible == true ? Vector3.one : Vector3.zero;
        gUI_LerpMethods_Scale.RescaleDirect(finalScale: scale, 
                                            finalValueOperations: finalValueOperations);
    }


    public override void SetupButton(ButtonFunctionType.GameItemInfoPanel buttonFunction_IN)
    {
        IEnhanceable enhanceableBluePrint = GameItemInfoPanel_Manager.Instance.SelectedRecipe as IEnhanceable;

        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.GameItemInfoPanel.SlotOpenToEnhancement:

                buttonBG.color = Color.yellow;
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("PlusIcon"));
                buttonName.text = enhancementType switch
                {
                    global::EnhancementType.Type.Runestone_Enhancement => buttonNames[0],
                    global::EnhancementType.Type.Elemental_Enhancement => buttonNames[1],
                    global::EnhancementType.Type.Spirit_Enhancement => buttonNames[2],
                    _ => "",
                };

                SetupAmountNotifications();
                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();
                    _reassignablePanel.ReassignPanelLayout(GameItemType.Type.Enhancement, enhancementType, IReassignablePanel.AssignedState.Inventory_FromProductToEnhance);
                    PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[0], panelLoadAction_IN: null);        
                };
                

                break;

            case ButtonFunctionType.GameItemInfoPanel.SlotEnhanced:   // TODO : AFFINITTY indicator !! 

                if (imageContainer_Notification_Adressable.enabled != false || amountNotificationText.enabled != false) imageContainer_Notification_Adressable.enabled = amountNotificationText.enabled = false;
                var enhancement = enhanceableBluePrint.enhancementsDict_ro[enhancementType];

                buttonBG.color = NativeHelper.GetQualityColor(enhancement.GetQuality());
                buttonInnerImage_Adressable.LoadSprite(enhancement.GetAdressableImage());
                buttonName.text = enhancement.GetName();

                buttonFunctionDelegate = () =>
                {
                    gUI_TintScale.TintSize();

                    var enhancePanelLoadData = new PopupPanel_Enhancement_LoadData(
                            mainLoadInfo: (SortableBluePrint)enhanceableBluePrint,
                            panelHeader: null,
                            tcs_IN: null,
                            enhancement_IN: enhancement,
                            panelState_IN: null
                            );

                    PanelManager.ActivateAndLoad(invokablePanel_IN: _invokablePanels[1], 
                        panelLoadAction_IN:
                        () => EnhanceItemPopupPanel.Instance.LoadPanel(enhancePanelLoadData));
                }; 
                break;

            case ButtonFunctionType.GameItemInfoPanel.SlotRequiresWorker:   // TODO : To add later when characters are made !! 
                break; 
        }

    }

    private void SetupAmountNotifications()
    {
        var existingEnhacements = Inventory.Instance.CheckAmountInInventory_SubType(enhancementType, GameItemType.Type.Enhancement);
        if (existingEnhacements > 0)
        {
            if (imageContainer_Notification_Adressable.enabled != true || amountNotificationText.enabled != true)
            {
                imageContainer_Notification_Adressable.enabled = amountNotificationText.enabled = true;
            }

            imageContainer_Notification_Adressable.LoadSprite(ImageManager.SelectSprite("NotificationBG"));
            amountNotificationText.text = existingEnhacements.ToString();
        }
        else
        {
            if (imageContainer_Notification_Adressable.enabled != false || amountNotificationText.enabled != false)
            {
                imageContainer_Notification_Adressable.enabled = amountNotificationText.enabled = false;
            }
        }
    }


    public void EnhancementInfo()
    {

    }

    public void Recruit()
    {

    }

    //public void InvokePanel(Action panelLoadAction = null)
    //{
    //    ReassignablePanel.ReassignPanelLayout(GameItemType.Type.Enhancement, enhancementType, IReassignablePanel.AssignedState.Inventory_FromProductToEnhance);
    //    OnInvokeButtonPressed?.Invoke(panelToInvoke, panelLoadAction);
    //}


    public override void UnloadButton()
    {
        base.UnloadButton();
        buttonBG.color = Color.white;       // later to deassign those images a well 
        imageContainer_Notification_Adressable.UnloadSprite();

    }

}
