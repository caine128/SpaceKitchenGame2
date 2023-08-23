using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildOptionsPanel_Manager : Panel_Base
{
    [SerializeField] private BuildOptionsButton[] buildOptionsButtons;
    public void LoadPanel(Prop prop)
    {
        switch (prop.ShopUpgradeBluePrint) // TODO : Check if is a purchased one, maybe it's not the "fixed" parameter
        {
            case WorkStationUpgrade:
                buildOptionsButtons[0].SetupButton(ButtonFunctionType.BuildOptionsPanel.None);
                buildOptionsButtons[1].SetupButton(ButtonFunctionType.BuildOptionsPanel.Apply);
                buildOptionsButtons[2].SetupButton(ButtonFunctionType.BuildOptionsPanel.Rotate);
                break;
            default:
                break;
        }
    

    }
}
