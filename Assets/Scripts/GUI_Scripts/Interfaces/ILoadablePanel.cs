using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadablePanel
{
    void LoadPanel(PanelLoadData panelLoadData);
}




public interface ILoadable<in T_LoadInfo>
    where T_LoadInfo : ContentDisplayInfo
{
    void Load(T_LoadInfo info);

}

