using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class Character : SortableBluePrint, ILevellable, IToolTipDisplayable
{
    protected int currentLevel;
    protected int currentXP;

    public bool isHired { get; protected set; }
    public bool isAtMaxLevel => this switch
    {
        Commander commander => throw new System.NotImplementedException(),
        //TaskForce Fleet To Be implemented !! TODO
        Worker worker => worker.GetLevel() >= CharacterManager.MaxLevel,
        _ => throw new System.NotImplementedException(),
    };

    public int GetLevel()
    {
        return currentLevel;
    }

    public float GetCurrentXP()
    {
        return currentXP;
    }

    public abstract int GetXpToNextLevel();
    protected abstract void UpdateCharacterXP(int gainedXP);
    public abstract void TryHireCharacter(ISpendable spendable);
    public abstract ToolTipInfo GetToolTipText();
}
