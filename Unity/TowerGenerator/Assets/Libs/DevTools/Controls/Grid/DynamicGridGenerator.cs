using System.Collections.Generic;
using GameLib;
using UnityEngine;
using UnityEngine.Assertions;

public class DynamicGridGenerator : MonoBehaviour
{
    public int Rows;
    public int Columns;
    public RectTransform HorizontalLayoutGroup;
    public GameObject PrefabGridCell;
    public Transform VerticalLayoutGroupRoot;
    public List<GameObject> Cells { get; set; }

    public void ClearGrid()
    {
        Cells = new List<GameObject>();
        for (int count = 0; count < VerticalLayoutGroupRoot.childCount; count++)
            Destroy(VerticalLayoutGroupRoot.GetChild(count).gameObject);
    }

    public virtual void RegenerateGrid() 
    {
        ClearGrid();

        RectTransform rowParent;
        for (int rowIndex = 0; rowIndex < Rows; rowIndex++)
        {
            rowParent = Instantiate(HorizontalLayoutGroup);
            rowParent.transform.SetParent(VerticalLayoutGroupRoot);
            rowParent.transform.localScale = Vector3.one;

            if (PrefabGridCell != null)
                for (int colIndex = 0; colIndex < Columns; colIndex++)
                    CreateCell(rowParent, PrefabGridCell, colIndex, rowIndex);
        }
    }

    protected void CreateCell(RectTransform parent, GameObject controlPrefab, int x, int y)
    {
        Assert.IsNotNull(controlPrefab);
        var control = Instantiate(controlPrefab);
        var gridCell = control.GetComponent<IGridCell>();
        Assert.IsNotNull(gridCell);
        gridCell.SetPosition(new Vector2i(x,y));
        control.transform.SetParent(parent);
        control.transform.localScale = Vector3.one;
        Cells.Add(control);
    }
}
