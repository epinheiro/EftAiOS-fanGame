public class PlayerTurnData
{
    // UI Color controller
    public enum UIColors {Orange, Pink, Green, Cyan, Blue, Red, Yellow, Purple}
    public static int nextUiColor = UnityEngine.Random.Range(0, System.Enum.GetNames(typeof(UIColors)).Length);

    void IncrementeNextUIColor(){
        nextUiColor = (nextUiColor + 1) % System.Enum.GetNames(typeof(UIColors)).Length;
    }


    public PutPlayRequest lastPlay;
    public bool playedThisTurn;
    public ClientController.PlayerState role;
    public ClientController.PlayerState playingRole;

    UIColors _uiColor;

    public UIColors UIColor{
        get { return _uiColor; }
    }

    public int IntUIColor{
        get { return (int) _uiColor; }
    }

    public UnityEngine.Material GetUIColorMaterial(){
        return FileAsset.GetMaterialOfSoundParticleByColorName(System.Enum.GetName(typeof(UIColors), _uiColor));
    }

    public PlayerTurnData(PutPlayRequest lastPutPlay, ClientController.PlayerState role){
        this.lastPlay = lastPutPlay;
        this.playedThisTurn = false;
        this.role = role;
        playingRole = role;

        int colorToGet = nextUiColor;
        IncrementeNextUIColor();
        _uiColor = (UIColors) colorToGet;
    }

    public void InputNewPutPlay(PutPlayRequest newPutPlay){
        this.lastPlay = newPutPlay;
        this.playedThisTurn = true;
    }

    public override string ToString(){
        return string.Format("Player {0} {8} - ({1:00, 2:00}) ({3:00}{4:00}) ({5}) - Played {6} - Role {7}",
            lastPlay.playerId, lastPlay.movementTo.x, lastPlay.movementTo.y, lastPlay.sound.x, lastPlay.sound.y, lastPlay.PlayerAttacked,
            playedThisTurn, role, (UIColors) _uiColor);
    }
}
