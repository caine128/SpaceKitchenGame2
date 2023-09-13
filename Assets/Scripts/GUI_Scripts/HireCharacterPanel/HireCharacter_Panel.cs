using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HireCharacter_Panel : Panel_Base, IDeallocatable, IAnimatedPanelController_ManualHide_Cancellable
{
    public static HireCharacter_Panel Instance => _instance;
    private static HireCharacter_Panel _instance;
    private static readonly object _lock = new();



    public bool IsAnimating 
    { 
        get => _isAnimating ; 
        set => _isAnimating = (value == true && hasCompletedAnimation) ? false : value;
           
     
    }
    [SerializeField] private bool _isAnimating = false; // Serialized for debug purpose
    public IEnumerator[] CO  { get => _co; }
    private IEnumerator[] _co;
    public GUI_LerpMethods PanelToAwait { get => _panelToAwait; }
    [SerializeField] private GUI_LerpMethods _panelToAwait;

    public Character SelectedCharacter
    {
        get => _selectedCharacter;
    }
    private Character _selectedCharacter;

    [SerializeField] private DialogueDisplay_Panel dialogueDisplay_Panel;
    //[SerializeField] private Adressable_SpriteDisplay bigImageContainer;
    [SerializeField] private AdressableImage bigImageContainer_Adressable;
    [SerializeField] private Infographics_Panel infographics_Panel;
    [SerializeField] private HireCharacterPanelButton[] buttons;

    private WaitUntil waitUntilDiaglogueIsEnded;
    private WaitUntil waitUntilInforgraphicsPlaced;
    private WaitUntil waitUntilLastButtonScaled;
    private bool hasCompletedAnimation = false;

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
                }
            }
        }

        _co = new IEnumerator[1];
        waitUntilDiaglogueIsEnded = new WaitUntil(() => dialogueDisplay_Panel.CO[0] is null);
        waitUntilInforgraphicsPlaced = new WaitUntil(() => infographics_Panel.CO is null && !infographics_Panel.isSubContainersAnimating);
        waitUntilLastButtonScaled = new WaitUntil(() => buttons[buttons.Length - 1].IsAnimating);
    }

    public void LoadPanel(PanelLoadData panelLoadData)
    {
        hasCompletedAnimation = false;
        switch (panelLoadData.mainLoadInfo)
        {
            case Worker worker:
                _selectedCharacter = worker;
                dialogueDisplay_Panel.LoadPanel(worker.workerspecs.allDialoguePhrases[0].phrases);
                bigImageContainer_Adressable.LoadSprite(worker.GetAdressableImage());
                infographics_Panel.LoadPanel(worker);

                for (int i = 0; i < buttons.Length; i++)
                {
                    buttons[i].ScaleDirect(isVisible: false, finalValueOperations: null);
                }
                buttons[0].SetupButton(ButtonFunctionType.HireCharactersPanel.RecruitWithGold);
                buttons[1].SetupButton(ButtonFunctionType.HireCharactersPanel.RecruitWithGem);
                break;
        }       
    }
 
    public void DisplayContainers()
    {
        if (_co[0] is not null)
        {
            StopCoroutine(_co[0]);
            _co[0] = null;
        }
        /// This is added as a secondary chech in case of hirepanel is sent back in stack and then called forward 
        /// hence without this check it repeats setting _isAnimating bool to true while there is no animation.
        if (!hasCompletedAnimation) 
        {
            _co[0] = DisplayContainersRoutine();
            StartCoroutine(_co[0]);
        } 
    }


    private IEnumerator DisplayContainersRoutine()
    {
        _isAnimating = true;
        dialogueDisplay_Panel.DisplayContents();
        yield return waitUntilDiaglogueIsEnded;

        infographics_Panel.DisplayContents();
        yield return waitUntilInforgraphicsPlaced;

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].AnimateWithRoutine(customInitialValue: null,
                                          secondaryInterpolation: null,
                                          isVisible: true,
                                          lerpSpeedModifier: 1f,
                                          followingAction_IN: null);
        }
        yield return waitUntilLastButtonScaled;

        _co[0] = null;
        _isAnimating = false;
        hasCompletedAnimation = true;
    }


    public void FastForwardDisplayAnimation()
    {

        dialogueDisplay_Panel.FastForwardDisplayAnimation();
    }

    public void UnloadAndDeallocate()
    {
        infographics_Panel.UnloadAndDeallocate();
        bigImageContainer_Adressable.UnloadSprite();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].UnloadButton();
        }
    }

    public void HideContainers()
    {
    }
}
