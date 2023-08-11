
using System;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour
{

    #region Singleton Syntax

    private static ResourcesManager _instance;
    public static ResourcesManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    public Resources_SO Resources_SO { get { return _resources_SO; } }
    [SerializeField] private Resources_SO _resources_SO;

    public IngredientGenerators_SO IngredientGenerators_SO { get { return _ingredientGenerators_SO; } }
    [SerializeField] private IngredientGenerators_SO _ingredientGenerators_SO;

    private static Dictionary<IngredientType.Type, Ingredient> ingredientsDict; 
    public static event Action<bool, IngredientType.Type> onCapReached;

    #endregion

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
        //Config();
    }

    private void Start()
    {
        Config(); // LATER TO TAKE UP! !! AND ARRANGE IT TO BE CONFIGURABLE BY LOAD OR NORMAL START 
    }

    public void Config(Dictionary<IngredientType.Type, Ingredient> ingredientsDict_IN = null)
    {
        //ingredientsDict = ingredientsDict_IN ?? InitEmptyResourcesDict();
        ingredientsDict = ingredientsDict_IN ?? InitEmptyResourcesDict();
    }

    private Dictionary<IngredientType.Type, Ingredient> InitEmptyResourcesDict()
    {
        var newDict = new Dictionary<IngredientType.Type, Ingredient>
        {
            {IngredientType.Type.Meat, new Ingredient(IngredientType.Type.Meat)},
            {IngredientType.Type.Flour_Grain, new Ingredient(IngredientType.Type.Flour_Grain)},
            {IngredientType.Type.Veggie, new Ingredient(IngredientType.Type.Veggie)},
            {IngredientType.Type.Seafood, new Ingredient(IngredientType.Type.Seafood)},
            {IngredientType.Type.Fruit, new Ingredient(IngredientType.Type.Fruit)},
            {IngredientType.Type.Spices, new Ingredient(IngredientType.Type.Spices)},
            {IngredientType.Type.Dairy, new Ingredient(IngredientType.Type.Dairy)},
            {IngredientType.Type.Fats_Oils, new Ingredient(IngredientType.Type.Fats_Oils)},
        };

        return newDict;
    }

    public static Ingredient FindIngredient(IngredientType.Type ingredientType_In)
    {
        return ingredientsDict.TryGetValue(ingredientType_In, out Ingredient ingredient)
                        ? ingredient
                        : null;
    }


    public static int CheckAmountOfIngredient(IngredientType.Type ingredientType_In, out Ingredient ingredient_Out)
    {
        int amount = 0;
        ingredient_Out = null;

        if(ingredientsDict.TryGetValue(ingredientType_In, out Ingredient ingredient))
        {
            amount = ingredient.GetAmount();
            ingredient_Out = ingredient;
        }
        return amount;
    }

    public void EvaluateMaxCapReached(Ingredient ingredient_IN)
    {
        onCapReached?.Invoke(ingredient_IN.IsMaxCapReached(), ingredient_IN.IngredientType);
    }

    public void AddIngredient(IngredientType.Type ingredientType_IN, int amount_IN, bool bypassMaxCap)
    {
        var ingredient = ingredientsDict[ingredientType_IN];
        if (!bypassMaxCap && ingredient.IsMaxCapReached())   
        {
            return;
        }
        else
        {
            ingredient.SetAmount(amount_IN);
            ingredientEventMapping[ingredientType_IN][0]?.Invoke();

            if (ingredient.IsMaxCapReached())
            {
                onCapReached?.Invoke(true, ingredientType_IN);
            }
        }
    }

    public bool RemoveIngredient(IngredientType.Type ingredientType_IN, int amount_IN)
    {
        var ingredient = ingredientsDict[ingredientType_IN];
        if(ingredient.GetAmount() >= amount_IN)
        {
            var wasCapReached = ingredient.IsMaxCapReached();
            ingredient.SetAmount(-amount_IN);
            ingredientEventMapping[ingredientType_IN][1]?.Invoke();

            if(wasCapReached && !ingredient.IsMaxCapReached())
            {
                onCapReached?.Invoke(false, ingredientType_IN);
            }
            return true;
        }
        else
        {
            Debug.Log("you dont have enough ingredients yet");
            return false;
        }
    }

    public static event Action onMeatAdded, onMeatRemoved;
    public static event Action onFlourGrainAdded, onFlourGrainRemoved;
    public static event Action onVeggieAdded, onVeggieRemoved;
    public static event Action onSeafoodAdded, onSeafoodRemoved;
    public static event Action onFruitAdded, onFruitRemoved;
    public static event Action onSpiceAdded, onSpicesRemoved;
    public static event Action onDairyAdded, onDairyRemoved;
    public static event Action onFatsOilsAdded, onFatsOilsRemoved;



    public static readonly Dictionary<IngredientType.Type, Action[]> ingredientEventMapping = new Dictionary<IngredientType.Type, Action[]>
    {
        { IngredientType.Type.Meat, new Action[] {onMeatAdded,onMeatRemoved }},
        { IngredientType.Type.Flour_Grain, new Action[] {onFlourGrainAdded, onFlourGrainRemoved } },
        { IngredientType.Type.Veggie, new Action[] {onVeggieAdded, onVeggieRemoved} },
        { IngredientType.Type.Seafood, new Action[] {onSeafoodAdded, onSeafoodRemoved} },
        { IngredientType.Type.Fruit, new Action[] {onFruitAdded, onFruitRemoved} },
        { IngredientType.Type.Spices, new Action[] {onSpiceAdded, onSpicesRemoved}},
        { IngredientType.Type.Dairy, new Action[] {onDairyAdded, onDairyRemoved}},
        { IngredientType.Type.Fats_Oils, new Action[] {onFatsOilsAdded, onFatsOilsRemoved} },

    };

}
