using System;
using System.Collections.Generic;
using GameGUI;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ScreenDialogYesNo : GUIScreenBase
{
    public List<Text> ButtonTexts;
    public Text CaptionText;
    public Text MainText;
    public Text FooterText;
    private List<Action> _actions;
    private SimpleGUI _gui;

    public override void Awake()
    {
        _gui = GetComponentInParent<SimpleGUI>();
        Assert.IsNotNull(_gui);
    }

    public void Set(
        List<string> buttonNames, 
        List<Action> buttonActions, 
        string captionText, 
        string mainText, 
        string footerText)
    {
        int i = 0;
        foreach (var buttonText in ButtonTexts)
            buttonText.text = buttonNames[i++];

        CaptionText.text = captionText;
        MainText.text = mainText;
        FooterText.text = footerText;
        _actions = buttonActions;
    }

    public void OnButton1Tap()
    {
        if(_actions[0] != null)
            _actions[0]();
        _gui.PopScreen("Screen.Dialog.YesNo");
    }

    public void OnButton2Tap()
    {
        if (_actions[1] != null)
            _actions[1]();
        _gui.PopScreen("Screen.Dialog.YesNo");
    }
}
