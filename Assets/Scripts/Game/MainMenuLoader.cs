using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public GameObject aboutPopup;
    public GameObject mainMenu;

    public GameObject howToPlayPopup;
    HowToPlayPopupWrapper howtoPlayScrollBar;

    // <Screen, (Activate, Deactivate)>
    Dictionary<MainMenuSubscreenEnum, (Action, Action)> screenCallbacksController;

    MainMenuSubscreenEnum currentScreen;

    void Start(){
        howtoPlayScrollBar = howToPlayPopup.GetComponent<HowToPlayPopupWrapper>();
        screenCallbacksController = new Dictionary<MainMenuSubscreenEnum, (Action, Action)>()
        {
            {MainMenuSubscreenEnum.MainMenu, (() => MainMenuSetActivate(true), () => MainMenuSetActivate(false))},
            {MainMenuSubscreenEnum.HowToPlay, (() => HowToPlaySetActivate(true), () => HowToPlaySetActivate(false))},
            {MainMenuSubscreenEnum.About, (() => AboutSetActivate(true), () => AboutSetActivate(false))}
        };

        currentScreen = MainMenuSubscreenEnum.MainMenu;
    }

    void MainMenuSetActivate(bool isActive)
    {
        mainMenu.SetActive(isActive);
    }
    void HowToPlaySetActivate(bool isActive)
    {
        if(isActive) howtoPlayScrollBar.ActivateIntroPage();
        else howToPlayPopup.SetActive(isActive);
    }
    void AboutSetActivate(bool isActive)
    {
        aboutPopup.SetActive(isActive);
    }

    void OpenMainMenuScreen(MainMenuSubscreenEnum screen)
    {
        screenCallbacksController[currentScreen].Item2.Invoke();
        screenCallbacksController[screen].Item1.Invoke();
        currentScreen = screen;
    }

    void LoadScene(string loadScene){
        screenCallbacksController[currentScreen].Item2.Invoke();

        loadingScreen.SetActive(true);

        SceneManager.LoadScene(loadScene);
    }

    /// Public API to bind ///
    public void LoadOnlyClientScene()
    {
        LoadScene("OnlyClient");
    }
    public void LoadOnlyServerScene()
    {
        LoadScene("OnlyServer");
    }

    public void AboutPopup(){
        OpenMainMenuScreen(MainMenuSubscreenEnum.About);
    }

    public void BackToMainMenu(){
        OpenMainMenuScreen(MainMenuSubscreenEnum.MainMenu);
    }

    public void HowToPlayPopup(){
        OpenMainMenuScreen(MainMenuSubscreenEnum.HowToPlay);
    }

    public void ExitGame(){
        Application.Quit();
    }
}
