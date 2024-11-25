using UnityEngine;
using UnityEngine.SceneManagement;

public class playBtn : MonoBehaviour
{
    public string targetScene;

    private void OnMouseDown()
    {
        if (!string.IsNullOrEmpty(targetScene))
        {
            SceneManager.LoadScene(targetScene);
        }
        else
        {
            Debug.LogWarning("Target scene name is not set.");
        }
    }
}
