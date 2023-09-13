
using System;
using System.Collections;
using System.Collections.Generic;

public interface IAnimatedPanelController
{
    public IEnumerator[] CO { get; }
    public GUI_LerpMethods PanelToAwait { get; }
    void DisplayContainers();
}

public interface IAnimatedPanelController_ManualHide : IAnimatedPanelController
{  
    void HideContainers();
}

public interface IAnimatedPanelController_SelfDeactivate : IAnimatedPanelController
{
    public string SelfDeactivateCallerMemberName { get;}
    void SelfDeactivatePanel(Action beforeDeactivate = null, 
                                    Action unloadAction = null,
                                    Action nextPanelLoadAction = null,
                                    params Action[] extaLoadActions);
}

public interface IAnimatedPanelController_ManualHide_Cancellable  : IAnimatedPanelController_ManualHide
{
    public bool IsAnimating { get; set; }
    void FastForwardDisplayAnimation();
}
