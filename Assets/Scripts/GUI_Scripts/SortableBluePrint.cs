using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;


public abstract class SortableBluePrint_Base
{
    public abstract string GetName();
    public abstract string GetDescription();
}

public class SortableBluePrint_ExtractedData<T_SortableBluePrint> : SortableBluePrint_Base
    where T_SortableBluePrint:SortableBluePrint
{
    public object Data { get => _data; }
    private readonly object _data;
    public readonly T_SortableBluePrint BluePrint;

    public SortableBluePrint_ExtractedData(object data, T_SortableBluePrint bluePrint)
    {
        _data=data;
        BluePrint = bluePrint;
    }

    public override string GetName()
    {
        Debug.LogError("we shouldnt be requestingthis");
        return string.Empty;
    }

    public override string GetDescription()
    {
        Debug.LogError("we shouldnt be requestingthis");
        return string.Empty;
    }
}

public abstract class SortableBluePrint : SortableBluePrint_Base
{
    //public abstract string GetName();
    public abstract AssetReferenceAtlasedSprite GetAdressableImage();
    //public abstract string GetDescription();
}

public abstract class SortableBluePrint_MultiImage : SortableBluePrint_Base
{
    public abstract IEnumerable<AssetReferenceAtlasedSprite> GetAdressableImages();
}
