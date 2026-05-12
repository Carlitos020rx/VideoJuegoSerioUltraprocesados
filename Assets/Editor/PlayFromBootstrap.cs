// PlayFromBootstrap.cs
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
[InitializeOnLoad]
public class PlayFromBootstrap
{
    static PlayFromBootstrap()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (EditorBuildSettings.scenes.Length > 0)
            {
                EditorSceneManager.LoadScene(
                    EditorBuildSettings.scenes[0].path
                );
            }
        }
    }
}