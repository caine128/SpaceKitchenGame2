using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*public class GUI_PingPongMethod_Text : MonoBehaviour
{
    private TextMeshProUGUI textToSize;
    private bool canPingPong = false;
    public bool CanPingPong
    {
        get { return canPingPong; }
        set { canPingPong = value;}
    }

    private float sizeMaxDelta;
    private float originalTextSize;

    private void Start()
    {
        textToSize = GetComponent<TextMeshProUGUI>();
        originalTextSize = textToSize.fontSize;
        sizeMaxDelta = (textToSize.fontSize * 1.25f) - textToSize.fontSize; 
    }

    private void Update()
    {
        if (canPingPong)
        {
            float dynamicFinalSize = originalTextSize + ( Mathf.PingPong(Time.time*10, sizeMaxDelta));

            textToSize.fontSize = dynamicFinalSize;
        }
    }



}*/

public class GUI_PingPongMethod_Text : GUI_PingPongMethod<TextMeshProUGUI, float>
{
    protected override float GetOriginalValue()
        => sizeable.fontSize;

    protected override float SetSizeMaxDelta()
        => (sizeable.fontSize * 1.25f) - sizeable.fontSize;

    protected override void SetDynamicValue()
        => sizeable.fontSize = originalValue + (Mathf.PingPong(Time.time * 10, valueMaxDelta));

    public override void ResetToOriginalValue()
        => sizeable.fontSize = originalValue;
}
