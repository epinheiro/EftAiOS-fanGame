using System.Collections;
using UnityEngine;

public class BaseController : MonoBehaviour
{
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

    public void InstantiateCanvas(){
        GameObject go = Instantiate(uiCanvasPrefab, new Vector2(0, 0), Quaternion.identity);
        UIController = go.GetComponent<UIController>();
        go.name = "UICanvas";
    }

    /// BoardManager related fields
    public GameObject boardManagerPrefab;
    protected BoardManager _boardManager;
    public BoardManager BoardManagerRef{
        get { return _boardManager; }
        set { 
            if(_boardManager==null){
                _boardManager = value;
            }else{
                throw new System.Exception(string.Format("BoardManager already exists on node {0}", this.name));
            }
        }
    }
    public void InstantiateBoardManager(){
        GameObject go = Instantiate(this.boardManagerPrefab, new Vector2(0, 0), Quaternion.identity);
        this.BoardManagerRef = go.GetComponent<BoardManager>();
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

    public void CreateMidScreenText(string text){
        GUILayout.BeginArea(new Rect(100, 10, 100, 100));
        GUILayout.TextArea(text);
        GUILayout.EndArea();
    }
}
