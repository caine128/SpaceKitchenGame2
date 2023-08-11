using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Button_Base : MonoBehaviour, IPointerDownHandler , IPointerUpHandler

{
    public abstract void OnPointerDown(PointerEventData eventData);
    public abstract void OnPointerUp(PointerEventData eventData);

    //[SerializeField] protected Adressable_SpriteDisplay buttonImage;
    [SerializeField] protected AdressableImage buttonImage_Adressable;

}
