using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;

public class BackgroudPanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Singleton Syntax
    private static BackgroudPanel _instance;
    public static BackgroudPanel Instance { get { return _instance; } }
    private static readonly object _lock = new object();
    #endregion

   /* public Adressable_SpriteDisplay BackgroundImage
    {
        get => _backgroundImage;
    }*/
    //[SerializeField] private Adressable_SpriteDisplay _backgroundImage;
    [SerializeField] private AdressableImage _backgroundImage_Adressable;
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

    public GUI_LerpMethods_Color GUI_LerpMethods_Color { get { return gUI_LerpMethods_Color; } }
    [SerializeField] private GUI_LerpMethods_Color gUI_LerpMethods_Color;

    public void UpdateBackgroundPanel()
    {
        var isThereAnyActivePanel = PanelManager.SelectedPanels.TryPeek(out InvokablePanelController invokablePanelController);
        var adressableSpriteToLoad = ImageManager.SelectSprite(isThereAnyActivePanel
                                ? invokablePanelController.MainPanel.GetType().ToString()
                                : null);

        _backgroundImage_Adressable.preserveAspect = _backgroundImage_Adressable.preserveAspect == false
                                                       ? _backgroundImage_Adressable.preserveAspect
                                                       : false;

        switch (isThereAnyActivePanel, this.gameObject.activeInHierarchy)
        {
            
            case (true, _) when (!gameObject.activeInHierarchy ||gUI_LerpMethods_Color.RunningCoroutine is not null)
                   && invokablePanelController.MainPanel is not ShopPanel_Manager or  BuildOptionsPanel_Manager:

                this.gameObject.SetActive(true);
                gUI_LerpMethods_Color.ColorLerpInitialCall(lerpedColor: adressableSpriteToLoad is not null
                                                                             ? PanelManager.BackgroundPanel_LerpColor_WithImage
                                                                             : PanelManager.BackgroundPanel_LerpColor_NoImage,
                                                           adressableAction: adressableSpriteToLoad is not null
                                                                             ? () => _backgroundImage_Adressable.LoadSprite(adressableSpriteToLoad)
                                                                             : null);
                break;
            
            case (true, true) when !_backgroundImage_Adressable.IsLoadedSpriteRefSameWith(adressableSpriteToLoad):

                gUI_LerpMethods_Color.ColorLerpInitialCall(lerpedColor: adressableSpriteToLoad is not null
                                                                             ? PanelManager.BackgroundPanel_LerpColor_WithImage
                                                                             : PanelManager.BackgroundPanel_LerpColor_NoImage,
                                                            adressableAction: adressableSpriteToLoad is not null
                                                                             ? () => _backgroundImage_Adressable.LoadSprite(adressableSpriteToLoad)
                                                                             : () => _backgroundImage_Adressable.UnloadSprite());
                break;

            case (false, true):
            case (true, true) when invokablePanelController.MainPanel is ShopPanel_Manager or BuildOptionsPanel_Manager:
                                   //|| invokablePanelController.MainPanel is BuildOptionsPanel_Manager:

                gUI_LerpMethods_Color.ColorLerpFinalCall(disableObject: true,
                                                         adressableAction: !_backgroundImage_Adressable.IsLoadedSpriteRefSameWith(null)
                                                                        ? () => _backgroundImage_Adressable.UnloadSprite()
                                                                        : null);
                break;
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        var currentPanel = PanelManager.SelectedPanels.Peek();

        if (currentPanel.MainPanel is IAanimatedPanelController_Cancellable aanimatedPanelController_Cancellable && aanimatedPanelController_Cancellable.IsAnimating)
        {
            aanimatedPanelController_Cancellable.FastForwardDisplayAnimation();
            Debug.Log("is animatiing still ? :");
        }
        else if (currentPanel.MainPanel is Information_Modal_Panel information_Modal_Panel && information_Modal_Panel.ModalLoadDataQueue.TryDequeue(out ModalLoadData result))
        {
            information_Modal_Panel.LoadModalQueue(modalLoadData_IN: result,Enumerable.Empty<ModalLoadData>());
            information_Modal_Panel.DisplayContainers();

        }
        else
        {
            UnityEngine.GameObject activePanelsExitButton = currentPanel.GetComponentInChildren<ExitButton>().gameObject;
            ExecuteEvents.Execute(activePanelsExitButton, eventData, ExecuteEvents.pointerUpHandler);
        }
    }
}
