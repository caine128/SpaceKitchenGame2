using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NamedAttributeElements : PropertyAttribute
{
    public readonly string[] names;

    public NamedAttributeElements(string[] names)
    {
        this.names = names;
    }
}
