using Lean.Touch;
using UnityEngine;
using UnityEngine.Assertions;

public class DevToolsDeactivator : MonoBehaviour
{
    public GameObject DevToolsObject;
    [Tooltip("is DevTools initially enabled (in case if it's not disabled from build")]
    public bool IsInitialyEnabled;

    void Awake()
    {
        Assert.IsNotNull(DevToolsObject);
        LeanTouch.OnFingerSet += FingerSet;
        if (!IsInitialyEnabled)
            DisableDevTools();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
            DisableDevTools();
    }

    private void FingerSet(LeanFinger finger)
    {
        if (finger.Index == 4 && finger.Up) // secret gesture
            DisableDevTools();
    }

    private void DisableDevTools()
    {
        DevToolsObject.SetActive(false);

        // create activator in case if we want to enable DevTools back
        var activatorObj = new GameObject("DevToolsActivator");
        var activator = activatorObj.AddComponent<DevToolsActivator>();
        activatorObj.transform.SetParent(transform.parent);
        activator.DevToolsObject = DevToolsObject;
        Debug.LogFormat("Press <D> to activate DevMenu");
    }
}
