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

    public enum HowToPopup {Intro, Setup, Playing, Human, Alien}

    // Start is called before the first frame update
    void Awake()
    {
        popups = new GameObject[]{introPopup, setupPopup, playingPopup, humanRolePopup, alienRolePopup};
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetActive(bool isActive){
        this.gameObject.SetActive(isActive);
        GoToPopup(HowToPlayPopupWrapper.HowToPopup.Intro);
    }

    public void GoToPopup(string nextPopup){
        GoToPopup((HowToPopup)System.Enum.Parse(typeof(HowToPopup), nextPopup));
    }

    public void GoToPopup(HowToPopup nextPopup){
        foreach(HowToPopup popup in System.Enum.GetValues(typeof(HowToPopup))){
            if(nextPopup == popup){
                GameObject goPopup = popups[(int)popup];
                goPopup.SetActive(true);
                goPopup.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 1;

                if(nextPopup == HowToPopup.Intro) backButton.SetActive(false);
                else backButton.SetActive(true);
            }else{
                popups[(int)popup].SetActive(false);
            }
        }
    }


}
