using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[FilePath("SomeSubFolder/StateFile.foo", FilePathAttribute.Location.ProjectFolder)]
public class MySingleton : ScriptableSingleton<MySingleton>
{
    [SerializeField]
    float m_Number = 42;

    [SerializeField]
    List<string> m_Strings = new List<string>();

    public void Modify()
    {
        m_Number *= 2;
        m_Strings.Add("Foo" + m_Number);

        Save(true);
        Debug.Log("Saved to: " + GetFilePath());
    }

    public void Log()
    {
        Debug.Log("MySingleton state: " + JsonUtility.ToJson(this, true));
    }
}


static class MySingletonMenuItems
{
    [MenuItem("SingletonTest/Log")]
    static void LogMySingletonState()
    {
        MySingleton.instance.Log();
    }

    [MenuItem("SingletonTest/Modify")]
    static void ModifyMySingletonState()
    {
        MySingleton.instance.Modify();
    }
}