using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Adressable_SpriteDisplay : MonoBehaviour
{
    private AssetReferenceT<Sprite> loadedSpriteRef = null;
    public Image ImageContainer { get; private set; }

    public void Awake()
    {
        ImageContainer = this.GetComponent<Image>();
    }

    public void LoadSprite(AssetReferenceT<Sprite> newSpriteRef_IN)
    {
        //this.ProcessAdressableSprites_Load(ref loadedSpriteRef, ref newSpriteRef_IN);

        if (loadedSpriteRef != null && loadedSpriteRef == newSpriteRef_IN)
        {
            return;
        }

        else if (loadedSpriteRef != null && loadedSpriteRef != newSpriteRef_IN)
        {
            SpriteLoader.Instance.UnloadAdressable(loadedSpriteRef);
        }
        loadedSpriteRef = newSpriteRef_IN;
        SpriteLoader.Instance.LoadAdressable(loadedSpriteRef, sprite => ImageContainer.sprite = sprite);//ImageContainer);


    }

    public void UnloadSprite()
    {
        if(ImageContainer.sprite != null)
        {
            ImageContainer.sprite = null;
            SpriteLoader.Instance.UnloadAdressable(loadedSpriteRef);
            loadedSpriteRef = null;
        }
        //this.ProcessAdressableSprite_Unload(ref loadedSpriteRef);
        //if (loadedSpriteRef != null) this.ProcessAdressableSprite_Unload(ref loadedSpriteRef);
    }

    public bool IsLoadedSpriteRefSameWith(AssetReferenceT<Sprite> newSpriteRef) => loadedSpriteRef == newSpriteRef;
     
}


