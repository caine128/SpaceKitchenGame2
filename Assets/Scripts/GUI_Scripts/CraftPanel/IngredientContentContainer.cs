using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientContentContainer : ContentContainer<IngredientType.Type>
{
    [SerializeField] private GUI_LerpMethods_Float progressBar;

    private void OnEnable()
    {
        ResourcesManager.onCapReached += SetProgressBarVisibility;
        ResourcesManager.onCapReached += SetTextColor;
        ResourcesManager.ingredientEventMapping[type][0] += UpdateTextfield;
        ResourcesManager.ingredientEventMapping[type][1] += UpdateTextfield;

    }

    private void OnDisable()
    {

        ResourcesManager.ingredientEventMapping[type][0] -= UpdateTextfield;
        ResourcesManager.ingredientEventMapping[type][1] -= UpdateTextfield;
        ResourcesManager.onCapReached -= SetTextColor;
        ResourcesManager.onCapReached -= SetProgressBarVisibility;
        progressBar.ClearQueue();
    }

    public override void Load()
    {
        var existingAmount = ResourcesManager.CheckAmountOfIngredient(type, out Ingredient ingredient);  //ResourcesManager.ingredientsDict[type].GetAmount();  //ResourcesManager.ingredientsDict[type].ingredientAmount;
        var currentMaxCap = ingredient.MaxCap;                    //ResourcesManager.ingredientsDict[type].MaxCap;

        imageContainer_Adressable.LoadSprite(ResourcesManager.Instance.Resources_SO.ingredients[(int)type].spriteRef);
        contentText.text = NativeHelper.BuildString_Append(existingAmount.ToString(), "/", currentMaxCap.ToString());
        SetTextColor(existingAmount >= currentMaxCap, type);
        SetProgressBarVisibility(existingAmount >= currentMaxCap, type);
    }


    private void UpdateTextfield()
    {
        contentText.text = NativeHelper.BuildString_Append(ResourcesManager.CheckAmountOfIngredient(type, out Ingredient ingredient).ToString(), " / ", ingredient.MaxCap.ToString());                                     //ResourcesManager.ingredientsDict[type].GetAmount().ToString(), " / ", ResourcesManager.ingredientsDict[type].MaxCap.ToString());
    }

    private void SetTextColor(bool isCapReached, IngredientType.Type ingredientType_IN)
    {
        if (type == ingredientType_IN) contentText.SetTextColor_CapReached(isCapReached);
        //{
            //if (isCapReached && contentText.color != Color.green)
            //{
            //    contentText.color = Color.green;
            //}
            //else if (!isCapReached && contentText.color != Color.white)
            //{
            //    contentText.color = Color.white;
            //}
        //}
    }

    private void SetProgressBarVisibility(bool isCapReached_IN, IngredientType.Type ingredientType_IN)
    {
        if (type == ingredientType_IN)
        {
            if (isCapReached_IN && progressBar.gameObject.activeInHierarchy == true)
            {
                progressBar.gameObject.SetActive(false);
                progressBar.ClearQueue();
            }
            else if (!isCapReached_IN && progressBar.gameObject.activeInHierarchy == false)
            {

                progressBar.gameObject.SetActive(true);
            }
        }
    }

    public void UpdateBarFill(float initialValue, float finalValue, float lerpSpeedModifier)
    {
        progressBar.UpdateBarCall(initialValue, finalValue, lerpSpeedModifier, queueRequest:false);
    }



    public override void Unload()
    {
        //base.Unload();
        imageContainer_Adressable.UnloadSprite();
        contentText.text = null;
        //SpriteLoader.UnloadAdressable(ResourcesManager.Instance.Resources_SO.ingredients[(int)type].spriteRef);
    }
}
