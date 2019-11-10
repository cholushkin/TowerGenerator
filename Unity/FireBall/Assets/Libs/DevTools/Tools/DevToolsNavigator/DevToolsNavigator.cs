using GameGUI;
using GameGUI.Dev;
using UnityEngine;
using UnityEngine.Assertions;


// DevToolsNavigator has list of IDevTools to navigate through
public class DevToolsNavigator : MonoBehaviour, IDevTool
{
    public GUIScreenBase DevToolsScreen; // parent screen
    public SimpleGUI FrameGUI; // frame inside DevToolsScreen

    public int CurrentIndex;
    private IDevTool _curTool;
    private GUIScreenBase _curToolScreen; // subscreen of the current DevTool
    private RectTransform _outputNavigationRoot;

    public IDevTool GetCurrentTool()
    {
        return _curTool;
    }

    public void NavigateTo(int index )
    {
        if(_curTool != null)
            PopCurrentTool();

        // create new
        FrameGUI.PushScreen(FrameGUI.Screens[CurrentIndex].name);
        _curToolScreen = FrameGUI.GetCurrentScreen();
        Assert.IsNotNull(_curToolScreen);

        _curTool = _curToolScreen.GetComponent<IDevTool>();

        _curTool.OnDevToolCreate();
        _curTool.OnDevToolNavigateTo();
    }

    protected void NavigatePrev()
    {
        PopCurrentTool();
        CurrentIndex--;
        if (CurrentIndex < 0)
            CurrentIndex = FrameGUI.Screens.Length - 1;
        NavigateTo(CurrentIndex);
    }

    protected void NavigateNext()
    {
        PopCurrentTool();
        CurrentIndex++;
        if (CurrentIndex >= FrameGUI.Screens.Length)
            CurrentIndex = 0;
        NavigateTo(CurrentIndex);
    }

    private void PopCurrentTool()
    {
        FrameGUI.PopScreen(_curToolScreen.name);
    }

    #region Button handlers
    public void OnPrevButtonTap()
    {
        Debug.Log("OnPrevButtonTap");
        NavigatePrev();
    }

    public void OnNextButtonTap()
    {
        Debug.Log("OnNextButtonTap");
        NavigateNext();
    }

    public void OnCollapseButtonTap()
    {
        Debug.Log("OnCollapseButtonTap");
    }

    public void OnButtonPanelButton1Tap()
    {
        Debug.Log("OnButtonPanelButton1Tap");
    }

    public void OnButtonPanelButton2Tap()
    {
        Debug.Log("OnButtonPanelButton2Tap");
    }

    public void OnButtonPanelButton3Tap()
    {
        Debug.Log("OnButtonPanelButton3Tap");
    }

    public void OnExitButtonTap()
    {
        // todo: pop all modals of DevToolsScreen.SimpleGui
        //DevToolsScreen.SimpleGui.PopScreen("Screen.DevTools");
        //DevToolsScreen.SimpleGui.PushScreen("Screen.DevHUD");
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
