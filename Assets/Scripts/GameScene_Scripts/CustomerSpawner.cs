using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CustomerSpawner : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.GameObject customer;

    public event EventHandler<OnNewCustomerSpawnedEventArgs> onNewCustomerSpawned;

    public class OnNewCustomerSpawnedEventArgs
    {
        public UnityEngine.GameObject newCustomer;
    }

    private void Start ()
    {
        UnityEngine.GameObject newCustomer = Instantiate(customer, new Vector3(-22.5f,1,-67.5f), transform.rotation);
        onNewCustomerSpawned?.Invoke (this, new OnNewCustomerSpawnedEventArgs { newCustomer = newCustomer });
    }

    

}
