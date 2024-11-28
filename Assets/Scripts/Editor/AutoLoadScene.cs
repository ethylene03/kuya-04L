using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class AutoLoadScene
{
    static AutoLoadScene()
    {
        // Specify the scene to load (use the full path relative to the Assets folder)
        string scenePath = "Assets/Scenes/home.unity";

        // Check if the scene is already loaded to prevent unnecessary reloading
        if (!EditorSceneManager.GetActiveScene().path.Equals(scenePath))
        {
            // Load the specified scene
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(scenePath);
                Debug.Log($"Auto-loaded scene: {scenePath}");
            }
        }
    }
}
