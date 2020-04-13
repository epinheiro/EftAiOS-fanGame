public class EndGameServerState : IStateController
{
    bool isExecuting = false;
    ServerController serverController;

    public EndGameServerState(ServerController serverController){
        this.serverController = serverController;
    }

    protected override void ExecuteLogic(){
        if(!isExecuting){
            TimeLogger.Log("SERVER - end game");
            isExecuting = true;

            ProcessEndGame();

            serverController.DelayedCall(ResetGame, 10f); // NODE SYNCHRONIZATION DELAY - future match feedback
        }
    }

    void ProcessEndGame(){
        int countAliens = 0;
        int countHumans = 0;

        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);

            switch(data.role){
                case ClientController.PlayerState.Alien:
                    countAliens++;
                    break;
                case ClientController.PlayerState.Human:
                    countHumans++;
                    break;

                case ClientController.PlayerState.Unassigned:
                    throw new System.Exception("Shouln't be Unassigned player role in ProcessEndGame");
            }
        }

        foreach(int key in serverController.PlayerTurnDict.Keys){
            PlayerTurnData data;
            serverController.PlayerTurnDict.TryGetValue(key, out data);

            if(countAliens == 0){
                data.role = ClientController.PlayerState.Escaped;
            }else{
                switch(data.role){
                    case ClientController.PlayerState.Alien:
                        data.role = ClientController.PlayerState.AlienOverrun;
                        break;
                    case ClientController.PlayerState.Human:
                        data.role = ClientController.PlayerState.Died;
                        break;
                }
            }
        }
    }

    void ResetGame(){
        TimeLogger.Log("SERVER - reset");
        serverController.Reset();
    }

    protected override void GUISetter(){
        this.serverController.UIController.SetInfoText("Shutting down ship AI");
    }
}
