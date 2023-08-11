using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientDisplayPanel_Manager : Panel_Base ,IRefreshablePanel, IDeallocatable
{
    private static IngredientDisplayPanel_Manager _instance;
    public static IngredientDisplayPanel_Manager Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    [SerializeField] IngredientContentContainer[] ingredientContentContainers;
    //public const float LERPSPEED_MODIFIER = TimeTickSystem.NUMERIC_LERPDURATION;  //.01f / TimeTickSystem.NUMERIC_LERPDURATION;    //GUI_PlayerStats_Manager.LERP_DURATION  ;

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
                }
            }
        }
    }

    public void RefreshPanel()
    {
        for (int i = 0; i < ingredientContentContainers.Length; i++)
        {
            ingredientContentContainers[i].Load();
        }
    }

    //public void DeallocateFields()
    //{

    //}

    public void SetDisplayContainerBarFill(IngredientType.Type ingredientType, float initialValue, float finalValue)
    {
        ingredientContentContainers[(int)ingredientType].UpdateBarFill(initialValue, finalValue, lerpSpeedModifier:1);
    }

    public void UnloadAndDeallocate()
    {
        for (int i = 0; i < ingredientContentContainers.Length; i++)
        {
            ingredientContentContainers[i].Unload();
        }
    }
}
