using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GUI_TintScale))]
public abstract class MultiPurposeButton<T_FunctionType> : Button_Base  //MonoBehaviour, IPointerDownHandler
    where T_FunctionType : Enum
{
    //[SerializeField] protected Adressable_SpriteDisplay buttonInnerImage;
    [SerializeField] protected AdressableImage buttonInnerImage_Adressable;
    
    protected string[] buttonNames;
    [SerializeField] protected TextMeshProUGUI buttonName;
    [SerializeField] protected GUI_TintScale gUI_TintScale;

    protected delegate void ButtonFunctionDelegate();
    protected ButtonFunctionDelegate buttonFunctionDelegate;
    protected T_FunctionType buttonFunction;       // this should go as well !!! serialied for debug purposes 



    public abstract void SetupButton(T_FunctionType buttonFunction_IN);

    public sealed override void OnPointerUp(PointerEventData eventData)
    {

    }
    public sealed override void OnPointerDown(PointerEventData eventData)    // SHOULD BE ASYNC VOID METHOD ??
    {
        buttonFunctionDelegate();
    }

    protected void DoNothing()
    {

    }

    public virtual void UnloadButton()
    {
        buttonName.text = null;
        buttonInnerImage_Adressable.UnloadSprite();
        buttonFunctionDelegate = null;
    }
}
