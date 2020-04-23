using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;
using UnityEngine.UI;

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

    public GameObject hexagonPrefab;
    public GameObject glowMovementPrefab;
    public GameObject glowSoundPrefab;
    public GameObject soundEffectPrefab;
    public GameObject attackEffectPrefab;

    Dictionary<string, TileData> mapTiles;
    GameObject glowMovementTilesAggregator;
    GameObject glowSoundTilesAggregator;
    GameObject soundEffectsAggregator;
    string humanDormCode;
    public string HumanDormCode {
        get { return humanDormCode; }
    }
    string alienNestCode;
    public string AlienNestCode {
        get { return alienNestCode; }
    }

    public BaseController controller;

    // Start is called before the first frame update
    void Start()
    {
        glowMovementTilesAggregator = new GameObject("Glow movement tiles");
        glowSoundTilesAggregator = new GameObject("Glow sound tiles");
        soundEffectsAggregator = new GameObject("Sound effects");
        glowMovementTilesAggregator.transform.parent = this.transform;
        glowSoundTilesAggregator.transform.parent = this.transform;
        soundEffectsAggregator.transform.parent = this.transform;

        mapTiles = new Dictionary<string, TileData>();
        CreateMap("Galilei");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public BoardManager.PossibleTypes GetTileType(string tileCode){
        TileData data;
        if(!mapTiles.TryGetValue(tileCode, out data)){
            throw new System.Exception(string.Format("Try to get a invalid tile code {0}", tileCode));
        }

        return data.type;
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

    public void LastSoundEffects(List<NoiseInfo> soundTileCodes){
        foreach(NoiseInfo info in soundTileCodes){
            if(info.isAttack){
                GlowTile(info.tileCode, attackEffectPrefab, soundEffectsAggregator);
            }else{
                GlowTile(info.tileCode, soundEffectPrefab, soundEffectsAggregator);
            }
        }
    }

    public void CleanLastSoundEffects(){
        int objectsNumber = soundEffectsAggregator.transform.childCount;

        for (int i=0 ; i<objectsNumber ; i++){
            GameObject child = soundEffectsAggregator.transform.GetChild(i).gameObject;
            GameObject.Destroy(child);
        }
    } 

    public void GlowPossibleNoises(){
        foreach(string key in mapTiles.Keys){
            if(GetTileType(key) == PossibleTypes.EventTile){
                GlowTile(key, glowSoundPrefab, glowSoundTilesAggregator);
            }
        }
    }

    public void GlowPossibleMovements(string tileCode, int movement, ClientController.PlayerState role){
        List<TileData> movementsList = PossibleMovements(tileCode, movement, role);

        foreach(TileData data in movementsList){
            GlowTile(data.tileCode, glowMovementPrefab, glowMovementTilesAggregator);
        }
    }

    void GlowTile(string tileCode, GameObject glowUsed, GameObject aggregatorUsed){
        TileData data;
        mapTiles.TryGetValue(tileCode, out data);

        Vector2 worldPosition = data.gameObjectReference.transform.position;

        GameObject go = Instantiate(glowUsed, new Vector3(0, 0, 0), Quaternion.identity);
        go.name = tileCode;
        FitUIElementOnServerScreen(go);
        go.transform.parent = aggregatorUsed.transform;
        go.transform.position = new Vector3(worldPosition.x, worldPosition.y, 1);
    }

    List<TileData> PossibleMovements(string startingTile, int movement, ClientController.PlayerState role){
        List<TileData> movementsList = new List<TileData>();

        List<string> invalidTileList = CreateInvalidTileList(startingTile, role);

        PropagateMovement(movementsList, startingTile, startingTile, movement, invalidTileList);

        return movementsList;
    }

    void PropagateMovement(List<TileData> list, string startingTileCode, string tileCode, int movementsLeft, List<string> invalidTileList){
        if(movementsLeft==0) return;

        string[] parseResult = ParseTileCode(tileCode);
        int columnNumber = TranslateColumnIdToNumber(parseResult[0]);
        int rowNumber = TranslateRowIdToNumber(parseResult[1]);

        CheckTileCodeAndInsert(list, startingTileCode, columnNumber, rowNumber-1, movementsLeft, invalidTileList);
        CheckTileCodeAndInsert(list, startingTileCode, columnNumber, rowNumber+1, movementsLeft, invalidTileList);
        // Neighbor column
        if (columnNumber % 2 == 0){
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber-1, rowNumber  , movementsLeft, invalidTileList);
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber-1, rowNumber+1, movementsLeft, invalidTileList);
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber+1, rowNumber  , movementsLeft, invalidTileList);
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber+1, rowNumber+1, movementsLeft, invalidTileList);
        }else{
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber-1, rowNumber-1, movementsLeft, invalidTileList);
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber-1, rowNumber  , movementsLeft, invalidTileList);
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber+1, rowNumber-1, movementsLeft, invalidTileList);
            CheckTileCodeAndInsert(list, startingTileCode, columnNumber+1, rowNumber  , movementsLeft, invalidTileList);
        }
    }

    void CheckTileCodeAndInsert(List<TileData> list, string startingTileCode, int columnNumber, int rowNumber, int movementsLeft, List<string> invalidTileList){
        string tileCode = TranslateTileNumbersToCode(columnNumber, rowNumber);

        if (TileExistsInMap(tileCode) && IsTileValid(startingTileCode, tileCode, invalidTileList)) {
            if(!TileExistsInList(list, tileCode)){
                TileData data;
                mapTiles.TryGetValue(tileCode, out data);
                list.Add(data);
            }
            PropagateMovement(list, startingTileCode, tileCode, movementsLeft-1, invalidTileList);
        }
    }

    bool IsTileValid(string startingTileCode, string tileCode, List<string> invalidTileList){
        foreach(string code in invalidTileList){
            if(string.Equals(code, tileCode)) return false;
        }
        return true;
    }

    List<string> CreateInvalidTileList(string startingTileCode = "", ClientController.PlayerState specificRole = ClientController.PlayerState.Unassigned){
        List<string> list = new List<string>();

        if(!string.IsNullOrEmpty(startingTileCode)) list.Add(startingTileCode);

        foreach(string key in mapTiles.Keys){
            switch(GetTileType(key)){
                case PossibleTypes.HumanDorm:
                case PossibleTypes.AlienNest:
                    list.Add(key);
                    break;
                case PossibleTypes.EscapePod:
                    if(specificRole != ClientController.PlayerState.Human){
                        list.Add(key);
                    }
                    break;
            }
        }

        return list;
    }

    bool TileExistsInList(List<TileData> list, string tileCode){
        foreach(TileData data in list){
            if(data.tileCode == tileCode) return true;
        }
        return false;
    }

    bool TileExistsInMap(string tileCode){
        return mapTiles.ContainsKey(tileCode);
    }
    public void CleanGlowTiles(){
        int glowTilesNumber = glowMovementTilesAggregator.transform.childCount;

        for (int i=0 ; i<glowTilesNumber ; i++){
            GameObject child = glowMovementTilesAggregator.transform.GetChild(i).gameObject;
            GameObject.Destroy(child);
        }

        glowTilesNumber = glowSoundTilesAggregator.transform.childCount;

        for (int i=0 ; i<glowTilesNumber ; i++){
            GameObject child = glowSoundTilesAggregator.transform.GetChild(i).gameObject;
            GameObject.Destroy(child);
        }
    }

    public void CleanSoundGlowTiles(string remainingTileCode = ""){
        CleanGlowTiles(remainingTileCode, glowSoundTilesAggregator);
    }

    public void CleanMovementTile(string codeToDelete){
        int glowTilesNumber = glowMovementTilesAggregator.transform.childCount;

        for (int i=0 ; i<glowTilesNumber ; i++){
            GameObject child = glowMovementTilesAggregator.transform.GetChild(i).gameObject;
            if(string.Equals(child.name, codeToDelete)){
                GameObject.Destroy(child);
                break;
            }
        }
    }


    public void CleanMovementGlowTiles(string remainingTileCode = ""){
        CleanGlowTiles(remainingTileCode, glowMovementTilesAggregator);
    }

    public void CleanGlowTiles(string remainingTileCode, GameObject aggregatorUsed){
        int glowTilesNumber = aggregatorUsed.transform.childCount;

        for (int i=0 ; i<glowTilesNumber ; i++){
            GameObject child = aggregatorUsed.transform.GetChild(i).gameObject;
            if(!string.Equals(child.name, remainingTileCode)){
                GameObject.Destroy(child);
            }else{
                Destroy(child.GetComponent<PolygonCollider2D>());
            }
        }
    }

    void CreateMap(string mapName){
        string[] tileInfo = ParseMap(FileAsset.GetMapTileInfo(mapName));

        for (int i=0; i < tileInfo.Length; i = i + 2){
            string tileId = tileInfo[i];
            string tileType = tileInfo[i+1];

            CreateTileInMap(tileId, tileType);
        }
        FitUIElementOnServerScreen(this.gameObject);
    }

    void FitUIElementOnClientScreen(GameObject go){
        // TODO - technical dept - rescalling map proportions to 16:10
        go.transform.position = new Vector3(-6, 4, 0);
        go.transform.localScale = new Vector3(.3f, .3f, 1);
    }

    void FitUIElementOnServerScreen(GameObject go){
        // TODO - technical dept - rescalling map proportions to 16:10
        go.transform.position = new Vector3(-4.5f, 3, 0);
        go.transform.localScale = new Vector3(.23f, .23f, 1);
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
        go.transform.parent = this.transform;
        go.GetComponent<SpriteRenderer>().color = colors[tileTypeNumber];

        switch((PossibleTypes) tileTypeNumber){
            case PossibleTypes.AlienNest:
                alienNestCode = tileId;
            break;
            
            case PossibleTypes.HumanDorm:
                humanDormCode = tileId;
            break;
        }

        string code = TranslateTileNumbersToCode(columnNumber, rowNumber);

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

    static public string[] ParseTileCode(string code){
        //https://stackoverflow.com/a/1968058
        Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
        Match result = re.Match(code);

        return new string[]{
            result.Groups[1].Value, //alphaPart
            result.Groups[2].Value  //numberPart
        };
    }
    static public Vector2Int TileCodeToVector2Int(string tileCode){
        string[] parts = ParseTileCode(tileCode);
        return new Vector2Int(TranslateColumnIdToNumber(parts[0]), TranslateRowIdToNumber(parts[1]));
    }

    static public string TranslateTilePositionToCode(Vector2Int tilePosition){
        return TranslateTileNumbersToCode(tilePosition.x, tilePosition.y);
    }

    static public string TranslateTileNumbersToCode(int columnNumber, int rowNumber){
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
