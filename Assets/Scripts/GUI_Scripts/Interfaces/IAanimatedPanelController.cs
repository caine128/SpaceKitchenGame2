
using System.Collections;
using System.Collections.Generic;

public interface IAanimatedPanelController
{
    public IEnumerator[] CO { get; }
    public GUI_LerpMethods PanelToAwait { get; }

    void DisplayContainers();
    void HideContainers();
}


public interface IAanimatedPanelController_Cancellable  : IAanimatedPanelController
{
    public bool IsAnimating { get; set; }
    void FastForwardDisplayAnimation();
}
