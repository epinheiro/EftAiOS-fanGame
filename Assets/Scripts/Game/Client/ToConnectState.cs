using UnityEngine;
using System;

public class ToConnectState : IStateController
{
    ClientController clientController; 
    string _customIp = "192.168.0.";

    enum Connection {Disconnected, Connecting, Connected}
    Connection state = Connection.Disconnected;
    const float connTimeOut = 5f;
    Nullable<DateTime> timeoutEnd;

    public ToConnectState(ClientController clientController){
        this.clientController = clientController;
        SetClientIdentity();
        clientController.ClientCommunication = clientController.gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
    }

    public void ExecuteLogic(){}
    public void ShowGUI(){
        _customIp = GUILayout.TextField(_customIp, GUILayout.Width(100));

        if(state == Connection.Disconnected){
            if(NodeCommunication.IsIPValid(_customIp)){
                if(GUILayout.Button("JOIN GAME")){
                    TryToConnect();
                }
            }else{
                if(GUILayout.Button("JOIN GAME (standard local ip)")){
                    TryToConnect();
                }
            }
        }else{
            ProcessConnectionState();
        }
    }

    void TryToConnect(string ip = ""){
        clientController.ClientCommunication.IP = ip;
        if(clientController.ClientCommunication.ConnectToServer(ip)){
            ProcessConnectionState();
        }
    }

    void ProcessConnectionState(){
        switch(state){
            case Connection.Disconnected:
                state = Connection.Connecting;
                break;

            case Connection.Connecting:
                Debug.Log(string.Format("CLIENT - connecting to {0}", _customIp));
                ControlTimeOut();
                break;

            case Connection.Connected:
                Debug.Log(string.Format("CLIENT - connected to {0}", _customIp));
                clientController.CurrentState = ClientController.ClientState.WaitingGame;
                break;
        }
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
