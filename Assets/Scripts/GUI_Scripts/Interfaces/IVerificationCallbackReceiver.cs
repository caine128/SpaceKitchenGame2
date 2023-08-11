using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IVerificationCallbackReceiver 
{
    public void Subscribe(bool shouldSubscribe);
    public void VerificationCallback(bool isVerified);
    
}
