using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ContentContainer : MonoBehaviour
{
    //[SerializeField] protected Adressable_SpriteDisplay imageContainer;
    [SerializeField] protected AdressableImage imageContainer_Adressable;
    [SerializeField] protected TextMeshProUGUI contentText;

    public abstract void Load();

    public abstract void Unload();
}


public abstract class ContentContainer<T_Type> : ContentContainer   // Make This Updatable ??
    where T_Type:System.Enum
{
    [SerializeField] protected T_Type type;

}

