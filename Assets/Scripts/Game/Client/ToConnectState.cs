﻿using UnityEngine;

public class ToConnectState : IStateController
{
    ClientController clientController; 
    string _customIp = "192.168.0.";

    public ToConnectState(ClientController clientController){
        this.clientController = clientController;

        clientController.ClientCommunication = clientController.gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
    }

    public void ExecuteLogic(){}
    public void ShowGUI(){
        _customIp = GUILayout.TextField(_customIp);

        if (GUILayout.Button("JOIN GAME") && ClientCommunication.IsIPValid(_customIp)){
            SetClientIdentity();
            clientController.ClientCommunication.IP = _customIp;
            if(clientController.ClientCommunication.ConnectToServer(_customIp)){
                clientController.CurrentState = ClientController.ClientState.WaitingGame;
            }
        }
    }

    public void SetClientIdentity(){
        clientController.ClientId = Mathf.Abs(clientController.gameObject.GetInstanceID() + System.DateTime.Now.Second);
    }
}
