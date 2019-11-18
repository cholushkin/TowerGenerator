using System.Collections;
using System.Collections.Generic;
using GameLib.Random;
using UnityEngine;
using UnityEngine.Assertions;

public class GroupStack : Group
{
    public int LayerIndexSelected;

    public override void SetDefaultValues()
    {
        base.SetDefaultValues();
    }

    public override void DoRndChoice(ref RandomHelper rnd)
    {
        DisableItems();
        LayerIndexSelected = rnd.FromRangeIntInclusive(0, GetAmountOfTransformImpact() - 1);
        for (int i = 0; i <= LayerIndexSelected; ++i)
            transform.GetChild(i).gameObject.SetActive(true);
    }

    public override void DoRndMinimalChoice(ref RandomHelper rnd)
    {
        DoChoice(0);
    }

    public void DoChoice(int layer)
    {
        Assert.IsTrue(layer >= 0);
        Assert.IsTrue(layer < GetAmountOfTransformImpact());

        DisableItems();
        LayerIndexSelected = layer;
        for (int i = 0; i <= layer; ++i)
            transform.GetChild(i).gameObject.SetActive(true);
    }
}
