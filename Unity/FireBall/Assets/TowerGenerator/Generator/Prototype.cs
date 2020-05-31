using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prototype : MonoBehaviour
{
    public int SeedTopology;
    public int SeedVisual;
    public int SeedContent;

    void Reset()
    {
        SeedTopology = -1;
        SeedVisual = -1;
        SeedContent = -1;
    }
}
