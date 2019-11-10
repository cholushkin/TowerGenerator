using Alg;
using UnityEngine;

public class OnGuiHelper : Singleton<OnGuiHelper>
{
    public float LeftOffset;
    public float LineGap;
    public float LineHeight;
    public float LineWidth;

    public void Left(int line, string text)
    {
        if(gameObject.activeInHierarchy == false)
            return;
        var rect = GetRect(line);
        GUI.contentColor = line % 2 == 0 ? Color.yellow : Color.green;
        GUI.Label(rect, text);
    }

    private Rect GetRect(int line)
    {
        return new Rect(LeftOffset, line * (LineHeight + LineGap), LineWidth, LineHeight);
    }
}
