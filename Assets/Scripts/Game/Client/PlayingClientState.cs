using UnityEngine;
using System.Collections.Generic;

public class PlayingClientState : IStateController
{
    enum EffectFeedback {Silent, ChooseSector, SectorSound, LuckDangerousSector}
    Dictionary<EffectFeedback, Color> effects;
    EffectFeedback lastEffect = EffectFeedback.Silent;

    ClientController clientController;
    UIController uiController;

    public enum TurnSteps {
        Movement,
        PlayerWillAttack,
        Card,
        Noise,
        SendData
    }
    TurnSteps currentTurnStep = TurnSteps.Movement;

    public PlayingClientState(ClientController clientController){
        this.clientController = clientController;
        this.uiController = clientController.UIController;

        effects = new Dictionary<EffectFeedback, Color>();
        effects.Add(EffectFeedback.Silent, new Color(0.086f,0.094f,0.101f));
        effects.Add(EffectFeedback.LuckDangerousSector, new Color(0.035f,0.282f,0.113f));
        effects.Add(EffectFeedback.ChooseSector, new Color(0.780f,0.462f,0f));
        effects.Add(EffectFeedback.SectorSound, new Color(0.301f,0.058f,0.031f));

        ActivateEffectFeedback(EffectFeedback.Silent);
    }

    protected override void ExecuteLogic(){
        switch(currentTurnStep){
            case TurnSteps.Movement:
                ClientPing();
                if(clientController.PlayerNullableNextPosition.HasValue){
                    currentTurnStep = TurnSteps.PlayerWillAttack;
                }

                break;

            case TurnSteps.PlayerWillAttack:
                if(clientController.CurrentPlayerState == ClientController.PlayerState.Alien){
                    if(!clientController.PlayerNullableWillAttack.HasValue){
                        this.uiController.SetTwoButtonsVisibility(true);
                        ClientPing();
                    }else{
                        this.uiController.SetTwoButtonsVisibility(false);
                        if(clientController.PlayerNullableWillAttack.Value){
                            clientController.PlayerNullableNexSound = clientController.PlayerNullableNextPosition;
                            ActivateEffectFeedback(EffectFeedback.SectorSound);
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
                            TimeLogger.Log("CLIENT {0} can choose a sector to make a noise", clientController.ClientId);
                            uiController.SetInfoText("Choose an alarm sector");
                            ActivateEffectFeedback(EffectFeedback.ChooseSector);
                            clientController.BoardManagerRef.GlowPossibleNoises();
                            currentTurnStep = TurnSteps.Noise;
                            break;

                        case EventDeck.CardTypes.CurrentSectorSound:
                            TimeLogger.Log("CLIENT {0} make a noise in his sector", clientController.ClientId);
                            ActivateEffectFeedback(EffectFeedback.SectorSound);
                            clientController.PlayerNextSound = clientController.PlayerNextPosition;
                            currentTurnStep = TurnSteps.Noise;
                            break;

                        case EventDeck.CardTypes.NoSound:
                            TimeLogger.Log("CLIENT {0} is silent", clientController.ClientId);
                            ActivateEffectFeedback(EffectFeedback.LuckDangerousSector);
                            currentTurnStep = TurnSteps.SendData;
                            break;
                    }
                    
                }else{
                    ActivateEffectFeedback(EffectFeedback.Silent);
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.Noise:
                ClientPing();
                if(clientController.PlayerNullableNexSound.HasValue){
                    currentTurnStep = TurnSteps.SendData;
                }
                break;

            case TurnSteps.SendData:
                uiController.SetInfoText();
                clientController.ClientCommunication.SchedulePutPlayRequest(
                    (Vector2Int) clientController.PlayerNullableNextPosition,
                    clientController.PlayerNullableNexSound.HasValue ? (Vector2Int) clientController.PlayerNullableNexSound : new Vector2Int(-1,-1), // TODO - check if is there better solutions than V(-1,-1)
                    clientController.PlayerNullableWillAttack.HasValue ? (bool) clientController.PlayerNullableWillAttack : false
                );
                StateEnd();
                break;
        }
    }
    protected override void GUISetter(){
        this.uiController.SetTwoButtonsLayout("Attack", AttackCallback, "Don't Attack", DontAttackCallback);
        this.uiController.SetTwoButtonsVisibility(false);
        this.uiController.SetActiveClientFooterGroup(true);
        ActivateEffectFeedback(EffectFeedback.Silent);
    }

    void ActivateEffectFeedback(EffectFeedback effect){
        Color currentColor;
        effects.TryGetValue(lastEffect, out currentColor);

        Color newColor;
        effects.TryGetValue(effect, out newColor);

        clientController.StartCoroutine(CameraController.BackgroundChangeCoroutine(currentColor, newColor));

        lastEffect = effect;
    }

    void AttackCallback(){
        clientController.PlayerNullableWillAttack = true;
    }

    void DontAttackCallback(){
        clientController.PlayerNullableWillAttack = false;
    }

    void ClientPing(){
        clientController.ChangeClientStateBaseOnServer(ServerController.ServerState.EndGame, ClientController.ClientState.Updating, delegate(){ ForceEndGame(); });
    }

    void ForceEndGame(){
        uiController.HideRolePopup();
    }

    protected override void StateEnd(){
        ResetStateController();

        this.uiController.SetActiveClientFooterGroup(false);

        clientController.CurrentState = ClientController.ClientState.WaitingPlayers;
        currentTurnStep = TurnSteps.Movement;
        clientController.PlayerNullableNextPosition = null;
        clientController.PlayerNullableNexSound = null;
        clientController.PlayerNullableWillAttack = null;
    }
}
