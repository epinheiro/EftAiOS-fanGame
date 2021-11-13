using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HowToPlayPopupWrapper : MonoBehaviour
{
    public GameObject introPopup;
    public GameObject setupPopup;
    public GameObject playingPopup;
    public GameObject humanRolePopup;
    public GameObject alienRolePopup;

    public GameObject backButton;

    HowToPopupEnum currentPopup;
    Dictionary<HowToPopupEnum, (GameObject, Scrollbar, Action, Action)> popups;

    void Awake()
    {
        popups = new Dictionary<HowToPopupEnum, (GameObject, Scrollbar, Action, Action)>()
        {
            {HowToPopupEnum.Intro, (introPopup, introPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>(), null, null)},
            {HowToPopupEnum.Setup, (setupPopup, setupPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>(), null, null)},
            {HowToPopupEnum.Playing, (playingPopup, playingPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>(), null, null)},
            {HowToPopupEnum.Human, (humanRolePopup, humanRolePopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>(), null, null)},
            {HowToPopupEnum.Alien, (alienRolePopup, alienRolePopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>(), null, null)}
        };

        currentPopup = HowToPopupEnum.Closed;
    }

    public void ActivateIntroPage(){
        SetActive(true);
        GoToPopup(HowToPopupEnum.Intro);
    }

    public void SetActive(bool isActive){
        this.gameObject.SetActive(isActive);
    }

    public void GoToPopup(string nextPopup){
        GoToPopup((HowToPopupEnum)System.Enum.Parse(typeof(HowToPopupEnum), nextPopup));
    }

    public void GoToPopup(HowToPopupEnum nextPopup){
        (GameObject, Scrollbar, Action, Action) popup;
        if(currentPopup != HowToPopupEnum.Closed)
        {
            // Deactivate old
            popup = popups[currentPopup];
            popup.Item1.SetActive(false);
        }

        // Activate next
        popup = popups[nextPopup];
        popup.Item1.SetActive(true);
        popup.Item2.value = 1;

        if(nextPopup == HowToPopupEnum.Intro) backButton.SetActive(false);
        else backButton.SetActive(true);

        currentPopup = nextPopup;
    }

    public void CloseHowToPopup(){
        currentPopup = HowToPopupEnum.Closed;
        SetActive(false);
    }

}
