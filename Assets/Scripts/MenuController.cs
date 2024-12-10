using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject menu;

    public void ShowMenu() {
        if(menu != null) {
            menu.SetActive(true);
            globalVariables.startGame = false;
        }
    }

    public void HideMenu() {
        if(menu != null) {
            menu.SetActive(false);
            globalVariables.startGame = true;
        }
    }

    public void QuitGame() {
        NetworkManagerController.Instance.RestartNetworkManager();

        // string targetScene = "home";

        // if (!string.IsNullOrEmpty(targetScene))
        // {
        //     SceneManager.LoadScene(targetScene);
        // }
        // else
        // {
        //     Debug.LogWarning("Target scene name is not set.");
        // }
    }
}
