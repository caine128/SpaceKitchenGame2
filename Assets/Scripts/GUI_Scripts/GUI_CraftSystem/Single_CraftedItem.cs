using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Single_CraftedItem : MonoBehaviour,IConfigurablePanel , IRushableWithEnergy
{
    [SerializeField] private Single_CraftedItem_VisibilityChecker single_CraftedItem_VisibilityChecker;
    public bool isOccupied { get; private set; } = false;
    public bool IsReadyToReclaim { get; private set; } = false;
    public bool isMoving = false;
    public int? craftedAmount = null;
    public bool isVisible { get; private set;}= false;
    private bool isCraftBarVisible = false;
    public bool cr_Running { get; private set; } = false;
    private Vector2 originalSize;
    //private const int UPDATEINTERVAL = 3;

    [SerializeField] private Image Anchor_Image; //this is for catching the raycasts//
    [SerializeField] private Image BG_Image;
    //[SerializeField] private Image craftedItem_Image;
    //[SerializeField] private Adressable_SpriteDisplay imageContainer_CraftedItem;
    [SerializeField] private AdressableImage imageContainer_CraftedItem_Adressable;
    [SerializeField] private TextMeshProUGUI craftedItem_Text;
    public Image ProgressImage { get { return progressImage; } }
    [SerializeField] private Image progressImage;

    [SerializeField] private GUI_PingPongMethod_Text gUI_PingPongMethod_Text;
    [SerializeField] private GUI_LerpMethods_Float craftProgressFill;
    [SerializeField] private GUI_LerpMethods_Size resizeScript;
    private float lerpSpeedModifier;
    [SerializeField] private string ready_String = "READY";
    [SerializeField] private Color originalTextColor;
    [SerializeField] private Color finishedTextColor;

    private int tickCount = 0; 
    private float maxTickCount; 
    private float craftDuration; 
    public ProductRecipe productRecipe { get; private set; } = null;

    public int TotalRushCostEnergy => productRecipe.recipeSpecs.SpeedUpEnergy;
    public int TotalRushCostGem => productRecipe.GetLevel() * 10;
    public SortableBluePrint BluePrint => this.productRecipe;
    public float CurrentProgress => (float)tickCount / maxTickCount;
    public float RemainingDuration => craftDuration;
    public float MaxUpgradeDuration => productRecipe.GetCraftDuration();

    public event Action<float, float> OnProgressTicked;

    private void OnDisable()
    {
        CraftWheel_Controller.isBarVisible -= SetBarVisibility;
    }
    public void PanelConfig()
    {
        originalSize = GetComponent<RectTransform>().sizeDelta;
        GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        CraftWheel_Controller.isBarVisible += SetBarVisibility;
    }

    private void SetBarVisibility(bool isCraftBarVisibleIN)
    {
        isCraftBarVisible = isCraftBarVisibleIN;
        if (isCraftBarVisible)
        {
            SetScreenVisibilityStatus();
        }
    }

    private void LateUpdate()
    {
        if(Time.frameCount % TimeTickSystem.UPDATEINTERVAL == 0)
        {
            if (isMoving && isCraftBarVisible)
            {
                SetScreenVisibilityStatus();
            }
        }
    }
    //private void PanelConfig()  // LATER TO MAKE IT GLOBALLY
    //{
    //    originalSize = GetComponent<RectTransform>().sizeDelta;
    //    GetComponent<RectTransform>().sizeDelta = Vector2.zero;
    //}

    public void StartCrafting(ProductRecipe productRecipeIN)
    {
        if (cr_Running)
        {
            return;
        }
        SetScreenVisibilityStatus();

        isOccupied = true;
        craftProgressFill.GetComponent<Image>().fillAmount = 0f;  // It already starts from 0 this is just for safety //
        SetChildrenVisibility(true);
        productRecipe = productRecipeIN;

        imageContainer_CraftedItem_Adressable.LoadSprite(productRecipe.GetAdressableImage());
        //imageContainer_CraftedItem_Adressable.preserveAspect = true;
        //SpriteLoader.LoadAdressable(productRecipe.GetAdressableImage(), craftedItem_Image);
        //craftedItem_Image.sprite = productRecipeIN.GetAdressableImage(); //craftedItem_Sprite;
        //craftedItem_Image.preserveAspect = true;

        StartCoroutine(StartCraftingRoutine());

        craftDuration = productRecipeIN.GetCraftDuration(); //craftDurationIN;
        maxTickCount = Mathf.Ceil(craftDuration * 5);
        lerpSpeedModifier = TimeTickSystem.TICK_TIMER_MAX / TimeTickSystem.NUMERIC_LERPDURATION; //  GUI_PlayerStats_Manager.LERP_DURATION;

        TimeTickSystem.onTickTriggered += StartCraftTimer;

        UpdateText(ConvertTime.ToHourMinSec(craftDuration));
      
    }

    IEnumerator StartCraftingRoutine()
    {
        cr_Running = true;

        resizeScript.Resize(originalSize);
        while(resizeScript.RunningCoroutine != null)
        {
            yield return null;
        }
        cr_Running = false;
    }

    private void SetChildrenVisibility(bool isVisible)
    {
        Anchor_Image.enabled = BG_Image.enabled = ProgressImage.enabled = imageContainer_CraftedItem_Adressable.enabled = craftedItem_Text.enabled = isVisible;  // craftedItem_Image.enabled
    }

    private void StartCraftTimer(int tickCountIN,bool isRefillCall)
    {
        var valueInitial = (float)tickCount / maxTickCount;
        var valueFinal = (float)(tickCount + tickCountIN) / maxTickCount;

        UpdateFill(valueInitial, valueFinal);
       
        tickCount += tickCountIN;
        
        if(tickCount % 5 == 0 && tickCount < maxTickCount)
        {
            craftDuration --;
            UpdateText(ConvertTime.ToHourMinSec(craftDuration));
            //UpdateText(ConvertTime(craftDuration));
        }
        else if(tickCount == maxTickCount)
        {
            UpdateText(ready_String, true);
            TimeTickSystem.onTickTriggered -= StartCraftTimer;

            IsReadyToReclaim = true;

            if (!isVisible)
            {
                NotifyInvisibleItemsCounter(canIncrement: true);
            }
        }

        OnProgressTicked?.Invoke(valueInitial, valueFinal);
    }

    public void Rush()
    {
        TimeTickSystem.onTickTriggered -= StartCraftTimer;
        var ticksToRush = Mathf.CeilToInt(maxTickCount - tickCount);
        StartCraftTimer(ticksToRush, isRefillCall: false);
    }


    private void UpdateText(string text, bool isFinished = false)
    {     
        if (isFinished)
        {
            craftedItem_Text.color = finishedTextColor;
            gUI_PingPongMethod_Text.CanPingPong = true;
        }
        else
        {
            craftedItem_Text.color = originalTextColor;
            gUI_PingPongMethod_Text.CanPingPong = false;
        }
        craftedItem_Text.text = text;
    }

    public void UpdateFill(float initialValue, float finalValue)
    {
        craftProgressFill.UpdateBarCall(initialValue, finalValue, lerpSpeedModifier, queueRequest: false);
    }

    public void ReclaimCrafted()
    {
        if (cr_Running)
        {
            return;
        }

        craftedAmount = null;
        isOccupied = false;
        IsReadyToReclaim = false;
        tickCount = 0;
        gUI_PingPongMethod_Text.ResetToOriginalValue();
        SetFGColor();
        StartCoroutine(ReclaimCraftedRoutine());
    }

    IEnumerator ReclaimCraftedRoutine()
    {
        cr_Running = true;

        resizeScript.Resize(Vector2.zero);
        while(resizeScript.RunningCoroutine != null)
        {
            yield return null;
        }
        SetChildrenVisibility(false);

        imageContainer_CraftedItem_Adressable.UnloadSprite();
        //craftedItem_Image.sprite = null;
        //SpriteLoader.UnloadAdressable(productRecipe.GetAdressableImage());

        productRecipe = null;

        cr_Running = false;
    }

    public void SetScreenVisibilityStatus()
    {
        if(isVisible == single_CraftedItem_VisibilityChecker.CheckVisibility())
        {
            return;
        }
        else
        {
            isVisible = single_CraftedItem_VisibilityChecker.CheckVisibility();
            if (!isVisible && IsReadyToReclaim)
            {
                NotifyInvisibleItemsCounter(canIncrement: true);
            }
            else if(isVisible && IsReadyToReclaim)
            {
                NotifyInvisibleItemsCounter(canIncrement: false);
            }
        }      
    }

    private void NotifyInvisibleItemsCounter(bool canIncrement)
    {
        Crafted_InvisibleItems_Counter.Instance.CountInvisibleReadyItems(canIncrement);
    }

    public void SetFGColor()
    {
        if (craftedAmount == null || craftedAmount == 1 && progressImage.color != Color.green)
        {
            progressImage.color = Color.green;
        }
        else if(progressImage.color != Color.red)
        {
            progressImage.color = Color.red;
        }
    }


}
