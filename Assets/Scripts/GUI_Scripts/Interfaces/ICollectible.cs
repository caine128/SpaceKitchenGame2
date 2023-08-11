using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible :IAmountable
{
    public void SetAmount(int amount);
    
}
