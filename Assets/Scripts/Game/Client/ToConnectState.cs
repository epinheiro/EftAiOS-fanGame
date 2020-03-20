using UnityEngine;

public class ToConnectState : IStateController
{
    ClientController clientController; 
    string _customIp = "192.168.0.";

    enum Connection {Disconnected, Connecting, Connected}
    Connection state = Connection.Disconnected;
    const float connTimeOut = 5f;
    float currentTimeOut = 0f;

    public ToConnectState(ClientController clientController){
        this.clientController = clientController;

        clientController.ClientCommunication = clientController.gameObject.AddComponent(typeof(ClientCommunication)) as ClientCommunication;
    }

    public void ExecuteLogic(){}
    public void ShowGUI(){
        _customIp = GUILayout.TextField(_customIp, GUILayout.Width(100));

        if(state == Connection.Disconnected){
            if(ClientCommunication.IsIPValid(_customIp)){
                if(GUILayout.Button("JOIN GAME")){
                    clientController.ClientCommunication.IP = _customIp;
                    if(clientController.ClientCommunication.ConnectToServer(_customIp)){
                        ProcessConnectionState();
                    }
                }
            }
        }else{
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
                SetClientIdentity();
                clientController.CurrentState = ClientController.ClientState.WaitingGame;
                break;
        }
    }

    void ControlTimeOut(){
        if(clientController.ClientCommunication.IsConnected){
            state = Connection.Connected;
        }else{
            currentTimeOut += Time.deltaTime;

            if(currentTimeOut > connTimeOut){
                state = Connection.Disconnected;
                clientController.ClientCommunication.Disconnect();
                Debug.Log(string.Format("CLIENT - connection timeout - check IP {0} and try again", _customIp));
            }
        }
    }

    public void SetClientIdentity(){
        clientController.ClientId = Mathf.Abs(clientController.gameObject.GetInstanceID() + System.DateTime.Now.Second);
    }
}
