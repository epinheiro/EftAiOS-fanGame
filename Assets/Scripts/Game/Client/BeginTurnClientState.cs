public class BeginTurnClientState : IStateController
{
    ClientController clientController;
    UIController uiController;

    public BeginTurnClientState(ClientController clientController){
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
        // INFO - Two options of feedbacks
        ///// Option 1 - delete all input traces
        // clientController.BoardManagerRef.CleanGlowTiles(); 
        ///// Option 2 - maintain current position
        clientController.BoardManagerRef.CleanSoundGlowTiles();
        clientController.BoardManagerRef.CleanMovementTile(currentTileCode); // INFO - related to WaitingPlayersClientState Option2 feedback
        clientController.BoardManagerRef.GlowPossibleMovements(currentTileCode, movement, clientController.CurrentPlayerState);
        StateEnd();
    }
    protected override void GUISetter(){
        uiController.SetAllInactiveLayout();
    }

    protected override void StateEnd(){
        ResetStateController();
        clientController.CurrentState = ClientController.ClientState.Playing;
    }
}