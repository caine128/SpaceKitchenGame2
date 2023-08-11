using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInvestable 
{
    (int currentTickAmount, int maxTickAmount) TickAmounts { get; }
    (int gold,int gem) GetCostsPerTick();
    void TryInvest(ISpendable spendable, int tickAmount, out int tokensToReturn);

    event Action OnInvested;
}
