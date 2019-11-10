using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : BasePropImporter
{
    public int Index = -1;

    public override bool SetProp(string propName, object value) // return false if cannot find property 
    {
        var baseResult = base.SetProp(propName, value);
        if (propName == "Index")
        {
            Index = (int)value;
            return true;
        }
        return baseResult;
    }

    public override void SetDefaultValues()
    {
        Index = GetMyLayer();
    }
}