using UnityEngine;
using System;

public class ToConnectClientState : IStateController
{
    ClientController clientController; 
    UIController uiController;
    string _customIp = "192.168.0.";

    string _insertedString = "";

    enum Connection {Disconnected, Connecting, Connected}
    Connection state = Connection.Disconnected;
    const float connTimeOut = 5f;
    Nullable<DateTime> timeoutEnd;

    public ToConnectClientState(ClientController clientController, string customIP = ""){
        if(!string.IsNullOrEmpty(customIP)) _insertedString = customIP;

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
                uiController.SetInsertTextButtonAttributes("ENTER SHIP (local)");
            }else{
                if(NodeCommunication.IsIPValid(_insertedString)){
                    uiController.SetInsertTextButtonVisibility(true);
                    uiController.SetInsertTextButtonAttributes("ENTER SHIP");
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
                if(!timeoutEnd.HasValue){
                    TimeLogger.Log("CLIENT - connecting to {0}", logMessage);
                }
                ControlTimeOut();
                break;

            case Connection.Connected:
                TimeLogger.Log("CLIENT - connected to {0}", logMessage);
                if(!string.IsNullOrEmpty(_insertedString)) clientController.LastSucessfulIp = _insertedString;
                StateEnd();
                break;
        }
    }

    protected override void StateEnd(){
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
                    TimeLogger.Log("CLIENT - connection timeout - check IP {0} and try again", _insertedString);
                    ResetControlTimeVariables();
                }
            }
        }else{
            state = Connection.Connected;
            ResetControlTimeVariables();
        }
    }

    void ResetControlTimeVariables(){
        timeoutEnd = null;
    }

    public void SetClientIdentity(){
        int seed1 = Mathf.Abs(Convert.ToInt32(System.DateTime.Now.Second * UnityEngine.Random.Range(0f, 1f)));
        int seed2 = Mathf.Abs(Convert.ToInt32(clientController.gameObject.GetInstanceID() * UnityEngine.Random.Range(0f, 1f)));

        clientController.ClientId =  Mathf.Abs(Convert.ToInt32((seed1 + seed2) * UnityEngine.Random.Range(0f, 1f)));
    }
}
