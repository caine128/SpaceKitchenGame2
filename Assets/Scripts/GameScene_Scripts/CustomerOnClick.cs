using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerOnClick : MonoBehaviour
{

    public event EventHandler <OnFoodSoldEventArgs> onFoodSold;

    public class OnFoodSoldEventArgs
    {
        public int gold;
    }

    private void OnMouseDown ()
    {
        onFoodSold?.Invoke (this, new OnFoodSoldEventArgs { gold = 20 });        
    }

}
