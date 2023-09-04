using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAmountChangeCallbackReceiver 
{
    public void SubscribeToAmountChangeCallback(bool shouldSubscribe);
    public void AmountChangeCallback(int newAmount);
}
