using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEventListeners : MonoBehaviour
{

    [SerializeField]
    private UnityEngine.GameObject customerSpawner;

    private void OnEnable ()
    {
        customerSpawner.GetComponent<CustomerSpawner> ().onNewCustomerSpawned += DisplayGreetingMessage;
    }

    private void OnDisable ()
    {
        if (customerSpawner)
        {
            customerSpawner.GetComponent<CustomerSpawner> ().onNewCustomerSpawned -= DisplayGreetingMessage;
        }

    }

    private void DisplayGreetingMessage (object sender, CustomerSpawner.OnNewCustomerSpawnedEventArgs e)
    {
        Debug.Log ("First Customer Arrived");
        e.newCustomer.GetComponent<CustomerOnClick> ().onFoodSold += DisplaySellingMessage;
    }

    private void DisplaySellingMessage(object sender, CustomerOnClick.OnFoodSoldEventArgs e)
    {
        Debug.Log ("Order processed " + e.gold + " gold gained");        
    }

}
