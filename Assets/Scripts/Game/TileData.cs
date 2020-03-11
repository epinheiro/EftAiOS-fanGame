using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileData
{
    public readonly string tileCode;
    public readonly Vector2Int tilePosition;
    public readonly BoardManager.PossibleTypes type;
    public readonly GameObject gameObjectReference;

    public TileData(GameObject gameObject, BoardManager.PossibleTypes type, string tileCode, Vector2Int tilePosition){
        this.gameObjectReference = gameObject;
        this.type = type;
        this.tileCode = tileCode;
        this.tilePosition = tilePosition;
    }
}
