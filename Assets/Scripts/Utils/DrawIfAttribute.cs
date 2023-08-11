using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Struct | AttributeTargets.Class |AttributeTargets.All, AllowMultiple =true, Inherited =true)]
public class DrawIfAttribute : PropertyAttribute
{
    #region fields
    public string comparedPropertyName { get; private set; }
    public object comparedValue { get; private set; }
    public DisablingType disablingType { get; private set; }
    #endregion

    public enum DisablingType
    {
        ReadOnly =2,
        DontDraw =3,
    }

    public DrawIfAttribute(string comparedPropertyName, object comparedValue, DisablingType disablingType = DisablingType.ReadOnly)
    {
        this.comparedPropertyName = comparedPropertyName;
        this.comparedValue = comparedValue;
        this.disablingType = disablingType;
    }


}
