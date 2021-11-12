using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientButtonWrapper : MonoBehaviour
{
    ClientController controller;
    UIController uiController;
    public GameObject uiCanvas;

    public GameObject howToPlayPopup;
    HowToPlayPopupWrapper howtoPlayScrollBar;

    void Awake(){
        controller = this.GetComponent<ClientController>();
        howtoPlayScrollBar = howToPlayPopup.GetComponent<HowToPlayPopupWrapper>();
    }

    void Start(){
        uiController = uiCanvas.GetComponent<UIController>();
    }

    public void CloseRolePopup(){
        uiController.HideRolePopup();
        uiController.SetActiveClientFooterGroup(true);
    }

    public void HelpButton(){
        howtoPlayScrollBar.SetActive(true);

        switch(controller.CurrentPlayerState){
            case ClientController.PlayerState.Alien:
                howtoPlayScrollBar.GoToPopup(HowToPopupEnum.Alien);
                break;

            case ClientController.PlayerState.Human:
                howtoPlayScrollBar.GoToPopup(HowToPopupEnum.Human);
                break;

            default:
                howtoPlayScrollBar.GoToPopup(HowToPopupEnum.Intro);
                break;
        }
    }
}
