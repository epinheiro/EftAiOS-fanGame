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

    Dictionary<HowToPopupEnum, (GameObject, Scrollbar)> popups;

    void Awake()
    {
        popups = new Dictionary<HowToPopupEnum, (GameObject, Scrollbar)>()
        {
            {HowToPopupEnum.Intro, (introPopup, introPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>())},
            {HowToPopupEnum.Setup, (setupPopup, setupPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>())},
            {HowToPopupEnum.Playing, (playingPopup, playingPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>())},
            {HowToPopupEnum.Human, (humanRolePopup, humanRolePopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>())},
            {HowToPopupEnum.Alien, (alienRolePopup, alienRolePopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>())}
        };
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
        foreach(KeyValuePair<HowToPopupEnum, (GameObject, Scrollbar)> popup in popups){
            if(nextPopup == popup.Key){
                popup.Value.Item1.SetActive(true);
                popup.Value.Item2.value = 1;

                if(nextPopup == HowToPopupEnum.Intro) backButton.SetActive(false);
                else backButton.SetActive(true);
            }else{
                popup.Value.Item1.SetActive(false);
            }
        }
    }

    public void CloseHowToPopup(){
        SetActive(false);
    }

}
