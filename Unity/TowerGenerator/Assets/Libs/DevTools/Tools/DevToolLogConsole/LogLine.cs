using UnityEngine;
using UnityEngine.UI;


namespace GameGUI.Dev
{
    public class LogLine : MonoBehaviour
    {
        public Text Text;
        public Text NumberText;
        public Image Icon;
        //public FixScrollRect FixScrollRect;
        private LogMessages.MessageEntry _msg;


        public void SetText(string text)
        {
            Text.text = text;
        }

        public void SetIcon(Texture2D icon)
        {
            Icon.sprite = Sprite.Create(icon, new Rect(0, 0, icon.width, icon.height), Vector2.one * 0.5f);
        }

        public void SetMsgNumber(int num)
        {
            NumberText.text = num.ToString("D4");
        }

        public void Set(LogMessages.MessageEntry msg)
        {
            _msg = msg;
            SetText(_msg.Message);
        }

        public void OnClick()
        {
            SetText(_msg.StackTrace);
        }
    }
}
