using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using UnityEngine;

public class AscensionTreeManager : MonoBehaviour
{
    #region Singleton Syntax

    private static AscensionTreeManager _instance;
    public static AscensionTreeManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    [SerializeField] private AscensionTreeList_SO ascensionTreeList_SO;
    private static Dictionary<ProductType.Type, AscensionTreeRewardState> _ascensionTreeRewardsOfAllProducts;
    public const int ascensionAmountInterval = 3;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this; // LATER TO ADD DONTDESTROY ON LOAD
                    DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }
    private void Start()
    {
        // This Configure has to be taken up LATER, now its receiveing no dictionary but it should actually LOAD the SAVED dictionary
        Configure();
        CheckAscensionIntervals();
    }

    public void Update() // this is for TESTING PURPOSES LATER TO DELETE 
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            if(RecipeInfoPanel_Manager.Instance.SelectedRecipe != null) 
            {
                ProcessNewAscensionTreeStatus(RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.productType, 1);
                Debug.Log($"acension amount added 1 for  {RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.productType}");
            }
        }
        if(Input.GetKeyDown(KeyCode.H))
        {           
            if (RecipeInfoPanel_Manager.Instance.SelectedRecipe != null) 
            {
                ProcessNewAscensionTreeStatus(RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.productType, 3);
                Debug.Log($"acension amount added 3 for  {RecipeInfoPanel_Manager.Instance.SelectedRecipe.recipeSpecs.productType}");
            }
        }
    }

    private void Configure(Dictionary<ProductType.Type, AscensionTreeRewardState> ascensionTreeRewardsOfAllProducts_IN = null)
    {
        _ascensionTreeRewardsOfAllProducts = ascensionTreeRewardsOfAllProducts_IN ?? new Dictionary<ProductType.Type, AscensionTreeRewardState>();
    }

    private void CheckAscensionIntervals()
    {
        foreach (var ascensionTree in ascensionTreeList_SO.listOfAscensionTrees)
        {
            for (int i = 0; i < ascensionTree.ascensionTreeRewards.Length; i++)
            {            
               if(!ascensionTree.ascensionTreeRewards[i].isPremiumReward && !(ascensionTree.ascensionTreeRewards[i].ascensionsNeeded - (i > 0 ? ascensionTree.ascensionTreeRewards[i - 1].ascensionsNeeded : 0) == ascensionAmountInterval)
                  ||  ascensionTree.ascensionTreeRewards[i].isPremiumReward && !(ascensionTree.ascensionTreeRewards[i].ascensionsNeeded == ascensionTree.ascensionTreeRewards[i-1].ascensionsNeeded))             
                {
                    Debug.LogError("ascension intervals are not correct");
                }
            }
        }
    }


    public void ProcessNewAscensionTreeStatus(ProductType.Type productType_IN, int amountToAdd_IN)
    {
        var newRewardState = GetNewRewardState(productType_IN, amountToAdd_IN);
        RegisterNewRewardState(productType_IN: productType_IN,
                               newascensionAmount: newRewardState.newAmount,
                               newRewardStates_IN: newRewardState.newRewardsAndStates);
    }

    private (int newAmount, IEnumerable<AscensionRewardState> newRewardsAndStates) GetNewRewardState(ProductType.Type productType_IN, int amountToAdd_IN)
    {
        (int newAmount, IEnumerable<AscensionRewardState> newRewardsAndStates) retVal;
        retVal.newAmount = _ascensionTreeRewardsOfAllProducts.TryGetValue(productType_IN, out AscensionTreeRewardState ascensionTreeRewardState)
                        ? ascensionTreeRewardState.currentAscensionAmount + amountToAdd_IN
                        : amountToAdd_IN;


        retVal.newRewardsAndStates = ascensionTreeRewardState is not null
                        ? ascensionTreeRewardState.rewardsAndStates
                                            .Select(rs => rs.isUnlocked || (!rs.isUnlocked && rs.reward.ascensionsNeeded > retVal.newAmount)
                                                            ? rs
                                                            : new AscensionRewardState(reward_IN: rs.reward, isUnlocked_IN: true, isClaimed_IN: false))// (rs.reward, isUnlocked: true, isClaimed: false))
                        : ascensionTreeList_SO.listOfAscensionTrees
                                            .First(rew => rew.productType == productType_IN).ascensionTreeRewards
                                            .Select(reward => new AscensionRewardState(reward_IN: reward, isUnlocked_IN: false, isClaimed_IN: false)); // (reward, false, false));

        return retVal;
    }


    private void RegisterNewRewardState(ProductType.Type productType_IN, int newascensionAmount, IEnumerable<AscensionRewardState> newRewardStates_IN)
    {
        _ascensionTreeRewardsOfAllProducts[productType_IN] = new AscensionTreeRewardState(
                                                               newAscensionAmount_IN: newascensionAmount,
                                                               newAscensionTreeRewardStates_IN: newRewardStates_IN);       
    }

    public AscensionTreeRewardState QueryAscensionRewardState(ProductType.Type productType_IN)
    {
        if(!_ascensionTreeRewardsOfAllProducts.ContainsKey(productType_IN))
        {
            ProcessNewAscensionTreeStatus(productType_IN, amountToAdd_IN: 0);
        }
        return _ascensionTreeRewardsOfAllProducts[productType_IN];     
    }

    //public bool IsPreviousAscensionClaimed(ProductType.Type productType_IN, int queriedAscensionValue_IN)
    //{
    //    var productAscensionTree = QueryAscensionRewardState(productType_IN);

    //    return !productAscensionTree.rewardsAndStates.Any(pat => pat.reward.ascensionsNeeded < queriedAscensionValue_IN && pat.IsClaimed == false);
    //}

}
