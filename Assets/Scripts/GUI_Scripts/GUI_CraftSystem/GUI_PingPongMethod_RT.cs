
using TMPro;
using UnityEngine;

public class GUI_PingPongMethod_RT : GUI_PingPongMethod<RectTransform, Vector3>
{
    [SerializeField] Vector3 displayvalue; // for debug purposes 

    private void Start()// for debug purposes 
    {
        displayvalue = sizeable.localScale;
    }
    protected override Vector3 GetOriginalValue()
        => sizeable.localScale;

    protected override Vector3 SetSizeMaxDelta()
        => Vector3.Scale(sizeable.localScale, new Vector3(0.05f, 0.05f, 1f));

    protected override void SetDynamicValue()
    {
        var timeVariable = Time.time / 10;

        sizeable.localScale = new Vector3(x:originalValue.x + Mathf.PingPong(timeVariable, valueMaxDelta.x),
                                          y:originalValue.y + Mathf.PingPong(timeVariable, valueMaxDelta.y));
    }

    public override void ResetToOriginalValue()
        =>sizeable.localScale = originalValue;
   
        
}
