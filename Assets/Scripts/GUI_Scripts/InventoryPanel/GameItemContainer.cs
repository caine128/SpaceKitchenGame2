
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameItemContainer : Container<GameObject>
{
    private static IReassignablePanel _reassignablePanel;

    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image descriptionImage;
    public override RectTransform rt { get { return _rect; } }
    [SerializeField] private RectTransform _rect;

    public TextMeshProUGUI Name { get { return _name; } }                                  // serialized for debug purposes later to delte  // THOSE CAN BE IN THE CONTAINER ABSTRACT CLASSS !!!
    [SerializeField] private TextMeshProUGUI _name;

    public TextMeshProUGUI Level { get { return _level; } }                                  // serialized for debug purposes later to delte 
    [SerializeField] private TextMeshProUGUI _level;

    public TextMeshProUGUI Value { get { return _value; } }                                  // serialized for debug purposes later to delte 
    [SerializeField] private TextMeshProUGUI _value;

    //public Image DisplayImage { get { return _displayImage; } }                              // serialized for debug purposes later to delte 
    //[SerializeField] private Image _displayImage;

    [SerializeField] private TextMeshProUGUI amountInInventory;
    [SerializeField] private TextMeshProUGUI bottomInfoText;
    //public override IconContentDisplay[] SubDisplaycontainers => throw new System.NotImplementedException();     // try TO REMOVE THIS UNNECESSARY ITEM !!

    public void Awake()                 // LATER TO TAKE UP   // TO MAKE SOMETHING FROM START FOR EVERY REASSIGNED CATCH HIS REASSIGNABLE PANEL
    {
        _reassignablePanel = UnityEngine.GameObject.Find("Inventory_Panel_Parent").GetComponent<IReassignablePanel>();
    }

    public override void LoadContainer(GameObject item_IN)    // change newrecipe to newblueprint_IN !!!!
    {
        mainImageContainer.LoadSprite(item_IN.GetAdressableImage());
        //if (_displayImage.sprite != null && bluePrint != null) SpriteLoader.UnloadAdressable(bluePrint.GetAdressableImage());
        //SpriteLoader.LoadAdressable(item_IN.GetAdressableImage(), _displayImage);
        
        if (item_IN is Product product)
        {
            bluePrint = product;

            if (_reassignablePanel.panelAssignedState == IReassignablePanel.AssignedState.Inventory_FromEnhanceToProduct)
            {
                if (bottomInfoText.enabled != true) bottomInfoText.enabled = true;

                var selectedEnhancement = GameItemInfoPanel_Manager.Instance.SelectedRecipe as Enhancement;
                var successRatio = Enhance.EnhanceSuccessRatio(product, selectedEnhancement);
                var successColor = NativeHelper.GetSuccessRatioColor(successRatio);//GetEnhancementRatioColor(succesRatio);
                bottomInfoText.text = string.Format("% {0} Chance", successRatio.ToString());
                if (bottomInfoText.color != successColor) bottomInfoText.color = successColor;
            }
            else if (_reassignablePanel.panelAssignedState != IReassignablePanel.AssignedState.Inventory_FromProductToEnhance)
            {
                if (bottomInfoText.enabled != false) bottomInfoText.enabled = false;
            }


            if (_level.enabled == false || _value.enabled == false || descriptionText.enabled == false || descriptionImage.enabled == false)
            {
                _level.enabled = _value.enabled = descriptionText.enabled = descriptionImage.enabled = true;
            }

            var qualityColor = product.GetQuality().GetQualityColor();
            if (_name.color != qualityColor)
            {
                _name.color = qualityColor;
            }


            _level.text = product.GetLevel().ToString();
            _value.text = ISpendable.ToScreenFormat(product.GetValue());//.ToString();

        }



        else if(item_IN is SpecialItem || item_IN is ExtraComponent)
        {
            bluePrint = item_IN;

            if (_level.enabled == true || _value.enabled == true || descriptionText.enabled == true || descriptionImage.enabled == true)
            {
                _level.enabled = _value.enabled = descriptionText.enabled = descriptionImage.enabled = false;
            }

            if (_name.color != Color.white)
            {
                _name.color = Color.white;
            }

            if (bottomInfoText.enabled != false)
            {
                bottomInfoText.enabled = false;
            }

        }


        else if (item_IN is Enhancement enhancement)
        {
            bluePrint = enhancement;

            if(_reassignablePanel.panelAssignedState == IReassignablePanel.AssignedState.Inventory_FromProductToEnhance)
            {
                if (bottomInfoText.enabled != true) bottomInfoText.enabled = true;

                var selectedEnhanceable = GameItemInfoPanel_Manager.Instance.SelectedRecipe as IEnhanceable;
                var successRatio = Enhance.EnhanceSuccessRatio(selectedEnhanceable, enhancement);
                var successColor = NativeHelper.GetSuccessRatioColor(successRatio);//GetEnhancementRatioColor(succesRatio);
                bottomInfoText.text = string.Format("% {0} Chance", successRatio.ToString());
                if (bottomInfoText.color != successColor) bottomInfoText.color = successColor;
            }
            else if (bottomInfoText.enabled != false) //if(_reassignablePanel.panelAssignedState != IReassignablePanel.AssignedState.Inventory_FromProductToEnhance)
            {
                 bottomInfoText.enabled = false;
            }

            if (_level.enabled == false || _value.enabled == false || descriptionText.enabled == false || descriptionImage.enabled == false)
            {
                _level.enabled = _value.enabled = descriptionText.enabled = descriptionImage.enabled = true;
            }

            var qualityColor = enhancement.GetQuality().GetQualityColor();
            if (_name.color != qualityColor)
            {
                _name.color = qualityColor;
            }

            _level.text = enhancement.GetLevel().ToString();
            _value.text = ISpendable.ToScreenFormat(enhancement.GetValue());//.ToString();
        }

        _name.text = item_IN.GetName() + item_IN.DateLastCrafted.ToString("T");          // DATE IS FOR DEBUG PURPOSE !!!
        //_displayImage.sprite = item_IN.GetImage();
        amountInInventory.text = item_IN.GetAmount().ToString();
    }

    //private Color GetEnhancementRatioColor(int enhancementSuccessRatio_IN)
    //{
    //    if (enhancementSuccessRatio_IN == 100) return Color.green;
    //    else if (enhancementSuccessRatio_IN >= 25 && enhancementSuccessRatio_IN < 100) return Color.yellow;
    //    else if (enhancementSuccessRatio_IN >= 10 && enhancementSuccessRatio_IN < 25) return Color.magenta;
    //    else return Color.red;
    //}


    public override void MatchContainerDynamicInfo()  // WHY THIS IS HERE SHOULDNT BE 
    {
        Debug.LogError("Should't BE WORKING ");
    }



    public override void UnloadContainer()
    {
        mainImageContainer.UnloadSprite();
        //_displayImage.sprite = null;
        //SpriteLoader.UnloadAdressable(bluePrint.GetAdressableImage());

        //bluePrint = null;    To not deallocate !! or find a proper order to do it !!
        _name.text = null;
        _level.text = null;
        _value.text = null;
        amountInInventory.text = null;
        bottomInfoText.text = null;
    }
}
