using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
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
    static protected void InstantiateBoardManager(BaseController controller){
        GameObject go = Instantiate(controller.boardManagerPrefab, new Vector2(0, 0), Quaternion.identity);
        controller.BoardManagerRef = go.GetComponent<BoardManager>();
        go.name = "BoardManager";
    }

    public delegate void BaseAction();

    public void DelayedCall(BaseAction function, float delay, bool repeat = false){
        StartCoroutine(DelayedCallCoroutine(function, delay, repeat));
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
