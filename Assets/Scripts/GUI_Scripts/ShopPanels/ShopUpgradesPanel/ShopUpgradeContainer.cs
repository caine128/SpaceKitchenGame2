using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ShopUpgrades_SO;

public class ShopUpgradeContainer : Container<ShopUpgrade>
{
    [SerializeField] private Image shopUpgradeProgressBarBG;
    [SerializeField] private Image shopUpgradeProgressBarFG;

    [SerializeField] private TextMeshProUGUI descriptionHeaderText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    private RectTransform descriptionTextRect;

    [SerializeField] private ContentDisplay_JustSprite descriptionSprite;
    private GUI_PingPongMethod_RT containerPingPong;

    public override RectTransform rt { get { return _rect; } }
    [SerializeField] private RectTransform _rect;

    public TextMeshProUGUI Name { get { return _name; } }                     // serialized for debug purposes later to delte  // THOSE CAN BE IN THE CONTAINER ABSTRACT CLASSS !!!
    [SerializeField] private TextMeshProUGUI _name;
    public TextMeshProUGUI Level { get { return _level; } }                    // serialized for debug purposes later to delte  // THOSE CAN BE IN THE CONTAINER ABSTRACT CLASSS !!!
    [SerializeField] private TextMeshProUGUI _level;
    public TextMeshProUGUI Value { get { return _value; } }                      // serialized for debug purposes later to delte  // THOSE CAN BE IN THE CONTAINER ABSTRACT CLASSS !!!
    [SerializeField] private TextMeshProUGUI _value;
    //public Image DisplayImage { get { return _displayImage; } }                        // serialized for debug purposes later to delte  // THOSE CAN BE IN THE CONTAINER ABSTRACT CLASSS !!!

    [SerializeField] private TextMeshProUGUI amountInShop;

    private ShopUpgrade correspondingbluePrint;

    private void Awake()
    {
        descriptionTextRect = descriptionText.GetComponent<RectTransform>();
        containerPingPong = GetComponent<GUI_PingPongMethod_RT>();
    }

    public override void LoadContainer(ShopUpgrade shopupgrade_IN)
    {
        containerPingPong.CanPingPong = false;
        containerPingPong.ResetToOriginalValue();
        mainImageContainer.LoadSprite(shopupgrade_IN.GetAdressableImage());
        if(correspondingbluePrint is WorkStationUpgrade previousWorkstationUpgrade) previousWorkstationUpgrade.OnUpgradeTimerUpdate -= MatchContainerDynamicInfo;
        
        bluePrint = shopupgrade_IN;
        _name.text = bluePrint.GetName();

        if (shopupgrade_IN is FoodDisplayUpgrade foodDisplayUpgrade)
        {
            //bluePrint = (FoodDisplayUpgrade)shopupgrade_IN;
            correspondingbluePrint = null;
        }
        else if (shopupgrade_IN is InventoryUpgrade inventoryUpgrade)
        {
            //bluePrint = (InventoryUpgrade)shopupgrade_IN;
            correspondingbluePrint = null;
        }
        else if (shopupgrade_IN is ResourceCabinetUpgrade resourceCabinetUpgrade)
        {
            correspondingbluePrint = null;
            //bluePrint = (ResourceCabinetUpgrade)shopupgrade_IN;
        }
        else if (shopupgrade_IN is WorkStationUpgrade workStationUpgrade)
        {
            if (descriptionHeaderText.gameObject.activeInHierarchy != false) descriptionHeaderText.gameObject.SetActive(false);

            switch (ShopData.CheckPresenceOfUpgrade(workStationUpgrade, out IEnumerable<WorkStationUpgrade> workstationUpgrades_Out))
            {

                case true:                   
                    var corrsepondingWorksation = workstationUpgrades_Out.First();
                    correspondingbluePrint = corrsepondingWorksation;

                    _level.text = corrsepondingWorksation.GetLevel().ToString(); // shopUpgrade_out.GetLevel().ToString();
                    amountInShop.text = "1";                   
                    //var corrsepondingWorksation = (WorkStationUpgrade)correspondingbluePrint;
                    var associtatedWorker = CharacterManager.Instance.WorkerList_SO.listOfWorkers.First(w => w.workStationPrerequisites[0].type == corrsepondingWorksation.GetWorkstationType());

                    _value.text = $"Current Max Level for {associtatedWorker.characterName}";
                    descriptionText.text = $"Lvl.{corrsepondingWorksation.GetMaxWorkerLevelCapCurrent()}";
                    descriptionSprite.Load(new ContentDisplayInfo_JustSprite(associtatedWorker.characterImageRef));
                    GUI_CentralPlacement.PlaceImageWithText(textToplace: descriptionText, textToplaceRect: descriptionTextRect, imageToPlaceRect: descriptionSprite.RT, isImageOnLeft: true);

                    if (descriptionText.gameObject.activeInHierarchy != true
                       || descriptionSprite.gameObject.activeInHierarchy != true || _value.gameObject.activeInHierarchy != true)
                    {
                        //_value.gameObject.SetActive(true);  // is this necessary ?
                        descriptionText.gameObject.SetActive(true);
                        descriptionSprite.gameObject.SetActive(true);
                    }

                    switch (corrsepondingWorksation.RemainingDuration <= 0)
                    {
                        case true and var isReadyToLevelUp when !corrsepondingWorksation.IsReadyToReclaim: 
                            /// is Not a Level Up 
                            _value.text = $"Current Max Level for {associtatedWorker.characterName}";
                            if (containerImage.color != Color.white) containerImage.color = Color.white;
                            if (shopUpgradeProgressBarBG.gameObject.activeInHierarchy != false
                                || shopUpgradeProgressBarFG.gameObject.activeInHierarchy != false)
                            {
                                shopUpgradeProgressBarBG.gameObject.SetActive(false);
                                shopUpgradeProgressBarFG.gameObject.SetActive(false);
                            }
                            break;
                        case true and var isReadyToLevelUp when corrsepondingWorksation.IsReadyToReclaim: 
                            /// is a Completed Level Up 
                            _value.text = $"Current Max Level for {associtatedWorker.characterName}";
                            containerPingPong.CanPingPong = true;

                            if (containerImage.color != Color.green) containerImage.color = Color.green;
                            if (shopUpgradeProgressBarBG.gameObject.activeInHierarchy != false
                                || shopUpgradeProgressBarFG.gameObject.activeInHierarchy != false)
                            {
                                shopUpgradeProgressBarBG.gameObject.SetActive(false);
                                shopUpgradeProgressBarFG.gameObject.SetActive(false);
                            }
                            break;
                        case false:
                            /// is a Running Level Up
                            MatchContainerDynamicInfo();
                            corrsepondingWorksation.OnUpgradeTimerUpdate += MatchContainerDynamicInfo;

                            //_value.text = $"{corrsepondingWorksation.MaxUpgradeDuration- corrsepondingWorksation.RemainingDuration} / {corrsepondingWorksation.MaxUpgradeDuration}";
                            if (containerImage.color != Color.yellow) containerImage.color = Color.yellow;
                            if (shopUpgradeProgressBarBG.gameObject.activeInHierarchy != true
                                || shopUpgradeProgressBarFG.gameObject.activeInHierarchy != true)
                            {
                                shopUpgradeProgressBarBG.gameObject.SetActive(true);
                                shopUpgradeProgressBarFG.gameObject.SetActive(true);
                            }
                            break;
                    }


                    if (descriptionText.gameObject.activeInHierarchy != true
                        || descriptionSprite.gameObject.activeInHierarchy != true || _value.gameObject.activeInHierarchy != true)
                    {
                        //_value.gameObject.SetActive(true);  // is this necessary ?
                        descriptionText.gameObject.SetActive(true);
                        descriptionSprite.gameObject.SetActive(true);
                    }


                    break;
                case false:
                    _level.text = "0";
                    amountInShop.text = "0";
                    _value.text = $"Purchase {_name.text}";
                    if (containerImage.color != Color.grey) containerImage.color = Color.grey;
                    if (descriptionText.gameObject.activeInHierarchy != false
                        || descriptionSprite.gameObject.activeInHierarchy != false)
                    {
                        descriptionText.gameObject.SetActive(false);
                        descriptionSprite.gameObject.SetActive(false);
                    }
                    if (shopUpgradeProgressBarBG.gameObject.activeInHierarchy != false
                        || shopUpgradeProgressBarFG.gameObject.activeInHierarchy != false)
                    {
                        shopUpgradeProgressBarBG.gameObject.SetActive(false);
                        shopUpgradeProgressBarFG.gameObject.SetActive(false);
                    }
                    break;
            }
        }
    }

    public override void MatchContainerDynamicInfo()
    {
        if (correspondingbluePrint is WorkStationUpgrade correspondingWorksation)
        {
            switch (correspondingWorksation.RemainingDuration > 0)
            {
                case true:
                    //containerPingPong.CanPingPong=true;

                    float elapsedDuration = correspondingWorksation.MaxUpgradeDuration - correspondingWorksation.RemainingDuration;
                    _value.text = $"{ConvertTime.ToHourMinSec(correspondingWorksation.RemainingDuration)}";
                    shopUpgradeProgressBarFG.fillAmount = elapsedDuration / correspondingWorksation.MaxUpgradeDuration;
                    break;
                case false:
                    containerPingPong.CanPingPong = true;
                    if (containerImage.color != Color.green) containerImage.color = Color.green;

                    Debug.Log(transform.localScale);
                    _value.text = "Upgrade Complete";
                    shopUpgradeProgressBarFG.fillAmount = 1f;
                    correspondingWorksation.OnUpgradeTimerUpdate -= MatchContainerDynamicInfo;
                    break;
            }
        }
    }

    public override void UnloadContainer()
    {
        if (correspondingbluePrint is WorkStationUpgrade correspondingWorksation) 
        {
            correspondingWorksation.OnUpgradeTimerUpdate -= MatchContainerDynamicInfo;
        }

        containerPingPong.CanPingPong = false;
        containerPingPong.ResetToOriginalValue();

        mainImageContainer.UnloadSprite();
        descriptionSprite.Unload();

        _name.text = null;
        _level.text = null;
        _value.text = null;
    }

}
