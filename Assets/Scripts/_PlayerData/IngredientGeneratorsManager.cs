using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientGeneratorsManager : MonoBehaviour
{
    private static IngredientGeneratorsManager _instance;
    public static IngredientGeneratorsManager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    public static  Dictionary<IngredientType.Type, IngredientGenerator> IngredientGenerators { get; private set; }

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
        Config();
    }
    //private void Start()
    //{
    //    Config(); // LATER TO TAKE UP! !! AND ARRANGE IT TO BE CONFIGURABLE BY LOAD OR NORMAL START 
    //}

    public void Config(Dictionary<IngredientType.Type, IngredientGenerator> ingredientGeneratorsDict_IN = null)
    {        
        IngredientGenerators = ingredientGeneratorsDict_IN ?? InitDeactivatedGeneratorsDict();
    }

    private Dictionary<IngredientType.Type, IngredientGenerator> InitDeactivatedGeneratorsDict()
    {
        
        var newDict = new Dictionary<IngredientType.Type, IngredientGenerator>
        {
            { IngredientType.Type.Meat, new IngredientGenerator(IngredientType.Type.Meat)},
            { IngredientType.Type.Flour_Grain, new IngredientGenerator(IngredientType.Type.Flour_Grain)},
            { IngredientType.Type.Veggie, new IngredientGenerator(IngredientType.Type.Veggie)},
            { IngredientType.Type.Seafood, new IngredientGenerator(IngredientType.Type.Seafood)},
            { IngredientType.Type.Fruit, new IngredientGenerator(IngredientType.Type.Fruit)},
            { IngredientType.Type.Spices, new IngredientGenerator(IngredientType.Type.Spices)},
            { IngredientType.Type.Dairy, new IngredientGenerator(IngredientType.Type.Dairy)},
            { IngredientType.Type.Fats_Oils, new IngredientGenerator(IngredientType.Type.Fats_Oils)},

        };
        return newDict;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ActivateGenerator(IngredientType.Type.Meat);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ActivateGenerator(IngredientType.Type.Flour_Grain);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            ActivateGenerator(IngredientType.Type.Veggie);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha4))
        {
            ActivateGenerator(IngredientType.Type.Seafood);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            ActivateGenerator(IngredientType.Type.Fruit);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            ActivateGenerator(IngredientType.Type.Spices);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ActivateGenerator(IngredientType.Type.Dairy);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            ActivateGenerator(IngredientType.Type.Fats_Oils);
        }
    }

    public void ActivateGenerator(IngredientType.Type ingredientType_IN) // Later to take this on the Generator object itself !! 
    {
        Debug.Log("Activating generator of : " + ingredientType_IN);

        IngredientGenerators[ingredientType_IN].Activate();
    }




}
