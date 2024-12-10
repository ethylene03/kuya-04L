using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    // [SerializeField] Image PauseButton;
    // [SerializeField] Image ContinueButton;

    // private bool muted = false;

    void Start(){
        AudioListener.pause = false;
    }

    void Update(){
        if(globalVariables.startGame){
            // muted = true;
            AudioListener.pause = false;
        }
        else{
            // muted = false;
            AudioListener.pause = true;
        }
    }
}
