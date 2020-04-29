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
        uiController = controller.UIController;
    }

    public void CloseRolePopup(){
        uiController.HideRolePopup();
        uiController.SetActiveClientFooterGroup(true);
    }

    public void HelpButton(){
        howtoPlayScrollBar.SetActive(true);

        switch(controller.CurrentPlayerState){
            case ClientController.PlayerState.Alien:
                howtoPlayScrollBar.GoToPopup(HowToPlayPopupWrapper.HowToPopup.Alien);
                break;

            case ClientController.PlayerState.Human:
                howtoPlayScrollBar.GoToPopup(HowToPlayPopupWrapper.HowToPopup.Human);
                break;

            default:
                howtoPlayScrollBar.GoToPopup(HowToPlayPopupWrapper.HowToPopup.Intro);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
