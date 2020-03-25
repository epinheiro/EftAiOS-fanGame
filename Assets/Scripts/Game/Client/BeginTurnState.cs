public class BeginTurnState : IStateController
{
    ClientController clientController;
    UIController uiController;

    public BeginTurnState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;
    }

    protected override void ExecuteLogic(){
        string currentTileCode = BoardManager.TranslateTileNumbersToCode(clientController.playerCurrentPosition.x, clientController.playerCurrentPosition.y);
        int movement = 0;
        switch(clientController.CurrentPlayerState){
            case ClientController.PlayerState.Alien:
                movement = 2;
                break;
            case ClientController.PlayerState.Human:
                movement = 1;
                break;
        }
        clientController.BoardManagerRef.GlowPossibleMovements(currentTileCode, movement, clientController.CurrentPlayerState);
        StateEnd();
    }
    protected override void GUISetter(){
        uiController.SetGenericLayout(UIController.Layout.AllInactive);
    }

    void StateEnd(){
        ResetStateController();
        clientController.CurrentState = ClientController.ClientState.Playing;
    }
}