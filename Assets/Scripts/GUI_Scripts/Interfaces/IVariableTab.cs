using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVariableTab 
{
    RectTransform RT { get; }
    float originalPosition_X { get;}
    float originalSizeDelta_X { get; }
    void GetOriginalSizePosition();
    void ArrangeButtonSizePosition(int buttonIndex_IN, float resizeRatio_IN);
    void ResetButtonSizePosition();
}
