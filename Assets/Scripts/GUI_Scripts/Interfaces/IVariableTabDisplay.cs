using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVariableTabDisplay 
{
    int TabAmountToDisplay { get; }
    void ArrangeTabButtons(int tabAmountToDisplay_IN);
}
