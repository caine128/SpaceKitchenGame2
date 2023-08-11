using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Crafted_InvisibleItems_Counter : MonoBehaviour, IConfigurablePanel
{
    #region Singleton Syntax
    private static Crafted_InvisibleItems_Counter _instance;
    public static Crafted_InvisibleItems_Counter Instance { get { return _instance; } }
    private static readonly object _lock = new object();
    #endregion

    private Image image;
    private TextMeshProUGUI notificationText;
    [SerializeField] private int invisibleItemsAmount = 0; 

    private IEnumerator co;
    private bool cr_Running = false;
    private WaitForSeconds waitForSeconds = new WaitForSeconds(.05f);

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            lock (_lock)
            {
                if (Instance == null)
                {
                    _instance = this; 
                    //DontDestroyOnLoad(this.gameObject);
                }
            }
        }
    }

    public void PanelConfig()
    {
        image = GetComponent<Image>();
        notificationText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
    }

    public void CountInvisibleReadyItems(bool canIncrement)
    {
        invisibleItemsAmount = canIncrement == true ? invisibleItemsAmount + 1 : invisibleItemsAmount - 1;
        SetTextAndPanelCall();
    }

    private void SetTextAndPanelCall()
    {
        if (cr_Running)
        {
            StopCoroutine(co);
        }
        co = SetTextAndPanelRoutine();
        StartCoroutine(co);
    }

    IEnumerator SetTextAndPanelRoutine()
    {
        cr_Running = true;

        yield return waitForSeconds;
        notificationText.text = invisibleItemsAmount.ToString();

        
        if (invisibleItemsAmount<=0 && image.enabled == true)
        {
            image.enabled = notificationText.enabled = false;
        }
        else if(invisibleItemsAmount > 0 && image.enabled == false)
        {
            image.enabled = notificationText.enabled = true;
        }

        cr_Running = false;
    }


}
