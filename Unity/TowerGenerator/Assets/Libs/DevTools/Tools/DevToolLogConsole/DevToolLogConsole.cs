using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Plugins.Alg;
using DevTools;
using GameLib.Log;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace GameGUI.Dev
{
    // todo:
    // + filter warning, normal, error, exception, assert
    // + clear button
    // - log filter button set icon automaticaly
    // - stop listen button / start listen
    // - show trace/different modes on tap msg
    // - scroll down automatically
    // - serialize to gui serializator

    public class DevToolLogConsole : GUIScreenBase, IDevTool
    {
        public Transform Content;
        public GameObject LinePrefab;
        private List<LogLine> _lines;


        #region IDevTool
        public void OnDevToolCreate()
        {
        }

        public void OnDevToolNavigateTo()
        {
            UpdateView();
        }

        public void OnDevToolShow()
        {
        }
        #endregion

        private void UpdateView()
        {
            // update log view
            var logManager = LogManager.Instance;
            Assert.IsNotNull(logManager);

            // check which messages are still not added
            foreach (var msg in logManager.LogMessages.Messages)
            {
                if (msg.HandledBy == null)
                    AddToConsole(msg);  
            }

            // todo: update counters
        }

        private void AddToConsole(LogMessages.MessageEntry msg)
        {
            Assert.IsNotNull(msg);
            msg.HandledBy = this;

            var logLine = Instantiate(LinePrefab, Content.transform).GetComponent<LogLine>();
            logLine.Set(msg);
            //    logLine.SetIcon(MsgTypeToTexture(msg.MsgType));
            //    logLine.SetMsgNumber(cntNumber);
            //    //logLine.FixScrollRect.MainScroll = scroll;
            //    msg.LineObj = logLine.gameObject;


            //    Assert.IsNotNull(ScreenState);

            //    // process counters
            //    var cntNumber = ScreenState.ChannelStates[msg.MsgType].MessageCount++;
            //    ScreenState.TotalMessageCount++;
            //    MsgTypeToLogFilterButton(msg.MsgType).SetCounterText(cntNumber + 1);

            //    ScreenState.Messages.Enqueue(msg);

            //    // process log line
            //    var logLine = Instantiate(LinePrefab, Content.transform).GetComponent<LogLine>();
            //    logLine.Set(msg);
            //    logLine.SetIcon(MsgTypeToTexture(msg.MsgType));
            //    logLine.SetMsgNumber(cntNumber);
            //    //logLine.FixScrollRect.MainScroll = scroll;
            //    msg.LineObj = logLine.gameObject;

            //    //ShowInTabControl(logLine);
        }


        //[Serializable]
        //public class SerializableState
        //{
        //    public List<LogType> FilteredChannels;
        //}

        //public class State : SerializableState
        //{
        //    public class ChannelState
        //    {
        //        public LogType ChannelType;
        //        public int MessageCount;
        //    }
        //    public int TotalMessageCount;
        //    public Dictionary<LogType, ChannelState> ChannelStates = new Dictionary<LogType, ChannelState>();
        //    public Queue<MessageEntry> Messages = new Queue<MessageEntry>(1024);

        //    public void ClearAllMessages()
        //    {
        //        TotalMessageCount = 0;
        //        Messages.Clear();
        //        foreach (var channelStateKVP in ChannelStates)
        //            channelStateKVP.Value.MessageCount = 0;
        //    }
        //}

        //public class MessageEntry
        //{
        //    public string Message;
        //    public string StackTrace;
        //    public GameObject LineObj;
        //    public LogType MsgType;
        //}

        //public GameObject Content;
        //public ScrollRect scroll;
        //public GameObject LinePrefab;
        //public Texture2D[] Icons;
        //public LogFilterButton[] LogFilterButtons;
        //public int BufferLineCount;

        //private State ScreenState;

        //public void Init()
        //{
        //    RegisterLogHandler();
        //    ScreenState = new State();
        //    ScreenState.ChannelStates.Add(LogType.Log, new State.ChannelState { ChannelType = LogType.Log });
        //    ScreenState.ChannelStates.Add(LogType.Assert, new State.ChannelState { ChannelType = LogType.Assert });
        //    ScreenState.ChannelStates.Add(LogType.Error, new State.ChannelState { ChannelType = LogType.Error });
        //    ScreenState.ChannelStates.Add(LogType.Exception, new State.ChannelState { ChannelType = LogType.Exception });
        //    ScreenState.ChannelStates.Add(LogType.Warning, new State.ChannelState { ChannelType = LogType.Warning });
        //    LogFilterButtons = GetComponentsInChildren<LogFilterButton>();
        //}

        //public void Deinit()
        //{
        //    UnregisterLogHandler();
        //    ClearAll();
        //    ScreenState = null;
        //}

        //public void HandlePopScreen()
        //{
        //    //SimpleGui.PopScreen();
        //}

        //private void RegisterLogHandler()
        //{
        //    Application.logMessageReceived += HandleLog;
        //}

        //private void UnregisterLogHandler()
        //{
        //    Application.logMessageReceived -= HandleLog;
        //}

        //private void HandleLog(string logString, string stackTrace, LogType type)
        //{
        //    AddMessage(
        //        new MessageEntry() { Message = logString, StackTrace = stackTrace, MsgType = type }
        //    );
        //}

        //void Update()
        //{
        //    if (ScreenState != null && (Content.transform.childCount > BufferLineCount))
        //    {
        //        var toDelete = Content.transform.childCount - BufferLineCount;
        //        for (int i = 0; i < toDelete; ++i)
        //        {
        //            Destroy(Content.transform.GetChild(i).gameObject);
        //            ScreenState.Messages.Dequeue();
        //        }
        //    }
        //}

        //private void AddMessage(MessageEntry msg)
        //{
        //    Assert.IsNotNull(ScreenState);

        //    // process counters
        //    var cntNumber = ScreenState.ChannelStates[msg.MsgType].MessageCount++;
        //    ScreenState.TotalMessageCount++;
        //    MsgTypeToLogFilterButton(msg.MsgType).SetCounterText(cntNumber + 1);

        //    ScreenState.Messages.Enqueue(msg);

        //    // process log line
        //    var logLine = Instantiate(LinePrefab, Content.transform).GetComponent<LogLine>();
        //    logLine.Set(msg);
        //    logLine.SetIcon(MsgTypeToTexture(msg.MsgType));
        //    logLine.SetMsgNumber(cntNumber);
        //    //logLine.FixScrollRect.MainScroll = scroll;
        //    msg.LineObj = logLine.gameObject;

        //    //ShowInTabControl(logLine);
        //}

        //public void ClearAll()
        //{
        //    ScreenState.ClearAllMessages();
        //    Content.transform.DestroyChildren();
        //    foreach (var logFilterButton in LogFilterButtons)
        //        logFilterButton.SetCounterText(0);
        //}

        //private Texture2D MsgTypeToTexture(LogType msgMsgType)
        //{
        //    if (msgMsgType == LogType.Log)
        //        return Icons[0];
        //    if (msgMsgType == LogType.Warning)
        //        return Icons[1];
        //    if (msgMsgType == LogType.Error)
        //        return Icons[2];
        //    if (msgMsgType == LogType.Exception)
        //        return Icons[3];
        //    if (msgMsgType == LogType.Assert)
        //        return Icons[4];
        //    return Icons[5];
        //}

        //private LogFilterButton MsgTypeToLogFilterButton(LogType msgMsgType)
        //{
        //    return LogFilterButtons.FirstOrDefault(but => but.LogType == msgMsgType);
        //}

        //void ShowInTabControl(LogLine ll)
        //{
        //    float normalizePosition = ll.transform.GetSiblingIndex() / (float)scroll.content.transform.childCount;
        //    scroll.verticalNormalizedPosition = 1 - normalizePosition;
        //}


        //public void OnLogFilterButtonPress(GameObject button)
        //{
        //    var logFilterButton = button.GetComponent<LogFilterButton>();
        //    var toggle = button.GetComponent<Toggle>();

        //    Assert.IsNotNull(logFilterButton);
        //    Assert.IsNotNull(toggle);

        //    foreach (var mEntry in ScreenState.Messages)
        //    {
        //        if (mEntry.MsgType == logFilterButton.LogType)
        //            mEntry.LineObj.SetActive(toggle.isOn);
        //    }

        //    LayoutRebuilder.ForceRebuildLayoutImmediate(Content.GetComponent<RectTransform>());
        //}


    }
}