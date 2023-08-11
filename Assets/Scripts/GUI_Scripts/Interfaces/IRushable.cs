using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRushable //Default Rushable, with Gems
{
    int TotalRushCostGem { get; } //rush cost by Gems
    int GetCurrentRushCostGem => Mathf.CeilToInt(TotalRushCostGem * (1 - CurrentProgress));
    void Rush();
    SortableBluePrint BluePrint { get; }
    float CurrentProgress { get; }
    float RemainingDuration { get; }
    bool IsReadyToReclaim { get; }
    float MaxUpgradeDuration { get; }
    event Action<float, float> OnProgressTicked;

}


public interface IRushableWithEnergy : IRushable
{
    int TotalRushCostEnergy { get; }
    int GetCurrentRushCostEnergy => Mathf.CeilToInt(TotalRushCostEnergy * (1 - CurrentProgress));
}