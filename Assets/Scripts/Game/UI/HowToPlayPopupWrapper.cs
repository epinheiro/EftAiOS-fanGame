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

    GameObject[] popups;

    // Start is called before the first frame update
    void Awake()
    {
        popups = new GameObject[]{introPopup, setupPopup, playingPopup, humanRolePopup, alienRolePopup};
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
        foreach(HowToPopupEnum popup in System.Enum.GetValues(typeof(HowToPopupEnum))){
            if(nextPopup == popup){
                GameObject goPopup = popups[(int)popup];
                goPopup.SetActive(true);
                goPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1;

                if(nextPopup == HowToPopupEnum.Intro) backButton.SetActive(false);
                else backButton.SetActive(true);
            }else{
                popups[(int)popup].SetActive(false);
            }
        }
    }

    public void CloseHowToPopup(){
        SetActive(false);
    }

}
