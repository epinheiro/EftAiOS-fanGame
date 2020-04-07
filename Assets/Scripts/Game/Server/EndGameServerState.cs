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
            serverController.DelayedCall(ResetGame, 10f); // NODE SYNCHRONIZATION DELAY - future match feedback
        }
    }

    void ResetGame(){
        TimeLogger.Log("SERVER - reset");
        serverController.Reset();
    }

    protected override void GUISetter(){
    }
}
