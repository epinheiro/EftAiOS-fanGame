using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public GameObject aboutPopup;
    public GameObject mainMenu;

    public GameObject howToPlayPopup;
    public Scrollbar howtoPlayScrollBar;

    void Start(){
        howtoPlayScrollBar = howToPlayPopup.transform.Find("Intro").Find("Scrollbar Vertical").GetComponent<Scrollbar>();
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
        howToPlayPopup.SetActive(true);
        mainMenu.SetActive(false);

        howtoPlayScrollBar.value = 1;
    }
}
