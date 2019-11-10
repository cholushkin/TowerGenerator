using Lean.Touch;
using UnityEngine;
using UnityEngine.Assertions;

public class DevToolsActivator : MonoBehaviour
{
    public GameObject DevToolsObject;

    void Awake()
    {
        LeanTouch.OnFingerSet += FingerSet;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            EnableDevTools();
    }

    private void FingerSet(LeanFinger finger)
    {
        if (finger.Index == 4 && finger.Up) // secret gesture
            EnableDevTools();
    }

    public void EnableDevTools()
    {
        Assert.IsNotNull(DevToolsObject);
        Assert.IsFalse(DevToolsObject.activeSelf);
        DevToolsObject.SetActive(true);
        Destroy(gameObject);
    }
}