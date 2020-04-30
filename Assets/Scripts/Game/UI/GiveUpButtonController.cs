using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveUpButtonController : MonoBehaviour
{
    public Camera mainCamera;
    public ClientController controller;
    public UIController uiController;

    Color initialColor;
    Color finalColor = Color.white;
    
    public float timeToHold = 3f;
    public float countingTime = 0;

    bool currentlyCouting = false;
    bool endReached = false;


    void Start()
    {
        if(mainCamera == null) mainCamera = Camera.main;
        if(controller == null) controller = GameObject.Find("Client").GetComponent<ClientController>();
        if(uiController == null) uiController = GameObject.Find("UICanvas").GetComponent<UIController>();
    }


    void Update()
    {
        if(!endReached && currentlyCouting){
            countingTime += Time.deltaTime;
            if(IsEndConditionReached()){
                EndScript();
            }else{
                Color nextColor = new Color(
                    Mathf.Lerp( initialColor.r, finalColor.r, countingTime/timeToHold),
                    Mathf.Lerp( initialColor.g, finalColor.g, countingTime/timeToHold),
                    Mathf.Lerp( initialColor.b, finalColor.b, countingTime/timeToHold)
                );
                
                CameraController.ChangeCameraBackgroundColor(nextColor);
            }
        }
    }

    bool IsEndConditionReached(){
        return countingTime > timeToHold;
    }

    void EndScript(){
        endReached = true;
        TimeLogger.Log("Player use the GIVE UP button");

        Destroy(this.GetComponent<UnityEngine.UI.Button>());

        controller.BoardManagerRef.CleanMovementGlowTiles();
        controller.BoardManagerRef.CleanSoundGlowTiles();
        controller.Audio.PlayerDiedEffect();

        CoroutineHelper.DelayedCall(controller, controller.ReloadClient, 3.5f);

        uiController.SetActiveClientFooterGroup(false);
    }

    public void OnPointerDown(){
        if(!endReached){
            initialColor = CameraController.GetBackgroundColor();
            currentlyCouting = true;
        }
    }

    public void OnPointerUp(){
        if(!endReached){
            CameraController.ChangeCameraBackgroundColor(initialColor);
            currentlyCouting = false;
            countingTime = 0;
        }
    }
}
