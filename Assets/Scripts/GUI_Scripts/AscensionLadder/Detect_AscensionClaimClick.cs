using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_AscensionClaimClick : DetectClickRequest<AscensionRewardState>
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        if (isValidClick && (Vector2.Distance(initialClickPosition, eventData.position) <= PanelManager.MAXCLICKOFFSET))
        {
            if (initialSelection is LadderContainer ladderContainerSelection)
            {
                ladderContainerSelection.Tintsize();
                Debug.Log("selectedrecipe is " + RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.productType);
            
                var isPreviousAscensionClaimed = !AscensionTreeManager.Instance.QueryAscensionRewardState(RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.productType).rewardsAndStates
                    .Any(pat => pat.reward.ascensionsNeeded < ladderContainerSelection.bluePrint.reward.ascensionsNeeded && !pat.reward.isPremiumReward && !pat.IsClaimed);
                               
                if (ladderContainerSelection.bluePrint.isUnlocked
                    && !ladderContainerSelection.bluePrint.IsClaimed
                    && isPreviousAscensionClaimed)
                {
                    ladderContainerSelection.bluePrint.ClaimAscension();
                    ladderContainerSelection.SetContainerColor();
                    Debug.Log(ladderContainerSelection.bluePrint.reward.ascensionTreeRewardType);
                }
                else if (!ladderContainerSelection.bluePrint.isUnlocked) Debug.Log("this acension reward is not unlocked yet");
                else if (!isPreviousAscensionClaimed) Debug.Log(" claim the previous ascensions first ");
                else Debug.Log("this acension reward is already claimed!");
            }
                

        }
    }
}
