using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Commander : Character

{

    public enum Type
    {
        Xandar = 1,
        Phenom = 2,
    }


    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        throw new System.NotImplementedException();
    }

    public override string GetDescription()
    {
        throw new System.NotImplementedException();
    }

    public override string GetName()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateCharacterXP(int gainedXP)
    {
        throw new System.NotImplementedException();
    }

    public override void TryHireCharacter(ISpendable spendable)
    {
        throw new System.NotImplementedException();
    }

    public override int GetXpToNextLevel()
    {
        throw new System.NotImplementedException();
    }

    public override ToolTipInfo GetToolTipText()
    {
        throw new System.NotImplementedException();
    }
}
