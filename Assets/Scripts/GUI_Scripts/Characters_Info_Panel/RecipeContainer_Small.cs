using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeContainer_Small : Container<ProductRecipe>
{
    public override RectTransform rt
    {
        get => _rect;
    } 
    [SerializeField] private RectTransform _rect;

    public TextMeshProUGUI RecipeName
    {
        get=>_recipeName;
    }  
    [SerializeField] protected TextMeshProUGUI _recipeName;

    [SerializeField] protected TextMeshProUGUI amountInInventory;

    public override void LoadContainer(ProductRecipe newRecipe_IN)
    {
        _recipeName.text = newRecipe_IN.GetName();
        mainImageContainer.LoadSprite(newRecipe_IN.GetAdressableImage());
        bluePrint = newRecipe_IN;
        amountInInventory.text = Inventory.Instance.CheckAmountInInventory_ByNameDict(newRecipe_IN.GetName(), out _).ToString();
    }

    public override void MatchContainerDynamicInfo() // later to remove this is not necessary here
    {
        Debug.LogError("shouldnt ve working");
        throw new System.NotImplementedException();
    }

    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();
        _recipeName.text = null;
        amountInInventory.text = null;
    }
}
