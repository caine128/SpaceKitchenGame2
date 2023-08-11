using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ExtraComponent : GameObject , IEquatable<ExtraComponent> , IToolTipDisplayable
{
    public ExtraComponentsType.Type ExtraComponentType { get { return extraComponentType; } }
    private readonly ExtraComponentsType.Type extraComponentType;

    private readonly int indexNo;                                       // Maybe ALL the Gameitem Static info can be drawn with index numbers !! Later to consider 
    public ExtraComponent(int indexNo_IN, DateTime? dateLastCrafted_IN = null)
    {
        indexNo = indexNo_IN;
        extraComponentType = ResourcesManager.Instance.Resources_SO.extraComponentsList[indexNo].type;
        DateLastCrafted = dateLastCrafted_IN ?? DateTime.Now;
    }

    public ToolTipInfo GetToolTipText()
        => new(bodytextAsColumns: new string[1] { GetDescription() }, header: GetName(), footer: null);
    /*{
        return new string[]
                            {
                                NativeHelper.BuildString_Append(GetName(),
                                                                Environment.NewLine,
                                                                GetDescription())
                            };
    }*/

    public override AssetReferenceAtlasedSprite GetAdressableImage()
    {
        return ResourcesManager.Instance.Resources_SO.extraComponentsList[indexNo].spriteRef;
    }

    public override string GetName()
    {
        return ResourcesManager.Instance.Resources_SO.extraComponentsList[indexNo].name;
    }
    
    public override string GetDescription()
    {
        return ResourcesManager.Instance.Resources_SO.extraComponentsList[indexNo].description;
    }

    public bool Equals(ExtraComponent other)
    {
        if (other == null || GetType() != other.GetType())
        {
            return false;
        }
        else
        {
            return GetName() == other.GetName();
        }
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ExtraComponent);
    }

    public override int GetHashCode()
    {
        return GetName().GetHashCode() * 21;
    }
}
