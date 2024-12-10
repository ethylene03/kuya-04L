using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    // [SerializeField] Image PauseButton;
    // [SerializeField] Image ContinueButton;

    void Start(){
        AudioListener.pause = false;
    }

    void Update(){
        if(globalVariables.startGame){
            AudioListener.pause = false;
        }
        else{
            AudioListener.pause = true;
        }
    }
}
