using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public GameObject aboutPopup;
    public GameObject mainMenu;

    public void LoadScene(string loadScene){
        SceneManager.LoadScene(loadScene);
    }

    public void AboutPopup(){
        aboutPopup.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void BackToMainMenu(){
        aboutPopup.SetActive(false);
        mainMenu.SetActive(true);
    }
}
