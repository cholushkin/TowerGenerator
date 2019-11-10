using GameGUI.Dev;
using UnityEngine;

public class DevSheetEditor : MonoBehaviour, IDevTool
{
    #region input handlers

    public void OnChangeModeButtonTap()
    {
        Debug.Log("OnChangeModeButtonTap");
    }

    public void OnOrientationButtonTap()
    {
        Debug.Log("OnOrientationButtonTap");
    }

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
