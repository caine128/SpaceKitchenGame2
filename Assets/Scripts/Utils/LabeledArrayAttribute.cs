using System;
using UnityEngine;

public class LabeledArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public LabeledArrayAttribute(Type enumType)
    {
        names = Enum.GetNames(enumType);
    }
    public LabeledArrayAttribute(string[] names)
    {
        this.names = names;
    }
}