using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryPanel_Capacity : MonoBehaviour    // Is this necessary ANYMORE 
{
    #region Singleton Syntax
    private static InventoryPanel_Capacity _instance;
    public static InventoryPanel_Capacity Instance { get { return _instance; } }
    private static readonly object _lock = new object();

    #endregion

    [SerializeField] private TextMeshProUGUI inventoryCapacity;


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
                    _instance = this; // LATER TO ADD DONTDESTROY ON LOAD
                }
            }
        }

    }


    public void SetInventoryCapacity(int newAmount)
    {
       // inventoryCapacity.text = string.Format("{0} / {1}",newAmount,)
        inventoryCapacity.text = newAmount.ToString();
    }
}
