//#define DISPLAY_FPS


using System.Net.Mime;
using GameGUI;
using GameLib.Time;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

// provides grid to display the stuff on top of game HUD

namespace DevTools
{
    public class ScreenDevHUD : GUIScreenBase
    {
        public Text StatusString;

        //        public DynamicGridGeneratorCustom ControlsGrid;
        //private DevToolsNavigator DevToolsNavigator;

        public override void Awake()
        {
            base.Awake();
            InitStatusString();
        }

        public void OnDevButtonTap()
        {
            SimpleGui.PopScreen("Screen.DevHUD"); 
            SimpleGui.PushScreen("Screen.DevTools");
        }

        //        public void SetLayout(LayoutSaveLoadManager.LayoutPreset layoutSaveData)
        //        {
        //            ControlsGrid.Columns = layoutSaveData.Size.X;
        //            ControlsGrid.Rows = layoutSaveData.Size.Y;
        //            ControlsGrid.RegenerateGrid(layoutSaveData);
        //        }

        void InitStatusString()
        {
            StatusString.text = string.Format("{0}\n{1}\n{2}",
                Application.productName,
                Application.version, 
                Application.unityVersion
            );
        }
    }
}
