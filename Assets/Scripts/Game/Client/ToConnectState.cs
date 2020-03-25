using UnityEngine;
using System;

public class ToConnectState : IStateController
{
    ClientController clientController; 
    UIController uiController;
    string _customIp = "192.168.0.";

    string _insertedString = "";

    enum Connection {Disconnected, Connecting, Connected}
    Connection state = Connection.Disconnected;
    const float connTimeOut = 5f;
    Nullable<DateTime> timeoutEnd;

    public ToConnectState(ClientController clientController){
        this.clientController = clientController;
        SetClientIdentity();
        clientController.ClientCommunication = clientController.gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        _insertedString = uiController.GetInsertedText();
         if(state == Connection.Disconnected){
            if(string.IsNullOrEmpty(_insertedString)){
                uiController.SetInsertTextButtonVisibility(true);
                uiController.SetInsertTextButtonAttributes("JOIN GAME (std IP)");
            }else{
                if(NodeCommunication.IsIPValid(_insertedString)){
                    uiController.SetInsertTextButtonVisibility(true);
                    uiController.SetInsertTextButtonAttributes("JOIN GAME");
                }else{
                    uiController.SetInsertTextButtonVisibility(false);
                }
            }
        }else{
            uiController.SetInsertTextButtonVisibility(false);
            ProcessConnectionState();
        }
    }

    protected override void GUISetter(){
        this.uiController.SetInsertTextLayout(_customIp, "", "", delegate(){ Callback(); });
        this.uiController.SetInsertTextButtonVisibility(false);
    }

    public void Callback(){
        TryToConnect(_insertedString);
    }

    void TryToConnect(string ip = ""){
        clientController.ClientCommunication.IP = ip;
        if(clientController.ClientCommunication.ConnectToServer(ip)){
            ProcessConnectionState();
        }
    }

    void ProcessConnectionState(){
        string logMessage = _insertedString;
        if(string.IsNullOrEmpty(logMessage)) logMessage = "localhost";

        switch(state){
            case Connection.Disconnected:
                state = Connection.Connecting;
                break;

            case Connection.Connecting:
                Debug.Log(string.Format("CLIENT - connecting to {0}", logMessage));
                ControlTimeOut();
                break;

            case Connection.Connected:
                Debug.Log(string.Format("CLIENT - connected to {0}", logMessage));
                StateEnd();
                break;
        }
    }

    void StateEnd(){
        ResetStateController();
        clientController.CurrentState = ClientController.ClientState.WaitingGame;
    }

    void ControlTimeOut(){
        if(!clientController.ClientCommunication.IsConnected){
            if(!timeoutEnd.HasValue){
                timeoutEnd = DateTime.Now.AddSeconds(connTimeOut);
            }else{
                if(DateTime.Now > timeoutEnd){
                    state = Connection.Disconnected;
                    clientController.ClientCommunication.Disconnect();
                    Debug.Log(string.Format("CLIENT - connection timeout - check IP {0} and try again", _insertedString));
                }
            }
        }else{
            state = Connection.Connected;
            timeoutEnd = null;
        }
    }

    public void SetClientIdentity(){
        clientController.ClientId = Mathf.Abs(clientController.gameObject.GetInstanceID() + System.DateTime.Now.Second);
    }
}
