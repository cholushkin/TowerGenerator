using GameGUI;
using GameLib.Time;
using UnityEngine;

namespace DevTools
{
    public class ScreenDevTools : GUIScreenBase
    {
        public DevToolsNavigator DevToolNavigator;

        public override void StartAppearAnimation()
        {
            base.StartAppearAnimation();

            PauseForDevMenu(true);

            // navigate
            var tool = DevToolNavigator.GetCurrentTool();
            if (tool == null)
                DevToolNavigator.NavigateTo(0);
            else
                tool.OnDevToolShow();
        }

        public override void StartDisappearAnimation()
        {
            base.StartDisappearAnimation();
            PauseForDevMenu(false);
        }


        private static int _pauseLevel;
        public static void PauseForDevMenu(bool flag)
        {
            if (flag)
                _pauseLevel = TimeScaleStack.Instance.Push(0f);
            else
                TimeScaleStack.Instance.Pop(_pauseLevel);
        }
    }
}