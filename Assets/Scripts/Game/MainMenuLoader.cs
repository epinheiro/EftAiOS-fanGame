using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public GameObject aboutPopup;
    public GameObject mainMenu;

    public GameObject howToPlayPopup;
    HowToPlayPopupWrapper howtoPlayScrollBar;

    void Start(){
        howtoPlayScrollBar = howToPlayPopup.GetComponent<HowToPlayPopupWrapper>();
    }

    public void LoadScene(string loadScene){
        SceneManager.LoadScene(loadScene);
    }

    public void AboutPopup(){
        aboutPopup.SetActive(true);
        howToPlayPopup.SetActive(false);
        mainMenu.SetActive(false);
    }

    public void BackToMainMenu(){
        aboutPopup.SetActive(false);
        howToPlayPopup.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void HowToPlayPopup(){
        aboutPopup.SetActive(false);
        howtoPlayScrollBar.SetActive(true);
        mainMenu.SetActive(false);
    }
}
