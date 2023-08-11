using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalculateFillAmount 
{
    public static float CalculateFill(int newAmount, int maxAmount)
    {
        float fillAmount = (float)newAmount / maxAmount;
        return fillAmount;
    }
}
