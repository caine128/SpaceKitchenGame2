using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVariableButtonPanel
{
    RectTransform[] PopupButtons_RT { get; }
    Vector2[] PopupButtons_OriginalLocations { get; }

    void ConfigureVariableButtons();

    void SetButtonLayout(int buttonAmountToDisplay)
    {
        if(buttonAmountToDisplay > PopupButtons_RT.Length)
        {
            Debug.LogWarning("there is no so many buttons on this Panel !");
            return;
        }

        var button0 = PopupButtons_RT[0].gameObject;
        switch (buttonAmountToDisplay)
        {
            case 1:
                if (button0.activeInHierarchy != false) button0.SetActive(false);
                if (PopupButtons_RT[1].anchoredPosition.x != 0) PopupButtons_RT[1].anchoredPosition = new Vector2(0, PopupButtons_RT[1].anchoredPosition.y);
                break;
            case 2:
                if (button0.activeInHierarchy != true) button0.SetActive(true);
                for (int i = 0; i < PopupButtons_RT.Length; i++)
                {
                    PopupButtons_RT[i].anchoredPosition = PopupButtons_OriginalLocations[i];
                }
                break;
            default:
                Debug.LogWarning("Default Statement Should'nt be Called");
                break;
        }
    }
    
}
