using System;

//public interface IPanelInvokeButton
//{
//    public event Action<InvokablePanelController, Action> OnInvokeButtonPressed;
//}


public interface ISinglePanelInvokeButton 
{
    public InvokablePanelController PanelToInvoke { get; }
    //void InvokePanel(Action panelLoadAction);   
}


public interface IMultiPanelInvokeButton 
{
    public InvokablePanelController[] InvokablePanels { get; }

    //public event Action<InvokablePanelController> OnInvokeButtonPressed;
    //void SelectPanelToInvoke(int invokablePanelIndex_IN);
   // void InvokePanel(int invokablePanelIndex_IN, Action panelLoadAction);
}