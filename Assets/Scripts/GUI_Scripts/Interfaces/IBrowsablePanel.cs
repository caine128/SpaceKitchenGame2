using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBrowsablePanel<T_BluePrint>
    where T_BluePrint : class
{
    BrowserButton<T_BluePrint>[] BrowserButtons{ get; } 
    int CurrentIndice { get; }  
    List<T_BluePrint> ListToIterate { get; }
    void BrowseInfo(T_BluePrint blueprint_IN);
    int SetCurrentIndice(T_BluePrint blueprint_IN)
    {
        int currentIndice = (default);
        for (int i = 0; i < ListToIterate.Count; i++)
        {
            if (ListToIterate[i].Equals(blueprint_IN))
            {
                currentIndice = i;
                return currentIndice;
            }
        }
        Debug.LogWarning("Match Not Found, Shouldn't come to this line");
        return currentIndice;
    }

    void InitialConfigBrowserButtons();
    void SetVisibilityBrowserButtons()
    {
        if (ListToIterate.Count > 1)
        {
            if (CurrentIndice == 0) 
            {
                if(BrowserButtons[0].gameObject.activeSelf != false) BrowserButtons[0].gameObject.SetActive(false);
                if(BrowserButtons[1].gameObject.activeSelf != true) BrowserButtons[1].gameObject.SetActive(true);
            }
            else if (CurrentIndice == ListToIterate.Count - 1)
            {
                if (BrowserButtons[0].gameObject.activeSelf != true) BrowserButtons[0].gameObject.SetActive(true);
                if (BrowserButtons[1].gameObject.activeSelf != false) BrowserButtons[1].gameObject.SetActive(false);
            }                     
            else
            {
                foreach (var browserButton in BrowserButtons)
                {
                    if (browserButton.gameObject.activeSelf != true) browserButton.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            foreach (var browserButton in BrowserButtons)
            {
                if (browserButton.gameObject.activeSelf != false) browserButton.gameObject.SetActive(false);
            }
        }
    }
}
