using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class CharactersInfoPanel_Manager : TabbedPanel<Character, Tab.CharacterInfoTabs>
{
    private static CharactersInfoPanel_Manager _instance;
    public static CharactersInfoPanel_Manager Instance { get { return _instance; } }
    private static readonly object _lock = new object();
    [SerializeField] private RectTransform progressbar_BG;
    [SerializeField] private GUI_LerpMethods_Float progressBar_FG;

    public override List<Character> ListToIterate
    {
        get
        {
            if (SelectedRecipe is Worker)
            {
                Debug.Log("listto iterate is returned");
                return CharacterManager.CharactersAvailable_Dict[CharacterType.Type.Worker].Where(w => w.isHired).ToList();
            }
            else
            {
                Debug.LogWarning("the list shouldnt return null !");
                return null;
            }
        }
    }

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
                if (_instance == null)
                {
                    _instance = this;
                    //DontDestroyOnLoad(this.gameObject);   /// This will be done later
                }
            }
        }
    }


    public sealed override void InitialConfigBrowserButtons()
    {
        foreach (var browserButton in _browserButtons)
        {
            browserButton.ButtonConfig(Instance);
        }
    }

    protected override void LoadInfo(Character bluePrint_IN)
    {
        var comparer = new CharacterEqualityComparer();
        if(SelectedRecipe is null || !comparer.Equals(SelectedRecipe, bluePrint_IN)) // char comparer can be done with Iequatable without character comparer instance  
        {
            SelectedRecipe = bluePrint_IN;

            blueprintNameText.text = SelectedRecipe.GetName();
            
        }

        blueprintTypeText.text = SelectedRecipe.GetType().ToString();
        bluePrintTypeLevelInfoText.text = $"Level {SelectedRecipe.GetLevel()}";
        Debug.LogWarning(SelectedRecipe.GetAdressableImage());
        bigImageContainer_Adressable.LoadSprite(SelectedRecipe.GetAdressableImage());
        thumbnailImageContainer_Adressable.LoadSprite(SelectedRecipe.GetAdressableImage());

        progressBar_FG.ClearQueue();

        if (bluePrint_IN.isAtMaxLevel)
        {
            progressbar_BG.gameObject.SetActive(false);
            progressBar_FG.gameObject.SetActive(false);
        }
        else
        {
            progressbar_BG.gameObject.SetActive(true);
            progressBar_FG.gameObject.SetActive(true);
            progressBar_FG.UpdateBarCall(
                                         initialValue: 0f,
                                         finalValue: (float)bluePrint_IN.GetCurrentXP() / (float)bluePrint_IN.GetXpToNextLevel(),
                                         lerpSpeedModifier: 3,
                                         queueRequest: false);
        }
        


        foreach (var tabPanel in tabPanels)
        {
            tabPanel.LoadInfo();
        }

        var browsablePanelInterface = ((IBrowsablePanel<Character>) this);

        _currentIndice = browsablePanelInterface.SetCurrentIndice(SelectedRecipe);
        browsablePanelInterface.SetVisibilityBrowserButtons();

        ExecuteEvents.Execute(tabSelectorButtons[lastSelectionIndex].gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
    }
}
