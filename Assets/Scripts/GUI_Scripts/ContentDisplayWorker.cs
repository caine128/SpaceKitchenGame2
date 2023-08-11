using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ContentDisplayWorker : ContentDisplay_WithText_PR<WorkerType.Type>, IPlacableRt
{
    public RectTransform RT { get { return _rt; } }
    [SerializeField] private RectTransform _rt;
    int requiredLevel;

    public sealed override void Load(ContentDisplayInfo_ContentDisplay_WithText info)
    {
        base.Load(info);
        requiredLevel = productRecipe.recipeSpecs.requiredworkers[indexNO].requiredWorkerLevel;
        contentType = productRecipe.recipeSpecs.requiredworkers[indexNO].requiredWorker;

        SelectAdressableSpritesToLoad(ImageManager.SelectSprite(contentType.ToString()));
        contentInfo.text = productRecipe.recipeSpecs.requiredworkers[indexNO].requiredWorkerLevel.ToString();
        contentInfo.SetAsModifiableSpec(requiredLevel.ToString(),
                                        isModified: false,
                                        isAmountEnough: IsExistingAmountEnough());
    }

    private bool IsExistingAmountEnough()
    {
        //return productRecipe.AreWorkerRequirementsMet.All(awm => awm);
        return productRecipe.AreWorkerRequirementsMet?[indexNO]??  false;
        /*if(CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker]
                            .Where(chr => ((Worker)chr).workerspecs.workerType == contentType
                                          && chr.isHired && chr.GetLevel() >= requiredLevel)
                            .Any())
        {
            return true;
        }
        else
        {
            return false;
        }*/
    }


    public override void Unload()  // LATER TO IMPLEMENT 
    {
        if (contentType != null)
        {
            UnloadAdressableSprite();
            contentType = null;
        }
    }

    //private void SelectAndLoadAdressableSprite()
    //{
    //    Debug.LogWarning("to be implemented");

    //}


}
