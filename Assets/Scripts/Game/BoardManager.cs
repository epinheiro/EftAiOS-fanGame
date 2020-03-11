using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class BoardManager : MonoBehaviour
{
    public enum PossibleTypes {EventTile, SilentTile, AlienNest, HumanDorm, EscapePod, Empty}
    public Color[] colors = {Color.gray, Color.white, Color.red, Color.blue, Color.green, Color.black};

    struct Offset{
        public float x;
        public float y;
    }

    struct Spacing{
        public float x;
        public float y;
    }

    readonly Offset spriteOffset = new Offset{x=3.3f, y=-1.9f};

    readonly Spacing evenColumnSpacing = new Spacing{x = 0, y = 0};
    readonly Spacing oddColumnSpacing = new Spacing{x = 1.65f, y = 0.95f};

    GameObject hexagonPrefab;
    GameObject mapInSceneReference;

    Dictionary<string, TileData> mapTiles;
    string humanDormCode;
    public string HumanDormCode {
        get { return humanDormCode; }
    }
    string alienNestCode;
    public string AlienNestCode {
        get { return alienNestCode; }
    }


    // Start is called before the first frame update
    void Start()
    {
        hexagonPrefab = GameObject.Find("Hexagon");
        mapInSceneReference = GameObject.Find("Map");

        mapTiles = new Dictionary<string, TileData>();
        CreateMap("Galilei");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public TileData GetSpawnPointTileData(ClientController.PlayerState state){
        TileData data;

        switch(state){
            case ClientController.PlayerState.Alien:{}
                mapTiles.TryGetValue(alienNestCode, out data);
                return data;

            case ClientController.PlayerState.Human:
                mapTiles.TryGetValue(humanDormCode, out data);
                return data;

            default:
                throw new System.Exception(string.Format("Tile {0} has no spawn point", (ClientController.PlayerState) state));
        }
    }

    List<string> PossibleMovements(string currentTileCode, int movement = 1){
        List<string> movementsList = new List<string>();

        if (movement < 1 || movement > 2) throw new Exception("Movement must be 1 or 2");
    
        string[] parseResult = ParseTileCode(currentTileCode);
        string columnId = parseResult[0];
        string rowId = parseResult[1];

        int columnNumber = TranslateColumnIdToNumber(columnId);
        int rowNumber = TranslateRowIdToNumber(rowId);

        // TODO - Technical debt - the tile selection is almost hardcoded and disconsider navigation by jumping obstacles

        ///////// Check 1 radius /////////
        // Same column
        CheckTileCodeAndInsert(movementsList, columnNumber, rowNumber-1);
        CheckTileCodeAndInsert(movementsList, columnNumber, rowNumber+1);
        // Neighbor column
        if (columnNumber % 2 == 0){
            CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber  );
            CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber+1);
            CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber  );
            CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber+1);
        }else{
            CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber-1);
            CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber  );
            CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber-1);
            CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber  );
        }
        ///////// Check 1 radius /////////

        ///////// Check 2 radius /////////
        if (movement == 2){
            // Same column
            CheckTileCodeAndInsert(movementsList, columnNumber, rowNumber-2);
            CheckTileCodeAndInsert(movementsList, columnNumber, rowNumber+2);
            // Neighbor column
            if (columnNumber % 2 == 0){
                CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber-1);
                CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber+2);
                CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber-1);
                CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber+2);
            }else{
                CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber-2);
                CheckTileCodeAndInsert(movementsList, columnNumber-1, rowNumber+1);
                CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber-2);
                CheckTileCodeAndInsert(movementsList, columnNumber+1, rowNumber+1);
            }

            // Neighbor's Neighbor column
            CheckTileCodeAndInsert(movementsList, columnNumber-2, rowNumber-1);
            CheckTileCodeAndInsert(movementsList, columnNumber-2, rowNumber  );
            CheckTileCodeAndInsert(movementsList, columnNumber-2, rowNumber+1);
            CheckTileCodeAndInsert(movementsList, columnNumber+2, rowNumber-1);
            CheckTileCodeAndInsert(movementsList, columnNumber+2, rowNumber  );
            CheckTileCodeAndInsert(movementsList, columnNumber+2, rowNumber+1);
        }
        ///////// Check 2 radius /////////
        
        return movementsList;
    }

    void CheckTileCodeAndInsert(List<string> list, int columnNumber, int rowNumber){
        string tileCode = TranslateTileNumbersToString(columnNumber, rowNumber);
        
        if (IsTileCodeExists(tileCode)) 
            list.Add(tileCode);
    }

    bool IsTileCodeExists(string tileCode){
        return mapTiles.ContainsKey(tileCode);
    }

    void CreateMap(string mapName){
        string[] tileInfo = ParseMap(FileAsset.GetMapTileInfo(mapName));

        for (int i=0; i < tileInfo.Length; i = i + 2){
            string tileId = tileInfo[i];
            string tileType = tileInfo[i+1];

            CreateTileInMap(tileId, tileType);
        }
        FitMapOnScreen();
        hexagonPrefab.SetActive(false); // Hide prefab hexagon on screen
    }

    string[] ParseTileCode(string code){
        //https://stackoverflow.com/a/1968058
        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        Match result = re.Match(code);

        return new string[]{
            result.Groups[1].Value, //alphaPart
            result.Groups[2].Value  //numberPart
        };
    }

    void FitMapOnScreen(){
        // TODO - technical dept - rescalling map proportions to 16:10
        mapInSceneReference.transform.position = new Vector3(-6, 4, 0);
        mapInSceneReference.transform.localScale = new Vector3(.3f, .3f, 1);
    }
    void CreateTileInMap(string tileId, string tileType){
        string[] result = ParseTileCode(tileId);
        string columnId = result[0];
        string rowId = result[1];

        int rowNumber = TranslateRowIdToNumber(rowId);
        int columnNumber = TranslateColumnIdToNumber(columnId);

        int tileTypeNumber = TranslateTileTypeToEnumNumber(tileType);

        float xPos;
        float yPos;

        if (columnNumber%2 == 0){
            xPos = evenColumnSpacing.x + (columnNumber/2) * spriteOffset.x;
            yPos = evenColumnSpacing.y + rowNumber * spriteOffset.y;
        }else{
            xPos = oddColumnSpacing.x + ((columnNumber-1)/2) * spriteOffset.x;
            yPos = oddColumnSpacing.y + rowNumber * spriteOffset.y;
        }

        GameObject go = Instantiate(hexagonPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
        go.transform.parent = mapInSceneReference.transform;
        go.GetComponent<SpriteRenderer>().color = colors[tileTypeNumber];

        switch((PossibleTypes) tileTypeNumber){
            case PossibleTypes.AlienNest:
                alienNestCode = tileId;
            break;
            
            case PossibleTypes.HumanDorm:
                humanDormCode = tileId;
            break;
        }

        string code = TranslateTileNumbersToString(columnNumber, rowNumber);

        go.name = code;

        mapTiles.Add(
            code, 
            new TileData(
                go, 
                (PossibleTypes) tileTypeNumber, 
                tileId, 
                new Vector2Int(columnNumber, rowNumber)
            )
        );
    }

    ////////////////////////////////////////
    ///////// Static board methods /////////
    ////////////////////////////////////////

    static public string TranslateTileNumbersToString(int columnNumber, int rowNumber){
        return string.Format("{0}{1:00}", TranslateNumberToColumnId(columnNumber), TranslateNumberToRowId(rowNumber));
    }

    static public int TranslateTileTypeToEnumNumber(string typeTypeString){
        if (Enum.IsDefined(typeof(PossibleTypes), typeTypeString)){
            PossibleTypes output;
            Enum.TryParse(typeTypeString, false, out output);
            return (int) output;
        } else {
            throw new System.Exception(string.Format("Tile type {0} is not supported", typeTypeString));
        }
    }

    static public int TranslateRowIdToNumber(string rowId){
        return Int32.Parse(rowId);
    }

    static public string TranslateNumberToRowId(int rowNumber){
        return string.Format("{0:00}", rowNumber);
    }

    static public int TranslateColumnIdToNumber(string columnId){
        if (columnId.Length == 1){
            return (int) columnId[0] - 64;
        }else{
            throw new Exception("Not yet implemented column id with more than 1 character");
        }
    }

    static public string TranslateNumberToColumnId(int columnNumber){
        int alphaCode = columnNumber + 64;
        return string.Format("{0}", (char) alphaCode);
    }

    static public string[] ParseMap(string mapString){
        char[] delimiterChars = { ' ', ',', '{', '}', '"', ':', '\n', '\r'};
        return mapString.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
    }
}
