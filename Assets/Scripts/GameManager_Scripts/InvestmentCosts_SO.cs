using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new InvestmentCost", menuName = "InvestmentCost")]
public class InvestmentCosts_SO : ScriptableObject
{
    public InvestmentCosts[] investmentCosts;

    [Serializable]
    public struct InvestmentCosts
    {
        public int level;
        public int requiredTickAmount;
        public int baseGoldPerTick;
        public int baseGemPerTick;
    }


 
}
