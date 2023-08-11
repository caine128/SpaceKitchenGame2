using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ContentDisplay_JustSpriteAndText : ContentDisplay_JustSprite
{
    [SerializeField] protected TextMeshProUGUI textField;
    public override void Load(ContentDisplayInfo info)
    {
        Load((ContentDisplayInfo_JustSpriteAndText)info);
    }

    private  void Load(ContentDisplayInfo_JustSpriteAndText info)
    {
        textField.text = info.textVal;
        SelectAdressableSpritesToLoad(info.spriteRef);
    }
}
