using UnityEngine;
using UnityEngine.UI;


namespace GameGUI.Dev
{
    public class LogFilterButton : MonoBehaviour
    {
        public string BaseText;
        public LogType LogType;
        public Text Text;

        public void Awake()
        {
            SetCounterText(0);
        }

        public void SetCounterText(int cntNumber)
        {
            if (0 == cntNumber)
                Text.text = BaseText.Split('(')[0];
            else
                Text.text = string.Format(BaseText, cntNumber);
        }
    }
}