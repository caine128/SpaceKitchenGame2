
using UnityEngine.EventSystems;

public class SkipButton : ExitButton
{
    public sealed override void OnPointerUp(PointerEventData eventData)
    {
        /// In case of the Modal Panel The Exit Button Is Considered As Skip (Since the modalpanelQueue can Hold a lot of enumerable info waiting
        /// to be enumerated.
        if (PanelToInvoke.MainPanel is Information_Modal_Panel information_Modal_Panel)
        {
            information_Modal_Panel.ModalLoadDataQueue.Clear();
        }

        PanelManager.DeactivatePanel(PanelToInvoke, nextPanelLoadAction_IN: null);
    }
}
