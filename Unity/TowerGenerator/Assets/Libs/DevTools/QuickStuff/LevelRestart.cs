using Alg;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelRestart : Singleton<LevelRestart>
{
    public string SceneName;
    public string Caption { get; set; }

    void Reset()
    {
        SceneName = "Gameplay";
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(Screen.width - 110, 10, 100, 60), Caption) || Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneName);
        }
    }
}
