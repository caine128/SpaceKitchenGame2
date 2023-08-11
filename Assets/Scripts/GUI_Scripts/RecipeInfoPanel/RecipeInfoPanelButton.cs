
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RecipeInfoPanelButton : MultiPurposeButton<ButtonFunctionType.RecipeInfoPanel> //MonoBehaviour, IPointerDownHandler
{
    //[SerializeField] private Sprite[] buttonSprites;

    public void Awake()
    {
        buttonNames = new string[] { "Craft", "Ascend", "Fully Ascended" };
    }

    public override void SetupButton(ButtonFunctionType.RecipeInfoPanel buttonFunction_IN)
    {
        switch (buttonFunction_IN)
        {
            case ButtonFunctionType.RecipeInfoPanel.None:
                //buttonFunction = buttonFunction_IN;
                buttonFunctionDelegate = DoNothing;
                buttonName.text = buttonNames[2];
                buttonInnerImage_Adressable.UnloadSprite();
                //buttonImage.sprite = null;
                break;

            case ButtonFunctionType.RecipeInfoPanel.CraftButton:
                //buttonFunction = buttonFunction_IN;
                //buttonFunctionDelegate = null;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += CraftAsync;
                buttonName.text = buttonNames[0];
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("QualityChanceIncrease"));
                //buttonImage.sprite = buttonSprites[0];
                break;

            case ButtonFunctionType.RecipeInfoPanel.AscendButton:
                // buttonFunction = buttonFunction_IN;
                //buttonFunctionDelegate = null;
                buttonFunctionDelegate = gUI_TintScale.TintSize;
                buttonFunctionDelegate += Ascend;
                buttonName.text = buttonNames[1];
                buttonInnerImage_Adressable.LoadSprite(ImageManager.SelectSprite("StarIconYellow"));
                //buttonImage.sprite = buttonSprites[1];
                break;
        }
    }



    private async void CraftAsync()           // THE EVENT CALLBACK CALLING THIS FUNCTION IS NOT ASYNC SHOULD IT BE ???!
    {
        await Radial_CraftSlots_Crafter.Instance.TryStartCraftingAsync(RecipeInfoPanel_Manager.Instance.SelectedRecipe);
    }

    private void Ascend()
    {
        RecipeInfoPanel_Manager.Instance.Ascend();
    }


}
