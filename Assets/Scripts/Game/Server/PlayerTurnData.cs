public class PlayerTurnData
{
    // UI Color controller
    public enum UIColors {Red, Orange, Yellow, Green, Cyan, Blue, Purple, Pink, White}
    public static int nextUiColor = 0;


    public PutPlayRequest lastPlay;
    public bool playedThisTurn;
    public ClientController.PlayerState role;
    public ClientController.PlayerState playingRole;

    public UnityEngine.Material uiColor;

    public PlayerTurnData(PutPlayRequest lastPutPlay, ClientController.PlayerState role){
        this.lastPlay = lastPutPlay;
        this.playedThisTurn = false;
        this.role = role;
        playingRole = role;

        int colorToGet = nextUiColor;
        ++nextUiColor;
        uiColor = FileAsset.GetMaterialOfSoundParticleByColorName(System.Enum.GetName(typeof(UIColors), colorToGet));
    }

    public void InputNewPutPlay(PutPlayRequest newPutPlay){
        this.lastPlay = newPutPlay;
        this.playedThisTurn = true;
    }

    public override string ToString(){
        return string.Format("Player {0} - ({1:00, 2:00}) ({3:00}{4:00}) ({5}) - Played {6} - Role {7}",
            lastPlay.playerId, lastPlay.movementTo.x, lastPlay.movementTo.y, lastPlay.sound.x, lastPlay.sound.y, lastPlay.PlayerAttacked,
            playedThisTurn, role);
    }
}
