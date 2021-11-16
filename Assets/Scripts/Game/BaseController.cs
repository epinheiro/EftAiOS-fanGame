using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    public void SetupApplication(){
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        SetupApplicationLogs();
    }

    public void SetupApplicationLogs(){
        // Supressing intended script logs to a single line
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Error, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Assert, StackTraceLogType.None);
    }

    /// Audio related fields
    AudioController _audioController;
    public AudioController Audio{
        get { return _audioController; }
    }

    public void InstantiateAudioController(){
        _audioController = new AudioController(this.gameObject);
    }

    /// UI canvas related fields
    public GameObject uiCanvasPrefab;
    protected UIController _uiController;
    public UIController UIController{
        get { return _uiController; }
        set { 
            if(_uiController==null){
                _uiController = value;
            }else{
                throw new System.Exception(string.Format("UIController already exists on node {0}", this.name));
            }
        }
    }

    public void GetUICanvasReference(){
        UIController = GameObject.Find("UICanvas").GetComponent<UIController>();
    }

    /// BoardManager related fields
    public GameObject boardManagerPrefab;
    protected BoardManager _boardManager;

    public void InstantiateBoardManager(){
        GameObject go = Instantiate(this.boardManagerPrefab, new Vector2(0, 0), Quaternion.identity);
        this._boardManager = go.GetComponent<BoardManager>();
        go.name = "BoardManager";
    }

    public delegate void BaseAction();

    public Coroutine DelayedCall(BaseAction function, float delay, bool repeat = false){
        return StartCoroutine(DelayedCallCoroutine(function, delay, repeat));
    }

    IEnumerator DelayedCallCoroutine(BaseAction function, float delay, bool repeat){
        do{
            yield return new WaitForSeconds(delay);
            function();
        }while(repeat);
    }

    public void LoadScene(string name){
        UIController.ActivateLoadingScreen();

        UnityEngine.SceneManagement.SceneManager.LoadScene(name);
    }

    /// BoardManager related calls
    public void CleanSoundGlowTiles(string remainingTileCode = ""){
        _boardManager.CleanSoundGlowTiles(remainingTileCode);
    }

    public void CleanMovementTile(string codeToDelete){
        _boardManager.CleanMovementTile(codeToDelete); // INFO - related to WaitingPlayersClientState Option2 feedback
    }

    public void GlowPossibleMovements(string tileCode, int movement, ClientController.PlayerState role){
        _boardManager.GlowPossibleMovements(tileCode, movement, role);
    }

    public void CleanMovementGlowTiles(string remainingTileCode = ""){
        _boardManager.CleanMovementGlowTiles(remainingTileCode);
    }

    public BoardManager.PossibleTypes GetTileType(string tileCode){
        return _boardManager.GetTileType(tileCode);
    }

    public void GlowPossibleNoises(){
        _boardManager.GlowPossibleNoises();
    }

    public void CleanLastSoundEffects(){
        _boardManager.CleanLastSoundEffects();
    }

    public void LastSoundEffects(List<NoiseInfo> soundTileCodes){
        _boardManager.LastSoundEffects(soundTileCodes);
    }
}
