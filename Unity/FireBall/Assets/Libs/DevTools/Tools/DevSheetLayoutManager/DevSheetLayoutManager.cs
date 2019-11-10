using UnityEngine;

namespace GameGUI.Dev
{
    public class DevSheetLayoutManager : MonoBehaviour, IDevTool
    {
        #region input handlers

        public void OnContextMenuTap()
        {
            Debug.Log("OnContextMenuTap");
        }

        #endregion


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