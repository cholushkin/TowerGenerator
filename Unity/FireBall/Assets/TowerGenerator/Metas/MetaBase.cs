using System;
using System.Collections.Generic;
using UnityEngine;

public class MetaBase : ScriptableObject
{
    public Type EnType;
    public string EntName;
    public uint Generation;
    // biome tags
    // user tags
    public List<Bounds> BBs;
}

