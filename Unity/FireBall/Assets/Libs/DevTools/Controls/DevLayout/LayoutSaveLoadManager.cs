using System;
using System.Collections.Generic;
using System.Linq;
using DevTools;
using GameGUI;
using GameGUI.Dev;
using GameLib;
using GameLib.Log;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class LayoutSaveLoadManager : MonoBehaviour
{
    public InputField fileNameInputField;
    //public LayoutPanel LayoutPanel;
    //public PresetList PresetList;

    [Serializable]
    public class LayoutPreset
    {
        [Serializable]
        public class SaveItem
        {
            public string name;
            public Vector2i Coord;
        }

        public List<SaveItem> Slots = new List<SaveItem>();
        public Vector2i Size;
        public string LayoutName;
    }

    [Serializable]
    public class Layouts
    {
        public List<LayoutPreset> Presets = new List<LayoutPreset>();
    }

    public LogChecker LogChecks;

    private const string KeyNameLayouts = "Layouts";
    internal Layouts _layouts;

    private SimpleGUI _gui;


    public void Awake()
    {
        _gui = GetComponentInParent<SimpleGUI>();
        Assert.IsNotNull(_gui);

        Assert.IsNotNull(fileNameInputField);
        //Assert.IsNotNull(LayoutPanel);
        //Assert.IsNotNull(PresetList);
        LoadLayouts();
    }


    public void OnSaveButtonClick()
    {
        if (LogChecks.Normal())
            Debug.Log("Saving to " + fileNameInputField.textComponent.text);

        var preset = CreatePresetFromCurrentState();
        if (preset == null)
        {
            if (LogChecks.Important())
                Debug.LogError("Didn't save anything");
            return;
        }

        RefreshPreset(preset);
        SaveLayouts();
        SetLayout(preset);
        LoadLayouts();
    }


    public void OnLoadButtonClick()
    {
        if (LogChecks.Normal())
            Debug.LogFormat("Loading layout preset");

        var presetName = fileNameInputField.textComponent.text;
        var preset = _layouts.Presets.FirstOrDefault(p => p.LayoutName == presetName);
        if (preset == null)
            return;

        //LayoutPanel.Load(preset);
        SetLayout(preset);
    }

    public void OnLayoutPresetItemClick(GameObject sender)
    {
        fileNameInputField.text = sender.name;
    }



    private void SetLayout(LayoutPreset layout)
    {
        if (LogChecks.Normal())
            Debug.LogFormat("Setting layout '{0}' as current", layout.LayoutName);

        //var gui = GetComponentInParent<SimpleGUI>();
        //var screenDevHUD = gui.ObtainScreen("Screen.DevHUD");
        //screenDevHUD.gameObject.GetComponent<ScreenDevHUD>().SetLayout(layout);
    }


    #region manage layouts
    private void SaveLayouts()
    {
        var jsonData = JsonUtility.ToJson(_layouts);
        PlayerPrefs.SetString(KeyNameLayouts, jsonData);
    }

    public void LoadLayouts()
    {
        //var layouts = PlayerPrefs.GetString(KeyNameLayouts);
        //if (!string.IsNullOrEmpty(layouts))
        //{
        //    _layouts = JsonUtility.FromJson<Layouts>(layouts);
        //    PresetList.Clear();
        //    foreach (var preset in _layouts.Presets)
        //        PresetList.Add(preset);
        //}
        //else
        //{
        //    _layouts = new Layouts();
        //}
    }

    private LayoutPreset CreatePresetFromCurrentState()
    {
        //var presetName = fileNameInputField.textComponent.text;
        //if (string.IsNullOrEmpty(presetName))
        //{
        //    if (LogChecks.Important())
        //        Debug.LogErrorFormat("The name of preset to save is empty. Please specify valid name for your configuration");

        //    return null;
        //}

        //var layoutPreset = new LayoutPreset
        //{
        //    Size = new Vector2i(LayoutPanel.Grid.Columns, LayoutPanel.Grid.Rows),
        //    LayoutName = presetName
        //};

        //if (LayoutPanel.Grid.Cells != null)
        //    foreach (var cell in LayoutPanel.Grid.Cells)
        //    {
        //        var c = cell.GetComponent<GridCell>();
        //        var saveitem = new LayoutPreset.SaveItem
        //        {
        //            Coord = c.GetPosition(),
        //            name = c.TextNameOfControl.text
        //        };
        //        layoutPreset.Slots.Add(saveitem);
        //    }

        //return layoutPreset;
        return null;
    }

    private void RefreshPreset(LayoutPreset preset)
    {
        Assert.IsNotNull(preset, "Preset shouldn't be null");
        Assert.IsNotNull(_layouts, "_layouts shouldn't be null");

        if (LogChecks.Normal())
            Debug.LogFormat("Refreshing preset '{0}'", preset.LayoutName);

        var existingPreset = _layouts.Presets.FirstOrDefault(l => l.LayoutName == preset.LayoutName);
        if (existingPreset != null)
        {
            if (LogChecks.Important())
                Debug.LogFormat("Preset with such name ('{0}') already exists, rewriting...", preset.LayoutName);
            _layouts.Presets.Remove(existingPreset);
        }
        _layouts.Presets.Add(preset);
    }

    public void DeletePreset(string presetName)
    {
        var existingPreset = _layouts.Presets.FirstOrDefault(l => l.LayoutName == presetName);
        Assert.IsNotNull(existingPreset);
        DeletePreset(existingPreset);
    }

    private void _deletePreset(LayoutPreset preset)
    {
        Assert.IsNotNull(preset);
        if (LogChecks.Important())
            Debug.LogFormat("Deleting '{0}' preset", preset.LayoutName);
        _layouts.Presets.Remove(preset);
        SaveLayouts();
        LoadLayouts();
    }

    public void DeletePreset(LayoutPreset preset)
    {
        //_gui.PushScreen("Screen.Dialog.YesNo", true);
        //var dlg = _gui.PeekScreen() as ScreenDialogYesNo;
        //Assert.IsNotNull(dlg);
        //Assert.IsNotNull(preset);

        //dlg.Set(
        //    new List<string> { "Yes", "No" },
        //    new List<Action> { () => _deletePreset(preset), null },
        //    "Confirm delete",
        //    string.Format("Are you sure you would like to delete '{0}' layout preset?", preset.LayoutName),
        //    "Note: This action you can not undo. So be careful! 0_0");
    }

    #endregion
}
