using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientButtonWrapper : MonoBehaviour
{
    ClientController controller;
    UIController uiController;
    public GameObject uiCanvas;

    void Awake(){
        controller = this.GetComponent<ClientController>();
    }

    void Start(){
        uiController = controller.UIController;
    }

    public void CloseRolePopup(){
        uiController.HideRolePopup();
        uiController.SetActiveClientFooterGroup(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
