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
}
