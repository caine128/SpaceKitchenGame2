using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContentDisplay_JustSprite : ContentDisplay, IPlacableRt
{
    public  RectTransform RT => _rt;
    [SerializeField] private RectTransform _rt;

    public override void Load(ContentDisplayInfo info)
    {
        Load((ContentDisplayInfo_JustSprite)info);
    }

    private void Load(ContentDisplayInfo_JustSprite info)
    {
        SelectAdressableSpritesToLoad(info.spriteRef);
    }

    public override void Unload()
    {
        UnloadAdressableSprite();
    }
}


