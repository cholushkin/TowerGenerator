using TMPro;
using UnityEngine;

namespace GameGUI.Dev
{
    public class DevMenuButton : MonoBehaviour, IDevTool
    {
        public TextMeshProUGUI Label1;
        public TextMeshProUGUI Label2;

        private void InitStatusString()
        {
            Label1.text = Application.productName;
            Label2.text = $"{Application.unityVersion}|{Application.productName}";
        }

        #region IDevTool
        public void OnDevToolCreate()
        {
            throw new System.NotImplementedException();
        }

        public void OnDevToolNavigateTo()
        {
            throw new System.NotImplementedException();
        }

        public void OnDevToolShow()
        {
            throw new System.NotImplementedException();
        }
        #endregion
    }
}