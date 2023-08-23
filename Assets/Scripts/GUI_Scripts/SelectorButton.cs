
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class SelectorButton<T> : Button_Base
    where T : System.Enum
{

    [SerializeField] public T type;   
    public RectTransform rt;                     // TODO : later to make autoproperty !!!! or dont display on editor !! its confusing
    protected bool isMainSelector = false;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void SetButtonColor(Color colorIN)
    {
        buttonImage_Adressable.color = colorIN;
    }

    public abstract void AssignPanel(object panel);
 

    public void AssignCriteria(T type_IN)
    {
        type = type_IN;
    }

    public void SetButtonImageVisibility(bool isVisible)
    {
        if (isVisible)
        {
            if(buttonImage_Adressable.enabled !=true || buttonImage_Adressable.raycastTarget != true)
            {
                buttonImage_Adressable.raycastTarget = buttonImage_Adressable.enabled = true;
            }
        }
        else
        {
            if (buttonImage_Adressable.enabled != false || buttonImage_Adressable.raycastTarget != false)
            {
                buttonImage_Adressable.raycastTarget = buttonImage_Adressable.enabled = false;
            }
        }
    }

}
