using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BuildOptionsPanel_Manager : Panel_Base ,IDeallocatable , IAanimatedPanelController
{
    [SerializeField] private BuildOptionsButton[] buildOptionsButtons;
    [SerializeField] private TextMeshProUGUI propDisplayName;
    private RectTransform propDisplayTextField_Rt;
    [SerializeField] private ContentDisplay_JustSpriteAndText propIconContentDisplay;
    private RectTransform propIconContentDisplay_Rt;

    private int requiredAmountOfButtons = 0;

    public IEnumerator[] CO { get => _co; }
    private IEnumerator[] _co = null;
    public GUI_LerpMethods PanelToAwait { get => _panelToAwait;}
    [SerializeField] private GUI_LerpMethods _panelToAwait;

    public TaskCompletionSource<bool> Tcs { get; private set; }

    private void Awake()
    {
        propDisplayTextField_Rt = propDisplayName.GetComponent<RectTransform>();
        propIconContentDisplay_Rt = propIconContentDisplay.GetComponent<RectTransform>();

        for (int i = 0; i < buildOptionsButtons.Length; i++)
        {
            buildOptionsButtons[i].gameObject.transform.SetAsFirstSibling();
        }

        /// 0 :DisplayContainersRoutine 1: First chunk of 2 buttons  2:second chunk of two buttons
        /// Designed for max 5 buttons , main button or main buttons (in case of round number of total buttons)
        /// are not included in routines, they are set in the beginning.    
        _co = new IEnumerator[3]; 
    }

    private void Update() //TODO: For debug purposes only later to delete 
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideContainers();
        }
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            DisplayContainers();
        }
    }

    public void LoadPanel(Prop prop)
    {
        propIconContentDisplay.Unload();

        switch (prop.ShopUpgradeBluePrint) 
        {
            case WorkStationUpgrade:

                propDisplayName.text = prop.ShopUpgradeBluePrint.GetName();
                var spriteToLoad = ImageManager.SelectSprite("StarIconRed"); // TODO : Later to Load Proper Sprite for Level Background
                propIconContentDisplay.Load(new ContentDisplayInfo_JustSpriteAndText(textVal_IN: prop.ShopUpgradeBluePrint.GetLevel().ToString(), spriteRef_IN: spriteToLoad));
                GUI_CentralPlacement.PlaceImageWithText(textToplace: propDisplayName, textToplaceRect: propDisplayTextField_Rt,
                                                        imageToPlaceRect: propIconContentDisplay_Rt, isImageOnLeft: true);
                


                switch (prop.IsPurchased)
                {
                    case true:
                        requiredAmountOfButtons = 4; // TODO : should be 5 , now its 4 for test purposes 

                        buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Upgrade);
                        buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Content);
                        buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Skins);
                        buildOptionsButtons[3].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);
                        //buildOptionsButtons[4].SetupButton(ButtonFunctionType.BuildOptionsPanel.Edit); // TODO : Should be active, commented out for test purposes
                        break;
                    case false:
                        requiredAmountOfButtons = 5; // TODO : should be 3 , now its 5 for test purposes

                        buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.Purchase);
                        buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Rotate);
                        buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Exit);

                        buildOptionsButtons[3].SetupButton(ButtonFunctionType.BuildOptionsPanel.Upgrade);
                        buildOptionsButtons[4].SetupButton(ButtonFunctionType.BuildOptionsPanel.Content);
                        break;
                }
                break;

            default:
                break;
        }

        for (int i = 0; i < buildOptionsButtons.Length; i++)
        {
            buildOptionsButtons[i].gameObject.SetActive(i < requiredAmountOfButtons);
        }

        foreach (var (rts,targetPos) in GetRequiredButtonsAndTargetPositions().Take(requiredAmountOfButtons % 2 == 0 ? 2: 1))
        {
            rts.anchoredPosition.Set(targetPos.x,targetPos.y);
        }

    }

    private IEnumerable<(RectTransform rts, Vector2 targetpos)> GetRequiredButtonsAndTargetPositions()
    {
        int mirroringMultiplier;
        int columnNo = 0;
        bool isIndexRound;
        float offsetBetween_X;
        float lastPosRoundX = 0; float lastPosOddX = 0;
        float finalPosX;

        if (requiredAmountOfButtons % 2 == 0)
        {
            for (int i = 0; i < requiredAmountOfButtons; i++)
            {
                if (i == 0 || i == 1)
                    buildOptionsButtons[i].Resize(new Vector2(1.5f, 1.5f));
                else
                    buildOptionsButtons[i].SizeRevertToPriginal();


                offsetBetween_X = i == 0 || i == 1
                                        ? buildOptionsButtons[i].RT.sizeDelta.x / 2
                                        : buildOptionsButtons[Mathf.Max(i - 2, 0)].RT.sizeDelta.x / 2 + buildOptionsButtons[i].RT.sizeDelta.x / 2;
                 yield return CalculatePositions(i);
            }
        }
        else
        {
            for (int i = 0; i < requiredAmountOfButtons; i++)
            {
                if (i == 0)
                    buildOptionsButtons[i].Resize(new Vector2(1.5f, 1.5f));
                else
                    buildOptionsButtons[i].SizeRevertToPriginal();


                offsetBetween_X = i == 0
                                        ? 0
                                        : buildOptionsButtons[Mathf.Max(i - 2, 0)].RT.sizeDelta.x / 2 + buildOptionsButtons[i].RT.sizeDelta.x / 2;

                yield return CalculatePositions(i);
            }
        }

        (RectTransform,Vector2) CalculatePositions(int i)
        {
            isIndexRound = i % 2 == 0;
            mirroringMultiplier = isIndexRound ? 1 : -1;
            columnNo += isIndexRound ? 0 : 1;

            if (isIndexRound)
            {
                finalPosX = lastPosRoundX + mirroringMultiplier * offsetBetween_X;
                lastPosRoundX = finalPosX;
            }
            else
            {
                finalPosX = lastPosOddX + mirroringMultiplier * offsetBetween_X;
                lastPosOddX = finalPosX;
            }

            return (buildOptionsButtons[i].RT, new Vector2(finalPosX, buildOptionsButtons[i].RT.anchoredPosition.y));

           // buildOptionsButtons[i].RT.anchoredPosition = new Vector2(finalPosX, buildOptionsButtons[i].RT.anchoredPosition.y);
        }
    }


    public void DisplayContainers()
    {
        Tcs = new(false);

        if (_co[0] != null)
        {
            StopCoroutine(_co[0]);
            _co[0] = null;
            Debug.LogWarning("stopping displaycontainers ");
        }

        _co[0] = DisplayContainersRoutine(Tcs);
        StartCoroutine(_co[0]);

        /*foreach (var (rts, targetpos) in GetTarGetPositionsOfRts())
        {
            Debug.Log(targetpos);
            rts.anchoredPosition = targetpos;
        }*/
        /*int mirroringMultiplier;
        int columnNo = 0;
        bool isIndexRound;
        float offsetBetween_X;
        float lastPosRoundX = 0; float lastPosOddX = 0;
        float finalPosX;
        if (requiredAmountOfButtons%2 == 0)
        {
            for (int i = 0; i < requiredAmountOfButtons; i++)
            {
                if (i == 0 || i == 1)
                    buildOptionsButtons[i].Resize(new Vector2(1.5f, 1.5f));
                else
                    buildOptionsButtons[i].SizeRevertToPriginal();



                offsetBetween_X = i == 0 || i == 1
                                        ? buildOptionsButtons[i].RT.sizeDelta.x / 2
                                        : buildOptionsButtons[Mathf.Max(i - 2, 0)].RT.sizeDelta.x / 2 + buildOptionsButtons[i].RT.sizeDelta.x / 2;
                CalculatePositions(i);

            

            }
        }
        else
        {
            for (int i = 0; i < requiredAmountOfButtons; i++)
            {
                if (i == 0)
                    buildOptionsButtons[i].Resize(new Vector2(1.5f, 1.5f));
                else
                    buildOptionsButtons[i].SizeRevertToPriginal();

                
                
                offsetBetween_X = i == 0 
                                        ? 0 
                                        : buildOptionsButtons[Mathf.Max(i - 2,0)].RT.sizeDelta.x/2 + buildOptionsButtons[i].RT.sizeDelta.x/2;

                CalculatePositions(i);

            }
        }

        void CalculatePositions(int i)
        {
            isIndexRound = i % 2 == 0;
            mirroringMultiplier = isIndexRound ? 1 : -1;
            columnNo += isIndexRound ? 0 : 1;

            if (isIndexRound)
            {
                finalPosX = lastPosRoundX + mirroringMultiplier * offsetBetween_X;
                lastPosRoundX = finalPosX;
            }
            else
            {
                finalPosX = lastPosOddX + mirroringMultiplier * offsetBetween_X;
                lastPosOddX = finalPosX;
            }

            buildOptionsButtons[i].RT.anchoredPosition = new Vector2(finalPosX, buildOptionsButtons[i].RT.anchoredPosition.y);
        }*/

    }

    private IEnumerator DisplayContainersRoutine(TaskCompletionSource<bool> Tcs)
    {
        bool isRound = requiredAmountOfButtons % 2 == 0;
        int chunkAmount = (requiredAmountOfButtons - (isRound ? 2 : 1)) / 2;

        var enumerableCurrentState = GetRequiredButtonsAndTargetPositions().Skip(isRound ? 2 : 1);

        for (int i = 0; i < chunkAmount; i++)
        {
            /// _co[i] should be set to _co[i+1] becuase _co[0] is the displaycontainersRoutine.  
            if (_co[i+1] != null)
            {
                StopCoroutine(_co[i + 1]);
            }

            _co[i+1] = CRHelper.MoveRoutine(rtInfos: enumerableCurrentState.Skip(i*2).Take(2), 
                                            lerpDuration: 1f, 
                                            followingAction: i==chunkAmount-1 ? () => Tcs.TrySetResult(true) : null );
            StartCoroutine(_co[i + 1]);

            yield return TimeTickSystem.WaitForSeconds_One;
        }
        _co[0] = null;

        //Tcs.TrySetResult(true);

       /*_co[1] = CRHelper.MoveRoutine(rtInfos: GetRequiredButtonsAndTargetPositions(), lerpDuration: 1f, followingAction: () => Debug.LogWarning("Routine Done!"));
        StartCoroutine(_co[0]);*/
    }

    public void HideContainers()
    {
        Tcs = new(false);

        if (_co[0] != null)
        {
            StopCoroutine(_co[0]);
            _co[0] = null;
            Debug.LogWarning("stopping HideContainers ");
        }

        _co[0] = HideContainersRoutine();
        StartCoroutine(_co[0]);
    }

    private IEnumerator HideContainersRoutine()
    {
        bool isRound = requiredAmountOfButtons % 2 == 0;
        int chunkAmount = (requiredAmountOfButtons - (isRound ? 2 : 1)) / 2;

        var enumerableCurrentState = GetRequiredButtonsAndTargetPositions()
                                            .Skip(isRound ? 2 : 1)
                                            .Reverse()
                                            .Select(rbtp => (rbtp.rts, targetPos: new Vector2(0, rbtp.rts.anchoredPosition.y)) );

        for (int i = chunkAmount; i > 0; i--)
        {
            /// Here the +1 on the index used in DisplayContainers() is not necessary becuase it's an inverse iteration 
            if (_co[i] != null)
            {
                StopCoroutine(_co[i]);
            }

            _co[i] = CRHelper.MoveRoutine(rtInfos: enumerableCurrentState.Skip((chunkAmount - i) * 2).Take(2),
                                                   lerpDuration: 1f,
                                                   followingAction: null); //i == 1 ? () => Tcs.TrySetResult(true) : null);

            StartCoroutine(_co[i]);

            yield return TimeTickSystem.WaitForSeconds_One;
        }
        _co[0] = null;
    }

    public void UnloadAndDeallocate()
    {
        propIconContentDisplay.Unload();
        foreach (var button in buildOptionsButtons)
        {
            button.UnloadButton();
        }
    }


}
