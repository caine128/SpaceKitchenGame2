using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ContentDisplayPopup_Generic : ContentDisplay, IPointerDownHandler, IPointerUpHandler, IPlacableRt //, ILoadable<ContentDisplayInfo_PopupGeneric>
{
    private readonly static ClickValidator<ContentDisplayPopup_Generic> ClickValidator = new();
    public RectTransform RT => _rt;
    [SerializeField] private RectTransform _rt;

    public  Vector2 OriginalPosition => (Vector2)_originalPosition;
    private  Vector2? _originalPosition;

    public static float TextContainrWidth => (float)_textContainerWidth;
    private static float? _textContainerWidth;

    [SerializeField] private RectTransform _rtContentInfo;
    [SerializeField] private GUI_LerpMethods_Movement _lerpContentInfo;

    [SerializeField] private TextMeshProUGUI contentInfo;
    private Lazy<ToolTipInfo> _toolTipInfoShaped = null;

    protected sealed override void Awake()
    {
        base.Awake();

        if (_textContainerWidth == null)
        {
            _textContainerWidth = _rtContentInfo.rect.width;
        }
        if (_originalPosition == null)
        {
            _originalPosition = _rt.anchoredPosition;
        }
    }

    public override void Load(ContentDisplayInfo info)
    {
        Load((ContentDisplayInfo_PopupGeneric)info);
    }
    public void Load(ContentDisplayInfo_PopupGeneric info)
    {
        //clickableInfoObject = info.clickableInfoObject_IN ?? null;
        adressableImageContainers[0].raycastTarget =  info.spriteRef_IN is not null;

        if (!string.IsNullOrEmpty(info.contentTitle_IN) || !string.IsNullOrEmpty(info.contentValue_IN))
        {
            if (contentInfo.gameObject.activeInHierarchy != true) contentInfo.gameObject.SetActive(true);

            contentInfo.text = NativeHelper.BuildString_Append(
            info.contentTitle_IN,
            " ",
            Environment.NewLine,
            info.isValueModified_IN.GetValueOrDefault(defaultValue:false) == true
                ? MethodHelper.GiveRichTextString_Color(Color.red)
                : MethodHelper.GiveRichTextString_Color(Color.green),
            info.contentValue_IN);     //string.Concat(info.contentStrings_IN.contentTitle," ", info.contentStrings_IN.contentValue); // Change this to the correct Buildstring method

        }

        _toolTipInfoShaped = _toolTipInfoShaped = info.GetTooltipText_IN is not null
                                                    ? new Lazy<ToolTipInfo>(info.GetTooltipText_IN)
                                                    : null;
        SelectAdressableSpritesToLoad(info.spriteRef_IN);
        
    }

    public sealed override void AnimateWithRoutine(Vector3? customInitialValue,
                                                   (Action<float, RectTransform> interpolator, Action<RectTransform, bool> setValues)? secondaryInterpolation,
                                                   bool isVisible,
                                                   float lerpSpeedmodifier,
                                                   Action followingAction_IN)
    {
        base.AnimateWithRoutine(customInitialValue: customInitialValue,
                                secondaryInterpolation: secondaryInterpolation,
                                isVisible: isVisible,
                                lerpSpeedModifier: lerpSpeedmodifier,
                                followingAction_IN: followingAction_IN);
        
        if (string.IsNullOrEmpty(contentInfo.text))
        {
            contentInfo.gameObject.SetActive(false);
        }
        else 
        {
            contentInfo.gameObject.SetActive(false);
            _rtContentInfo.anchoredPosition = new Vector2(_rtContentInfo.rect.width / 2, 0);
            contentInfo.gameObject.SetActive(true);
            _lerpContentInfo.InitialCall(Vector2.zero);
        }     
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ClickValidator.StartValidating(this, eventData, DateTime.Now);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (ClickValidator.IsValidClick(this, eventData.position, DateTime.Now))
        {
            var toolTipText = _toolTipInfoShaped?.Value ?? null;
            if (toolTipText?.bodyTextAsColumns is not null && toolTipText.bodyTextAsColumns.Length > 0)
            {
                var panelToload = PanelManager.InvokablePanels[typeof(Tooltip_Panel_Manager)];

                PanelManager.ActivateAndLoad(invokablePanel_IN: panelToload,
                                            //preLoadAction_IN: () => (panelToload.MainPanel as Tooltip_Panel_Manager).SetDynamicBGSize(toolTipText),
                                            panelLoadAction_IN: () => (panelToload.MainPanel as Tooltip_Panel_Manager).SetDynamicBGSize(toolTipText, _rt));
            }
            else
            {
                Debug.LogWarning("There is no tooltip to display");
            }
        }      
        /* if (clickableInfoObject is not null)
         {
             switch (clickableInfoObject)
             {
                 case Enhancement enhancement:
                     Debug.Log(enhancement.GetName() + " " + enhancement.GetDescription() + " of quality " + enhancement.GetQuality());
                     break;
                 default:
                     Debug.LogError("This object type is not expected !!");
                     break;
             }
         }*/
    }

   /* public string ShapeToolTipText(ContentDisplayInfo_PopupGeneric rawTooltipInfo)
    {
        throw new NotImplementedException();
    }*/

    public override void Unload()
    {
        UnloadAdressableSprite();
        _toolTipInfoShaped = null;
        contentInfo.text = null;
    }


}



