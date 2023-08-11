using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Infographics_Panel : Panel_Base , IDeallocatable
{
    [SerializeField] private ContentDisplay_JustSpriteAndText characterType_Text;
    [SerializeField] private TextMeshProUGUI characterProfession_Text;
    [SerializeField] private ContentDisplay_JustSprite[] unlocks;
    public bool isSubContainersAnimating;


    public IEnumerator CO
    {
        get => _co;
    }
    private IEnumerator _co;

    private int displayedSubcontainersAmount = 0;

    public void LoadPanel(Character character)
    {
        //HideContainers();
        switch (character)
        {
            case Worker worker:
                characterType_Text.Load(new ContentDisplayInfo_JustSpriteAndText(textVal_IN: "Worker",
                                                                                 spriteRef_IN: ImageManager.SelectSprite(worker.workerspecs.workerType.ToString())));
                characterProfession_Text.text = worker.workerspecs.workerType.ToString().Split("_").Last();

                displayedSubcontainersAmount = worker.workerspecs.unlockRecipes.Length;
                unlocks.PlaceContainers(requiredAmount: displayedSubcontainersAmount,
                                        containerWidth: unlocks[0].RT.rect.width,
                                        isHorizontalPlacement: true);
                unlocks.LoadContainers(loadData_IN: worker.workerspecs.unlockRecipes
                                            .Select(rcp => new ContentDisplayInfo_JustSprite(spriteRef_IN: rcp.receipeImageRef)),
                                       hideAtInit: true);
                break;
        }
        
    }

    public void DisplayContents()
    {
        if(_co is not null)
        {
            StopCoroutine(_co);
            _co = null;
        }
       
        _co = DisplayContentsRoutine();
        StartCoroutine(_co);
    }

    private IEnumerator DisplayContentsRoutine()
    {
        for (int i = 0; i < displayedSubcontainersAmount; i++)
        {
            if (i == 0) isSubContainersAnimating = true;

            unlocks[i].AnimateWithRoutine(customInitialValue: null,
                                          secondaryInterpolation: null,
                                          isVisible: true,
                                          lerpSpeedModifier: 1f,
                                          followingAction_IN: i == displayedSubcontainersAmount-1 
                                                                    ? () => isSubContainersAnimating = false
                                                                    : null);
            yield return null;
        }
        _co = null;
    }

    public void FastForwardDisplayAnimation()
    {
        if(_co is not null || isSubContainersAnimating)
        {
            StopCoroutine(_co);
            _co = null;
            isSubContainersAnimating = false;

            for (int i = 0; i < unlocks.Length; i++)
            {
                unlocks[i].ScaleDirect(isVisible:true, finalValueOperations:null);
            }
        }
    }

    public void UnloadAndDeallocate()
    {
        characterType_Text.Unload();
        GUI_CentralPlacement.DeactivateUnusedContainers(0, unlocks);
    }


    /*private void HideContainers()
    {
        characterType_Text.ScaleDirect(isVisible: false, finalValueOperations:null);
        characterProfession_Text.alpha = 0f;
        unlocks.HideContainers();
    }*/


}
