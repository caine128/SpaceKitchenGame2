using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class AdressableImage : Image
{
    protected override void Awake()
    {
        base.Awake();
        this.preserveAspect = true;
    }

    private AssetReferenceT<Sprite> loadedSpriteRef = null;

    public new Sprite sprite // to block external reach to sprite propoerty, otherwise someone can change the sprite bypasing adressable Load and Unload methods
    {
        get => base.sprite;      
    } 

    public void LoadSprite(AssetReferenceT<Sprite> newSpriteRef_IN)
    {
        if(loadedSpriteRef != null && loadedSpriteRef == newSpriteRef_IN)
        {
            return;
        }

        else if(loadedSpriteRef != null && loadedSpriteRef != newSpriteRef_IN)
        {
            SpriteLoader.Instance.UnloadAdressable(loadedSpriteRef);
        }

        loadedSpriteRef = newSpriteRef_IN;
        SpriteLoader.Instance.LoadAdressable(loadedSpriteRef, sprite => base.sprite = sprite);
    }

    public void UnloadSprite()
    {
        if(this.sprite != null)
        {
            base.sprite = null;
            SpriteLoader.Instance.UnloadAdressable(loadedSpriteRef);
            loadedSpriteRef = null;
        }
    }

    public bool IsLoadedSpriteRefSameWith(AssetReferenceT<Sprite> newSpriteRef) => loadedSpriteRef == newSpriteRef;
}
