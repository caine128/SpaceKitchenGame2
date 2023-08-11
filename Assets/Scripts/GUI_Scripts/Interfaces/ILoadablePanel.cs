using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoadablePanel
{
    //void LoadPanel(object mainLoadInfo, params object[] extraLoadInfo);
    void LoadPanel(PanelLoadData panelLoadData);
    //void LoadPanel<T_BlueprintType>(PanelLoadData<T_BlueprintType> panelLoadData)
      //  where T_BlueprintType : SortableBluePrint;
}

/*
public interface ILoadablePanelOfType
{
    void LoadPanel<T_BlueprintType>(object mainLoadInfo, params object[] extraLoadInfo) 
        where T_BlueprintType : SortableBluePrint;

    void LoadPanel<T_BlueprintType>(PanelLoadData<T_BlueprintType> panelLoadData)
        where T_BlueprintType: SortableBluePrint;
}*/


public interface ILoadable<in T_LoadInfo>
    where T_LoadInfo : ContentDisplayInfo
{
    void Load(T_LoadInfo info);

}

