using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RecipeManager : MonoBehaviour  /// MAKE ITE SINGLEDTON ///
{
    #region Singleton Syntax

    private static RecipeManager _instance;
    public static RecipeManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion


    [SerializeField] private Recipe_List_SO recipeListSO;
    //public static Dictionary<EquipmentType.Type, List<Dictionary<ProductType.Type, List<ProductRecipe>>>> recipesAvailable_Dict { get; private set; }
    public static Dictionary<EquipmentType.Type, Dictionary<ProductType.Type, List<ProductRecipe>>> recipesAvailable_Dict { get; private set; }
    public static Dictionary<ProductRecipe, ProductRecipe> RecipesAvailableLookUp_Dict;
    public static List<ProductRecipe> RecipesAvailable_List { get; private set; }    

    public static Dictionary<ProductType.Type, List<ProductRecipe>> recipesLockedToDispay_Dict;   // try to eliminate those by returning an IEnumereable
    public static Dictionary<ProductRecipe, (ProductRecipe recipeRequired, int masteryLevel)> recipesRequiredToUnlock_Dict;

    [SerializeField] private CraftPanel_Manager craftPanel_Manager_Script;
    [SerializeField] private PanelManager panelManager_Script;

    private void Awake() // Later to take on config // pay attention that execute event of default dict request comes before this script starts
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
                }
            }
        }

        CreateRecipeDict();
        CreateRecipesAvailableLookUp_Dict();
        CreateRecipeList();
        CreateRecipeLockedDict();
        CreateRecipesRequiredToUnlock_Dict();
    }

    private void Update()   //FOR TEST PUTPOSES LATER TO DELETE 
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            var selectedRecipe = recipeListSO.listOfRecipes.Find(recipe => recipe.recipeName == "Classic Burger");
            AddNewRecipe(selectedRecipe);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            foreach (Recipes_SO recipe in recipeListSO.listOfRecipes)
            {
                AddNewRecipe(recipe);
            }
        }
    }



    private void CreateRecipeDict(Dictionary<EquipmentType.Type, Dictionary<ProductType.Type, List<ProductRecipe>>> recipesAvailable_DictIN = null)
    {
        recipesAvailable_Dict = recipesAvailable_DictIN ?? CreateBlankDictionary();
    }

    private void CreateRecipesAvailableLookUp_Dict(Dictionary<ProductRecipe, ProductRecipe> recipesAvailableLookUp_Dict_IN = null)
    {
        RecipesAvailableLookUp_Dict = recipesAvailableLookUp_Dict_IN ?? new Dictionary<ProductRecipe, ProductRecipe>(new RecipeEqualityComparer());
    }

    private void CreateRecipeList(List<ProductRecipe> recipesAvailable_ListIN = null)
    {
        RecipesAvailable_List = recipesAvailable_ListIN ?? new List<ProductRecipe>();
    }

    private void CreateRecipeLockedDict(Dictionary<ProductType.Type, List<ProductRecipe>> recipesLockedToDispay_DictIN = null)
    {
        recipesLockedToDispay_Dict = recipesLockedToDispay_DictIN ?? CreateBlankLockedItemsDict();
    }

    private void CreateRecipesRequiredToUnlock_Dict(Dictionary<ProductRecipe, (ProductRecipe recipeRequired, int masteryLevel)> recipesRequiredToUnlock_Dict_IN = null)
    {
        recipesRequiredToUnlock_Dict = recipesRequiredToUnlock_Dict_IN ?? new Dictionary<ProductRecipe, (ProductRecipe recipeRequired, int masteryLevel)>();
    }

    private Dictionary<EquipmentType.Type, Dictionary<ProductType.Type, List<ProductRecipe>>> CreateBlankDictionary()
    {
        var dictionarySize = recipeListSO.listOfRecipes.Count;
        //Debug.LogError(dictionarySize);
        var newDictionary = new Dictionary<EquipmentType.Type, Dictionary<ProductType.Type, List<ProductRecipe>>>(dictionarySize);

        foreach (int equipmentTypeEnumIndex in Enum.GetValues(typeof(EquipmentType.Type)))
        {
            switch (equipmentTypeEnumIndex)
            {
                case > 0:
                    var requiredEquipment = (EquipmentType.Type)equipmentTypeEnumIndex;
                    var innerDict = new Dictionary<ProductType.Type, List<ProductRecipe>>();

                    foreach (int productEnumIndex in Enum.GetValues(typeof(ProductType.Type)))
                    {
                        switch (IsBelongingType(equipmentTypeEnumIndex, productEnumIndex))
                        {
                            case true:
                                var requiredProduct = (ProductType.Type)productEnumIndex;
                                var innermostListSize = recipeListSO.listOfRecipes.Where(recipe => recipe.productType == requiredProduct).Count();
                                //Debug.LogError(innermostListSize + " inner list size of product: " + requiredProduct);
                                innerDict.Add(requiredProduct, new List<ProductRecipe>(innermostListSize));
                                break;
                            default:
                                break;
                        }
                    }
                    newDictionary.Add(requiredEquipment, innerDict);
                    break;
                default:
                    break;
            }
        }
        return newDictionary;
    }



    private Dictionary<ProductType.Type, List<ProductRecipe>> CreateBlankLockedItemsDict()
    {
        var newDict = new Dictionary<ProductType.Type, List<ProductRecipe>>();
        foreach (int productTypeEnumIndex in Enum.GetValues(typeof(ProductType.Type)))
        {
            if (productTypeEnumIndex == -100)
            {
                continue;
            }
            else
            {
                newDict.Add((ProductType.Type)productTypeEnumIndex, new List<ProductRecipe>());
            }
        }
        return newDict;
    }

    private bool IsBelongingType(int equipmentTypeEnumIndexIN, int productTypeEnumIndexIN)
    {
        if (equipmentTypeEnumIndexIN / 100 == productTypeEnumIndexIN / 100)
        {
            return true;
        }
        return false;
    }

    public void AddNewRecipe(Recipes_SO recipe_SO_IN)
    {
        var _innerListOfRecipeContainers = GetRootList(recipe_SO_IN.requiredEquipment, recipe_SO_IN.productType);

        foreach (ProductRecipe productRecipe in _innerListOfRecipeContainers)
        {
            if (productRecipe.recipeSpecs == recipe_SO_IN)
            {
                Debug.Log("this recipe already exists");
                return;
            }
        }

        ProductRecipe newRecipe = new ProductRecipe(recipe_SO_IN, isUnlockedIN: true, isResearchedIN: recipe_SO_IN.researchPointsRequired == 0 ? true : false);
        _innerListOfRecipeContainers.Insert(0, newRecipe);
        RecipesAvailable_List.Add(newRecipe);
        RecipesAvailableLookUp_Dict.Add(newRecipe, newRecipe);

        UpdateLockedItemsDict(newRecipe.recipeSpecs.requiredEquipment, newRecipe.recipeSpecs.productType);
    }

    /* public void AddNewRecipe(Recipes_SO recipe_SO_IN)
     {
         var _innerListOfRecipeContainers = GetRootList(recipe_SO_IN.requiredEquipment, recipe_SO_IN.productType);

         foreach (ProductRecipe productRecipe in _innerListOfRecipeContainers)
         {
             if (productRecipe.recipeSpecs == recipe_SO_IN)
             {
                 Debug.Log("this recipe already exists");
                 return;
             }
         }

         ProductRecipe newRecipe = new ProductRecipe(recipe_SO_IN, isUnlockedIN: true, isResearchedIN: recipe_SO_IN.researchPointsRequired == 0 ? true : false);
         _innerListOfRecipeContainers.Insert(0, newRecipe);
         RecipesAvailable_List.Add(newRecipe);
         RecipesAvailableLookUp_Dict.Add(newRecipe, newRecipe);

         UpdateLockedItemsDict(newRecipe.recipeSpecs.requiredEquipment, newRecipe.recipeSpecs.productType);
         Debug.Log("NEW RECIPE UNLOCKED ! " + newRecipe.GetName());
     }*/

    private List<ProductRecipe> GetRootList(EquipmentType.Type equipmentType, ProductType.Type productType)
    {
        if(recipesAvailable_Dict.TryGetValue(equipmentType,out Dictionary<ProductType.Type,List<ProductRecipe>> innerDict))
        {
            innerDict.TryGetValue(productType, out List<ProductRecipe> list);
            return list;
        }
        else
        {         
            Debug.LogWarning("should be here");
            return null;
        }

    }
    /*
    private List<ProductRecipe> GetRootList(EquipmentType.Type equipmentType, ProductType.Type productType)
    {
        var _innerList = recipesAvailable_Dict[equipmentType];

        foreach (var _innerDict in _innerList)
        {
            if (_innerDict.TryGetValue(productType, out List<ProductRecipe> recipeRootList))    // _innerDict.ContainsKey(productType))
            {
                return recipeRootList;
            }
        }
        return null;

    }*/

    public ProductRecipe GetRootRecipe(ICraftable gameItem_In)
    {
        if(RecipesAvailableLookUp_Dict.TryGetValue(gameItem_In.GetProductRecipe(), out ProductRecipe productRecipe_Existing))
        {
            return productRecipe_Existing;
        }
        return null;
    }

   /* public List<ProductRecipe> GetRootList_W_LockedItems(EquipmentType.Type equipmentType, ProductType.Type productType)
    {
        var newList = new List<ProductRecipe>(GetRootList(equipmentType, productType));

            //newList= GetRootList( equipmentType,productType);
        newList.InsertRange(0, recipesLockedToDispay_Dict[productType]);

        return newList;

    }*/

    public IEnumerable<ProductRecipe> GetRootList_W_LockedItems(EquipmentType.Type equipmentType, ProductType.Type productType)
    {
        var listOfExistingProducts = GetRootList(equipmentType, productType);
        var listOfLockedItemsToDisplay = recipesLockedToDispay_Dict[productType];

        foreach (var lockedItemToDisplay in listOfLockedItemsToDisplay)
        {
            yield return lockedItemToDisplay;
        }
        
        foreach (var existingProduct in listOfExistingProducts)
        {
            yield return existingProduct;
        }
        //newList= GetRootList( equipmentType,productType);
        //newList.InsertRange(0, recipesLockedToDispay_Dict[productType]);

       //return newList;

    }

    private void UpdateLockedItemsDict(EquipmentType.Type equipmentType, ProductType.Type productType)
    {
        var rootLlist = GetRootList(equipmentType, productType);
        int maxLevel = rootLlist.Count > 0 ? rootLlist.Max(recipe => recipe.recipeSpecs.productLevel) : 1;

        recipesLockedToDispay_Dict[productType].Clear();

        foreach (Recipes_SO recipe_SO in recipeListSO.listOfRecipes)
        {
            if (recipe_SO.productLevel <= maxLevel + 1 && recipe_SO.productType == productType && !rootLlist.Any(x => x.recipeSpecs.Equals(recipe_SO)))              
            {
                ProductRecipe lockedRecipe = new ProductRecipe(recipe_SO, isUnlockedIN: false);
                recipesLockedToDispay_Dict[productType].Add(lockedRecipe);
                MapRecipesRequiredToUnlock_Dict(lockedRecipe);
            }
        }

        foreach (var worker in CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker] )
        {
            UpdateLockedItemsDict(worker as Worker);
        }
    }

    public void UpdateLockedItemsDict(Worker worker)
    {
        for (int i = 0; i < worker.workerspecs.unlockRecipes.Length; i++)
        {
            var workerUnlockedRecipe = worker.workerspecs.unlockRecipes[i];
            switch (worker.isHired)
            {
                case false when recipesLockedToDispay_Dict[workerUnlockedRecipe.productType].All(rld=> rld.recipeSpecs != workerUnlockedRecipe)
                             && GetRootList(workerUnlockedRecipe.requiredEquipment, workerUnlockedRecipe.productType).All(er => er.recipeSpecs != workerUnlockedRecipe) :
                    ProductRecipe lockedRecipe = new ProductRecipe(workerUnlockedRecipe, isUnlockedIN: false);
                    recipesLockedToDispay_Dict[workerUnlockedRecipe.productType].Add(lockedRecipe);
                    break;

                case true :
                    for (int j = 0; j < recipesLockedToDispay_Dict[workerUnlockedRecipe.productType].Count; j++)
                    {
                        if (recipesLockedToDispay_Dict[workerUnlockedRecipe.productType][j].recipeSpecs == workerUnlockedRecipe)
                        {
                            recipesLockedToDispay_Dict[workerUnlockedRecipe.productType].RemoveAt(j);
                        }
                    };
                    break;
            }
        }

        Debug.Log("this is working for " + worker.GetName());
    }

    private void MapRecipesRequiredToUnlock_Dict(ProductRecipe lockedRecipe)
    {
        foreach (ProductRecipe existingRecipe in RecipesAvailable_List)
        {
            for (int i = 0; i < existingRecipe.recipeSpecs.craftingUpgrades.Length; i++)
            {
                if (existingRecipe.recipeSpecs.craftingUpgrades[i].craftUpgradeType == Recipes_SO.CraftUpgradeType.UnlockRecipe && existingRecipe.recipeSpecs.craftingUpgrades[i].unlockRecipe == lockedRecipe.recipeSpecs)
                {
                    recipesRequiredToUnlock_Dict.Add(lockedRecipe, (existingRecipe, i));
                }
            }
        }
    }



}
