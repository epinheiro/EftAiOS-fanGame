using UnityEngine;

public class PlayingState : IStateController
{
    ClientController clientController;

    public enum TurnSteps {
        Movement,
        PlayerWillAttack,
        Card,
        Noise,
        SendData
    }
    TurnSteps currentTurnStep = TurnSteps.Movement;

    public PlayingState(ClientController clientController){
        this.clientController = clientController;
    }

    public void ExecuteLogic(){
        switch(currentTurnStep){
            case TurnSteps.Movement:
                if(clientController.PlayerNullableNextPosition.HasValue){
                    currentTurnStep = TurnSteps.PlayerWillAttack;
                }

                break;

            // PlayerWillAttack

            // Card

            case TurnSteps.Noise:
                if(clientController.PlayerNullableNexSound.HasValue){
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.SendData:
                clientController.ClientCommunication.SchedulePutPlayRequest(
                    clientController.ClientId, 
                    (Vector2Int) clientController.PlayerNullableNextPosition,
                    clientController.PlayerNullableNexSound.HasValue ? (Vector2Int) clientController.PlayerNullableNexSound : new Vector2Int(-1,-1), // TODO - check if is there better solutions than V(-1,-1)
                    clientController.PlayerNullableWillAttack.HasValue ? (bool) clientController.PlayerNullableWillAttack : false
                );
                clientController.CurrentState = ClientController.ClientState.WaitingPlayers;
                currentTurnStep = TurnSteps.Movement;
                clientController.PlayerNullableNextPosition = null;
                clientController.PlayerNullableNexSound = null;
                clientController.PlayerNullableWillAttack = null;
                break;
        }
    }
    public void ShowGUI(){
        switch(currentTurnStep){
            // Movement

            case TurnSteps.PlayerWillAttack:
                if(clientController.CurrentPlayerState == ClientController.PlayerState.Alien){
                    if(!clientController.PlayerNullableWillAttack.HasValue){
                        GUILayout.BeginArea(new Rect(0, 0, 175, 175));
                        if(GUILayout.Button("Attack")){
                            clientController.PlayerNullableWillAttack = true;
                        }
                        if(GUILayout.Button("Quiet")){
                            clientController.PlayerNullableWillAttack = false;
                        }
                        GUILayout.EndArea();
                    }else{
                        if(clientController.PlayerNullableWillAttack.Value){
                            clientController.PlayerNullableNexSound = clientController.PlayerNullableNextPosition;
                            currentTurnStep = TurnSteps.SendData;
                        }else{
                            currentTurnStep = TurnSteps.Card;
                        }
                    }
                }else{
                    currentTurnStep = TurnSteps.Card;
                }
                break;

            case TurnSteps.Card:
                string tileCode = BoardManager.TranslateTilePositionToCode(clientController.PlayerNullableNextPosition.Value);
                BoardManager.PossibleTypes tileType = clientController.BoardManagerRef.GetTileType(tileCode);

                if(tileType == BoardManager.PossibleTypes.EventTile){
                    EventDeck.CardTypes cardType = clientController.Deck.DrawCard();

                    switch(cardType){
                        case EventDeck.CardTypes.AnySectorSound:
                            Debug.Log(string.Format("CLIENT {0} can choose a sector to make a noise", clientController.ClientId));
                            clientController.BoardManagerRef.GlowPossibleNoises();
                            currentTurnStep = TurnSteps.Noise;
                            break;

                        case EventDeck.CardTypes.CurrentSectorSound:
                            Debug.Log(string.Format("CLIENT {0} make a noise in his sector", clientController.ClientId));
                            clientController.PlayerNextSound = clientController.PlayerNextPosition;
                            currentTurnStep = TurnSteps.Noise;
                            break;

                        case EventDeck.CardTypes.NoSound:
                            Debug.Log(string.Format("CLIENT {0} is silent", clientController.ClientId));
                            currentTurnStep = TurnSteps.SendData;
                            break;
                    }
                    
                }else{
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            // Noise

            // SendData
        }
    }
}
