using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaBase
{
    public string EntName;
    public uint Generation;
    // biome tags
    // user tags
    public List<Bounds> BBs;
}

public class MetaChunkStd : MetaBase
{
}

public class MetaCreature : MetaBase
{ }


