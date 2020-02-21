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

    public GameObject hexagonPrefab;
    public GameObject mapReference;


    // Start is called before the first frame update
    void Start()
    {
        CreateMap("Galilei");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateMap(string mapName){
        string[] tileInfo = ParseMap(FileAsset.GetMapTileInfo(mapName));

        for (int i=0; i < tileInfo.Length; i = i + 2){
            //// Separating tile id
            string tileId = tileInfo[i];
            //https://stackoverflow.com/a/1968058
            Regex re = new Regex(@"([a-zA-Z]+)(\d+)");
            Match result = re.Match(tileInfo[i]);
            string column = result.Groups[1].Value; //alphaPart
            string row = result.Groups[2].Value; //numberPart

            //// Separating tile type
            string tileType = tileInfo[i+1];

            CreateTileInMap(row, column, tileType);
        }
    }

    void CreateTileInMap(string rowId, string columnId, string tileType){
        int rowNumber = TranslateRowIdToNumber(rowId);
        int columnNumber = TranslateColumnIdToNumber(columnId);
        int tileTypeNumber = TranslateTileTypeToEnum(tileType);

        float xPos;
        float yPos;
        // Color clr; //debug

        if (columnNumber%2 == 0){
            xPos = evenColumnSpacing.x + (columnNumber/2) * spriteOffset.x;
            yPos = evenColumnSpacing.y + rowNumber * spriteOffset.y;
            // clr = Color.red; //debug
        }else{
            xPos = oddColumnSpacing.x + ((columnNumber-1)/2) * spriteOffset.x;
            yPos = oddColumnSpacing.y + rowNumber * spriteOffset.y;
            // clr = Color.blue; //debug
        }

        GameObject go = Instantiate(hexagonPrefab, new Vector3(xPos, yPos, 0), Quaternion.identity);
        go.transform.parent = mapReference.transform;
        go.GetComponent<SpriteRenderer>().color = colors[tileTypeNumber];
        // go.GetComponent<SpriteRenderer>().color = clr; //debug
        
    }

    int TranslateTileTypeToEnum(string typeTypeString){
        if (Enum.IsDefined(typeof(PossibleTypes), typeTypeString)){
            PossibleTypes output;
            Enum.TryParse(typeTypeString, false, out output);
            return (int) output;
        } else {
            throw new System.Exception(string.Format("Language {0} is not supported", typeTypeString));
        }
    }

    int TranslateRowIdToNumber(string rowId){
        return Int32.Parse(rowId);
    }

    int TranslateColumnIdToNumber(string columnId){
        if (columnId.Length == 1){
            return (int) columnId[0] - 64;
        }else{
            throw new Exception("Not yet implemented column id with more than 1 character");
        }
    }

    string[] ParseMap(string mapString){
        char[] delimiterChars = { ' ', ',', '{', '}', '"', ':', '\n', '\r'};
        return mapString.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
    }
}
