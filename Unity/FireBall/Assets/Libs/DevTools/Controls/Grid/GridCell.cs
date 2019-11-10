using GameLib;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

interface IGridCell
{
    Vector2i GetPosition();
    void SetPosition(Vector2i coord);
}

public class GridCell : MonoBehaviour, IGridCell
{
    private Vector2i _coord;
    public Text TextNameOfControl;
    private Button _button;
    private Image _img;
    private const string EmptySlotCaption = "None";


    public void Select(bool flag)
    {
        _img.color = flag ? Color.yellow : Color.white;
    }

    public void SetControl(string name)
    {
        TextNameOfControl.color = Color.gray;
        TextNameOfControl.text = name;
    }

    public void RemoveControl()
    {
        TextNameOfControl.color = Color.gray;
        TextNameOfControl.text = EmptySlotCaption;
    }

    void Awake()
    {
        _button = GetComponent<Button>();
        _img = GetComponent<Image>();
        Assert.IsNotNull(TextNameOfControl);
        Assert.IsNotNull(_button);
    }

    public void OnClick()
    {
        //GetComponentInParent<LayoutPanel>().OnClick(this);
    }

    public Vector2i GetPosition()
    {
        return _coord;

    }

    public void SetPosition(Vector2i coord)
    {
        _coord = coord;
    }
}
