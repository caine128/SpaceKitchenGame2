using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Detect_CharacterInfoClick : DetectClickRequest<Character>
{
    public override void OnPointerUp(PointerEventData eventData)
    {
        if(isValidClick && (Vector2.Distance(initialClickPosition, eventData.position) <= PanelManager.MAXCLICKOFFSET))
        {
            if(initialSelection is CharacterContainer characterContainer)
            {
                characterContainer.Tintsize();
                var bluePrint = characterContainer.bluePrint;


                PanelLoadData panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);
                /*InvokablePanelController panelToInvoke = bluePrint.isHired 
                                                            ? PanelManager.InvokablePanels[typeof(CharactersInfoPanel_Manager)]
                                                            : PanelManager.InvokablePanels[typeof(HireCharacter_Panel)];*/

                PanelManager.ActivateAndLoad(invokablePanel_IN: bluePrint.isHired
                                                                 ? PanelManager.InvokablePanels[typeof(CharactersInfoPanel_Manager)]
                                                                 : PanelManager.InvokablePanels[typeof(HireCharacter_Panel)],
                                             panelLoadAction_IN: bluePrint.isHired
                                                                 ? () => CharactersInfoPanel_Manager.Instance.LoadPanel(panelLoadData)
                                                                 : () => HireCharacter_Panel.Instance.LoadPanel(panelLoadData));
                /*switch (bluePrint.isHired)
                {
                    case true:
                        panelToInvoke = PanelManager.InvokablePanels[typeof(CharactersInfoPanel_Manager)];
                        panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);

                        PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                                     panelLoadAction_IN: () => CharactersInfoPanel_Manager.Instance.LoadPanel(panelLoadData));
                        break;

                    case false:
                        panelToInvoke = PanelManager.InvokablePanels[typeof(HireCharacter_Panel)];
                        panelLoadData = new PanelLoadData(mainLoadInfo: bluePrint, panelHeader: null, tcs_IN: null);
                        PanelManager.ActivateAndLoad(invokablePanel_IN: panelToInvoke,
                                                     panelLoadAction_IN: () => HireCharacter_Panel.Instance.LoadPanel(panelLoadData));
                        break;
                }*/

                

                
            }
        }
    }
}
