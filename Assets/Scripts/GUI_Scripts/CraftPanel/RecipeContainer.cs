using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class RecipeContainer : RecipeContainer_Small // Container<ProductRecipe>
{
    //public  RectTransform rt { get { return _rect; } }
    //[SerializeField] private RectTransform _rect;
    //public TextMeshProUGUI recipeName { get { return _recipeName; } }
    //[SerializeField] private TextMeshProUGUI _recipeName;
    public TextMeshProUGUI recipeLevel { get { return _recipeLevel; } }
    [SerializeField] private TextMeshProUGUI _recipeLevel;


    [SerializeField] private Image currentlyCraftedBG;
    [SerializeField] private Image currentlyCraftedFG;
    [SerializeField] private TextMeshProUGUI currentlyCraftedAmountText;           // daha sonra loaddan alýnacak þekilde olacak

    public float subContainerWidth { get { return contentDisplayIngredients[0].RT.rect.width; } }

    [SerializeField] private ContentDisplayIngredient[] contentDisplayIngredients;
    [SerializeField] private ContentDisplayAdditionalItems[] contentDisplayAdditionalItems;
    [SerializeField] private ContentDisplayWorker[] contentDisplayWorkers;

    [SerializeField] private TextMeshProUGUI masteryCurrentAmount;
    [SerializeField] private TextMeshProUGUI masteryNextLevelAmount;
    [SerializeField] private Image masteryLevelBarBG;
    [SerializeField] private Image masteryLevelBarFG;

    [SerializeField] private Image[] masteryStars;
    [SerializeField] private Color masteryStarInactiveColor;
    [SerializeField] private Color masteryStarActiveColor;

    //[SerializeField] private TextMeshProUGUI amountInInventory;
    [SerializeField] private TextMeshProUGUI recipeDescription;
    private string[] descriptionTexts = new string[] { "Craft ", " More ", "Unlocked From " };
    private StringBuilder sb = new StringBuilder(); // Later to make 1 Static method to also include Tabpanel Stats !!!!



    private void Update() // need to replace this with subscription or better equalisation ..
    {
        if(Time.frameCount % TimeTickSystem.UPDATEINTERVAL == 0)
        {
            if (bluePrint != null && bluePrint.IsUnlocked() != false && bluePrint.IsResearched() != false) MatchCurrentlyCraftedState();   // at least when its unlocked or not researched Disable
        } 
    }
    public override void LoadContainer(ProductRecipe newRecipeIN)
    {
        _recipeName.text = newRecipeIN.GetName() + "     " + ISpendable.ToScreenFormat(newRecipeIN.GetValue());//.ToString(); // remove value THÝS ÝS FOR DEBUGG
        recipeLevel.text = newRecipeIN.GetLevel().ToString() ;

        mainImageContainer.LoadSprite(newRecipeIN.GetAdressableImage());

        bluePrint = newRecipeIN;

        if (newRecipeIN.IsUnlocked() == false)
        {
            if (containerImage.color != Color.magenta) containerImage.color = Color.magenta;
            if (amountInInventory.enabled != false) amountInInventory.enabled = false;
            if (contentDisplayIngredients[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayIngredients);
            if (contentDisplayAdditionalItems[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayAdditionalItems);
            if (contentDisplayWorkers[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayWorkers);
            if (currentlyCraftedBG.enabled == true || currentlyCraftedFG.enabled == true || currentlyCraftedAmountText.enabled == true) currentlyCraftedBG.enabled = currentlyCraftedFG.enabled = currentlyCraftedAmountText.enabled = false;
            if (recipeDescription.enabled == false) recipeDescription.enabled = true;


            MatchMasteryStarStates();

            if (bluePrint.recipeSpecs.unlockPrerequisite.Length == 0)
            {
                if (RecipeManager.recipesRequiredToUnlock_Dict.ContainsKey(bluePrint))
                {
                    var requiredRecipeInfoTuple = RecipeManager.recipesRequiredToUnlock_Dict[bluePrint];

                    SetRemainingLockedCraftInfo(requiredRecipeInfoTuple);
                    SetRecipeDescription(requiredRecipeInfoTuple);
                    if (masteryLevelBarBG.enabled == false || masteryLevelBarFG.enabled == false || masteryCurrentAmount.enabled == false || masteryNextLevelAmount.enabled == false) masteryLevelBarBG.enabled = masteryLevelBarFG.enabled = masteryCurrentAmount.enabled = masteryNextLevelAmount.enabled = true;
                }
            }

            else if (bluePrint.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.ChestLoot)
            {
                SetRecipeDescription(bluePrint.recipeSpecs.unlockPrerequisite[0].chest.ToString());
                if (masteryLevelBarBG.enabled == true || masteryLevelBarFG.enabled == true || masteryCurrentAmount.enabled == true || masteryNextLevelAmount.enabled == true) masteryLevelBarBG.enabled = masteryLevelBarFG.enabled = masteryCurrentAmount.enabled = masteryNextLevelAmount.enabled = false;
            }
            else if (bluePrint.recipeSpecs.unlockPrerequisite[0].unlockPrerequisiteType == Recipes_SO.UnlockPrerequisiteType.RequiredWorker)
            {
                SetRecipeDescription(bluePrint.recipeSpecs.unlockPrerequisite[0].requiredworkers.requiredWorker[0].requiredWorker.ToString());
                if (masteryLevelBarBG.enabled == true || masteryLevelBarFG.enabled == true || masteryCurrentAmount.enabled == true || masteryNextLevelAmount.enabled == true) masteryLevelBarBG.enabled = masteryLevelBarFG.enabled = masteryCurrentAmount.enabled = masteryNextLevelAmount.enabled = false;
            }

            return;
        }
        else if (newRecipeIN.IsUnlocked() == true && newRecipeIN.IsResearched() == false)
        {
            if (containerImage.color != Color.blue) containerImage.color = Color.blue;
            if (amountInInventory.enabled != false) amountInInventory.enabled = false;
            if (currentlyCraftedBG.enabled == true || currentlyCraftedFG.enabled == true || currentlyCraftedAmountText.enabled == true) currentlyCraftedBG.enabled = currentlyCraftedFG.enabled = currentlyCraftedAmountText.enabled = false;
            if (recipeDescription.enabled == true) recipeDescription.enabled = false;
            if (masteryLevelBarBG.enabled == false || masteryLevelBarFG.enabled == false || masteryCurrentAmount.enabled == false || masteryNextLevelAmount.enabled == false) masteryLevelBarBG.enabled = masteryLevelBarFG.enabled = masteryCurrentAmount.enabled = masteryNextLevelAmount.enabled = true;
            if (contentDisplayWorkers[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayWorkers);

            MatchMasteryStarStates();
            SetResearchLevelInfo();
        }
        else if (newRecipeIN.IsUnlocked() && bluePrint.AreWorkerRequirementsMet.Any(awm => !awm))
        {
            /*Debug.Log("getmissing workers is empty enumerable ? : " + GetMissingWorkers().Any());
            foreach (var item in GetMissingWorkers())
            {
                Debug.Log(" is hired ? : " + item.Item1?.isHired + " | Required Worker Name: " + item.Item2.requiredWorker + " | Workerlevel " + item.Item1?.GetLevel() + " | Required Level : " + item.Item2.requiredWorkerLevel);
            }*/
            if (containerImage.color != Color.grey) containerImage.color = Color.grey;
            if (amountInInventory.enabled != false) amountInInventory.enabled = false;
            if (currentlyCraftedBG.enabled == true || currentlyCraftedFG.enabled == true || currentlyCraftedAmountText.enabled == true) currentlyCraftedBG.enabled = currentlyCraftedFG.enabled = currentlyCraftedAmountText.enabled = false;
            if (contentDisplayIngredients[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayIngredients);
            if (contentDisplayAdditionalItems[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayAdditionalItems);
            if (recipeDescription.enabled == true) recipeDescription.enabled = false;
            if (masteryLevelBarBG.enabled == false || masteryLevelBarFG.enabled == false || masteryCurrentAmount.enabled == false || masteryNextLevelAmount.enabled == false) masteryLevelBarBG.enabled = masteryLevelBarFG.enabled = masteryCurrentAmount.enabled = masteryNextLevelAmount.enabled = true;

            SetWorkerRequirementInfo();
            contentDisplayWorkers.PlaceContainers(newRecipeIN.recipeSpecs.requiredworkers.Length, CraftPanel_Manager.SubContainerWidth, isHorizontalPlacement: true, crossPosition_IN: CraftPanel_Manager.floating_YPpoints.upperPoint);
            contentDisplayWorkers.LoadContainers(newRecipeIN, newRecipeIN.recipeSpecs.requiredworkers.Length, hideAtInit: false);

            return;
        }

       

        else if (newRecipeIN.IsUnlocked() == true && newRecipeIN.IsResearched() == true
                    && newRecipeIN.recipeSpecs.requiredworkers.All(rqw => CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker].
                                                                                        Any(chr => chr.isHired && ((Worker)chr).workerspecs.workerType == rqw.requiredWorker)))
        {
            if (containerImage.color != Color.white) containerImage.color = Color.white;
            if (amountInInventory.enabled != true) amountInInventory.enabled = true;
            if (recipeDescription.enabled == true) recipeDescription.enabled = false;
            if (masteryLevelBarBG.enabled == false || masteryLevelBarFG.enabled == false || masteryCurrentAmount.enabled == false || masteryNextLevelAmount.enabled == false) masteryLevelBarBG.enabled = masteryLevelBarFG.enabled = masteryCurrentAmount.enabled = masteryNextLevelAmount.enabled = true;
            if (contentDisplayWorkers[0].gameObject.activeSelf == true) GUI_CentralPlacement.DeactivateUnusedContainers(requiredAmount_IN: 0, contentDisplayWorkers);

            amountInInventory.text = Inventory.Instance.CheckAmountInInventory_ByNameDict(newRecipeIN.GetName(), out _).ToString();

            MatchMasteryStarStates();
            SetMasteryLevelInfo();
        }

        contentDisplayIngredients.PlaceContainers(newRecipeIN.recipeSpecs.requiredIngredients.Length, CraftPanel_Manager.SubContainerWidth, isHorizontalPlacement: true, crossPosition_IN: CraftPanel_Manager.floating_YPpoints.upperPoint);
        contentDisplayIngredients.LoadContainers(newRecipeIN, newRecipeIN.recipeSpecs.requiredIngredients.Length, hideAtInit:false);

        if (newRecipeIN.recipeSpecs.requiredAdditionalItems.Length > 0)
        {
            contentDisplayAdditionalItems.PlaceContainers(newRecipeIN.recipeSpecs.requiredAdditionalItems.Length, CraftPanel_Manager.SubContainerWidth, isHorizontalPlacement: true, crossPosition_IN: CraftPanel_Manager.floating_YPpoints.lowerPoint);
            contentDisplayAdditionalItems.LoadContainers(newRecipeIN, newRecipeIN.recipeSpecs.requiredAdditionalItems.Length, hideAtInit: false);
        }
        else
        {           
            GUI_CentralPlacement.DeactivateUnusedContainers(0, contentDisplayAdditionalItems);
        }

        MatchContainerDynamicInfo();
    }
    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();

        _recipeName.text = null;
        recipeLevel.text = null;

        GUI_CentralPlacement.DeactivateUnusedContainers(0, contentDisplayIngredients);
        GUI_CentralPlacement.DeactivateUnusedContainers(0, contentDisplayAdditionalItems);
        GUI_CentralPlacement.DeactivateUnusedContainers(0, contentDisplayWorkers);
    }

    /*private IEnumerable<(Worker,Recipes_SO.RequiredWorker)> GetMissingWorkers() // this is not necessary here 
    {
        
        var doesAnyWorkerExists = collection2.Count > 0;

        foreach (var requiredworker in collection1)
        {
            if (!doesAnyWorkerExists) yield return (null,requiredworker);

            for (int i = 0; i < collection2.Count; i++)
            {
                if (((Worker)collection2[i]).workerspecs.workerType == requiredworker.requiredWorker && collection2[i].isHired && collection2[i].GetLevel() >= requiredworker.requiredWorkerLevel)
                {
                    continue;
                }
                else if  (((Worker)collection2[i]).workerspecs.workerType == requiredworker.requiredWorker && (!collection2[i].isHired || collection2[i].GetLevel() < requiredworker.requiredWorkerLevel))
                {
                    yield return (collection2[i] as Worker, requiredworker);
                    continue;
                }
                else if (i == collection2.Count-1 && ((Worker)collection2[i]).workerspecs.workerType != requiredworker.requiredWorker)
                {
                    yield return (null, requiredworker);
                }      
            }          
        }
    }*/


    private void SetWorkerRequirementInfo()
    {
        /*masteryCurrentAmount.text = requirements.Aggregate("Requires", (prev, req) => $" {req.workerName} Lvl.{req.workerLevel}");
        masteryCurrentAmount.text = missingWorkers.Select(mwTuple => mwTuple.Item1 is null
                                                                                    ? $"Unlock {mwTuple.Item2.requiredWorker}"
                                                                                    : !mwTuple.Item1.isHired
                                                                                            ? $"Hire {mwTuple.Item2.requiredWorker}"
                                                                                            : $"Requires Lvl.{mwTuple.Item2.requiredWorkerLevel} {mwTuple.Item2.requiredWorker}", ));
        */
       
        masteryLevelBarFG.fillAmount = 0f;
        masteryCurrentAmount.text = string.Empty;
            /*missingWorkers.OrderBy(mw => mw.Item2.requiredWorkerLevel)
                                                  .All(mw => mw.Item1 is null)
                                                            ? $" Unlock {missingWorkers.First().Item2.requiredWorker}"
                                                            : missingWorkers.First().Item1.isHired == false
                                                                    ? $"Hire {missingWorkers.First().Item2.requiredWorker}"
                                                                    : $"Requires Lvl.{missingWorkers.First().Item2.requiredWorkerLevel} {missingWorkers.First().Item2.requiredWorker}";*/
    }


    private  void SetResearchLevelInfo()
    {
        var researchScrollsOwned = Inventory.Instance.CheckAmountInInventory_Name(SpecialItemsManager.Instance.Keys_Shards_Scrolls_SO.researchScrollInfo.name, GameItemType.Type.SpecialItem);
        var researchScrollsRequired = bluePrint.recipeSpecs.researchPointsRequired;
        masteryCurrentAmount.text = researchScrollsOwned.ToString();
        masteryNextLevelAmount.text = researchScrollsRequired.ToString();
        masteryLevelBarFG.fillAmount = CalculateFillAmount.CalculateFill(researchScrollsOwned, researchScrollsRequired);
    }

    private void SetMasteryLevelInfo()
    {
        if ((int)bluePrint.masteryLevel < bluePrint.recipeSpecs.craftingUpgrades.Length)
        {
            if (masteryNextLevelAmount.enabled == false)
            {
                masteryNextLevelAmount.enabled = masteryLevelBarFG.enabled = true;
            }

            var masteryAmountCraftedInt = bluePrint.amountCraftedLocal;
            var masteryNextLevelAmountInt = bluePrint.recipeSpecs.craftingUpgrades[(int)bluePrint.masteryLevel].craftsNeeded;
            masteryCurrentAmount.text = masteryAmountCraftedInt.ToString();
            masteryNextLevelAmount.text = masteryNextLevelAmountInt.ToString();
            masteryLevelBarFG.fillAmount = CalculateFillAmount.CalculateFill(masteryAmountCraftedInt, masteryNextLevelAmountInt);
        }
        else
        {
            if (masteryNextLevelAmount.enabled == true)
            {
                masteryNextLevelAmount.enabled = masteryLevelBarFG.enabled = false;
            }
            masteryCurrentAmount.text = "fully mastered";
        }

    }
    private void SetRecipeDescription((ProductRecipe requiredRecipe, int masteryLevel) requiredRecipeInfo)
    {
        var totalRequiredAmount = requiredRecipeInfo.requiredRecipe.GetTotalRequiredAmount(requiredRecipeInfo.masteryLevel);
        recipeDescription.text = BuildString(descriptionTexts[0], (totalRequiredAmount - (requiredRecipeInfo.requiredRecipe.amountCraftedGlobal)).ToString(), descriptionTexts[1], requiredRecipeInfo.requiredRecipe.GetName());
    }

    private void SetRecipeDescription(string requirementName)
    {
        recipeDescription.text = BuildString(descriptionTexts[2], requirementName);
    }
    private void SetRemainingLockedCraftInfo((ProductRecipe requiredRecipe, int masteryLevel) requiredRecipeInfo)
    {
        masteryCurrentAmount.text = requiredRecipeInfo.requiredRecipe.amountCraftedGlobal.ToString();
        masteryNextLevelAmount.text = requiredRecipeInfo.requiredRecipe.GetTotalRequiredAmount(requiredRecipeInfo.masteryLevel).ToString();
        masteryLevelBarFG.fillAmount = CalculateFillAmount.CalculateFill(requiredRecipeInfo.requiredRecipe.amountCraftedGlobal, requiredRecipeInfo.requiredRecipe.GetTotalRequiredAmount(requiredRecipeInfo.masteryLevel));
    }



    public override void MatchContainerDynamicInfo()
    {
        MatchCurrentlyCraftedState();
    }

    private void MatchCurrentlyCraftedState()
    {
        if (bluePrint.itemHolderSlotsList.Count == 0 && currentlyCraftedBG.enabled == false)
        {
            return;
        }
        else if (bluePrint.itemHolderSlotsList.Count == 0 && currentlyCraftedBG.enabled == true)
        {
            currentlyCraftedBG.enabled = currentlyCraftedFG.enabled = currentlyCraftedAmountText.enabled = false;
            return;
        }
        else if (bluePrint.itemHolderSlotsList.Count > 0)
        {

            if (currentlyCraftedBG.enabled == false)
            {
                currentlyCraftedBG.enabled = currentlyCraftedFG.enabled = currentlyCraftedAmountText.enabled = true;
            }
            if (bluePrint.itemHolderSlotsList.Count != int.Parse(currentlyCraftedAmountText.text))
            {
                currentlyCraftedAmountText.text = bluePrint.itemHolderSlotsList.Count.ToString();
            }
            currentlyCraftedFG.fillAmount = bluePrint.itemHolderSlotsList[0].ProgressImage.fillAmount;
            if (bluePrint.itemHolderSlotsList[0].IsReadyToReclaim)
            {
                SwitchToTheNextSlot();
            }
        }
    }

    private void MatchMasteryStarStates()
    {
        if (bluePrint.IsUnlocked() == false || bluePrint.IsResearched() == false)
        {
            if (masteryStars[0].gameObject.activeSelf == true)
            {
                for (int i = 0; i < masteryStars.Length; i++)
                {
                    masteryStars[i].gameObject.SetActive(false);
                }
            }
            return;
        }
        else
        {
            if (masteryStars[0].gameObject.activeSelf == false)
            {
                for (int i = 0; i < masteryStars.Length; i++)
                {
                    masteryStars[i].gameObject.SetActive(true);
                }
            }
        }

        int masteryLevel = (int)bluePrint.masteryLevel;
        for (int i = 0; i < masteryStars.Length; i++)
        {
            if (i < masteryLevel)
            {
                if (masteryStars[i].color == masteryStarInactiveColor)
                {
                    masteryStars[i].color = masteryStarActiveColor;
                }
                else if (masteryStars[i].color == masteryStarActiveColor)
                {
                    continue;
                }
            }
            else if (i >= masteryLevel)
            {
                if (masteryStars[i].color == masteryStarInactiveColor)
                {
                    continue;
                }
                else if (masteryStars[i].color == masteryStarActiveColor)
                {
                    masteryStars[i].color = masteryStarInactiveColor;
                }
            }
        }
    }

    private void SwitchToTheNextSlot()
    {
        bluePrint.itemHolderSlotsList.RemoveAt(0);
        currentlyCraftedAmountText.text = bluePrint.itemHolderSlotsList.Count.ToString();
        if (bluePrint.itemHolderSlotsList.Count == 0)
        {
            currentlyCraftedBG.enabled = currentlyCraftedFG.enabled = currentlyCraftedAmountText.enabled = false;
        }
    }

    private string BuildString(string strConst1, string strAmount, string strConst2, string strValue)
    {
        sb.Clear();
        sb.Append(strConst1);
        sb.Append(strAmount);
        sb.Append(strConst2);
        sb.Append(strValue);

        return sb.ToString();
    }

    private string BuildString(string strConst1, string requirementName)
    {
        sb.Clear();
        sb.Append(strConst1);
        sb.Append(requirementName);

        return sb.ToString();
    }


}
