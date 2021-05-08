using System.Linq;
using GameLib;
using ResourcesHelper;
using UnityEngine;
using UnityEngine.Assertions;

public class DynamicGridGeneratorCustom : DynamicGridGenerator
{
    public PrefabHolder ControlPrefabs;

    public void RegenerateGrid(LayoutSaveLoadManager.LayoutPreset preset)
    {
        Assert.IsNotNull(preset);
        ClearGrid();

        if (preset.Slots.Count == 0)
            return;

        ControlPrefabs = GetComponent<PrefabHolder>();

        Assert.IsNotNull(ControlPrefabs);

        RectTransform rowParent;
        for (int rowIndex = 0; rowIndex < Rows; rowIndex++)
        {
            rowParent = Instantiate(HorizontalLayoutGroup);
            rowParent.transform.SetParent(VerticalLayoutGroupRoot);
            rowParent.transform.localScale = Vector3.one;

            for (int colIndex = 0; colIndex < Columns; colIndex++)
            {
                var slot = preset.Slots.FirstOrDefault(s => s.Coord == new Vector2i(colIndex, rowIndex));
                var prefabNameToSpawn = "SlotPlaceholder";
                if (!string.IsNullOrEmpty(slot.name))
                    prefabNameToSpawn = slot.name;

                var controlPrefab = ControlPrefabs.GetPrefab(prefabNameToSpawn);
                CreateCell(rowParent, controlPrefab, colIndex, rowIndex);
            }
        }
    }
}
