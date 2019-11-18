using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;

public class GroupUser : Group
{
    public override void DoRndChoice(ref RandomHelper rnd)
    {
        DisableItems();
    }

    public override void DoRndMinimalChoice(ref RandomHelper rnd)
    {
        DisableItems();
    }
}
