using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public void LoadScene(string loadScene){
        SceneManager.LoadScene(loadScene);
    }
}
