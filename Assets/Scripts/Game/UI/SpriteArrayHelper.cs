using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SpriteArrayHelper : UIHelper
{
    public enum Sprites {ToPlay, Played, Dead, Escaped}

    int playersToPlay = 0;
    int playersPlayed = 0;
    int playersDied = 0;
    int playersEscaped = 0;

    List<GameObject> spriteList;

    ///// Resources
    GameObject UIIconPrefab;
    Sprite hexagonIcon;
    Sprite skullIcon;
    Sprite spaceShipIcon;


    GameObject verticalGroup;

    public SpriteArrayHelper(GameObject uiObject) : base(uiObject){
        verticalGroup = uiObject;

        UIIconPrefab = (GameObject) Resources.Load<GameObject>("Prefabs/UI/UIIcon");
        hexagonIcon = (Sprite) Resources.Load<Sprite>("Sprites/UI/HexagonIcon");
        skullIcon = (Sprite) Resources.Load<Sprite>("Sprites/UI/SkullIcon");
        spaceShipIcon = (Sprite) Resources.Load<Sprite>("Sprites/UI/SpaceShipIcon");

        spriteList = new List<GameObject>();
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;

        playersToPlay = 0;
        playersPlayed = 0;
        playersDied = 0;
        playersEscaped = 0;
    }

    public void SetInitialSprites(ServerController serverController, int numberOfPlayers){
        int childCount = verticalGroup.transform.childCount;

        if(childCount != numberOfPlayers){
            if(childCount < numberOfPlayers){
                InstantiateSprites(numberOfPlayers - childCount);
            }else{
                RemoveSprites(childCount - numberOfPlayers);
            }

            int refCount = 0;
            ChangeIcons(numberOfPlayers, ref refCount, hexagonIcon, Color.red);
        }
    }

    public void SetUIComponent(ServerController serverController, int playersToPlay, int playersPlayed, int playersDied, int playersEscaped){
        int total = playersToPlay + playersPlayed + playersDied + playersEscaped;
        if( this.playersToPlay != playersToPlay || this.playersPlayed != playersPlayed || this.playersDied != playersDied || this.playersEscaped != playersEscaped ){

            this.playersToPlay = playersToPlay;
            this.playersPlayed = playersPlayed;
            this.playersDied = playersDied;
            this.playersEscaped = playersEscaped;

            int count = 0;

            //Debug.Log(string.Format("DEBUG - [Total {0}](ToPlay {2} e Played {3}) (Dead {4} - Escaped {5}) [childs {1}]", total,verticalGroup.transform.childCount,playersToPlay,playersPlayed,playersDied,playersEscaped));

            ChangeIcons(playersEscaped, ref count, spaceShipIcon);
            ChangeIcons(playersDied,    ref count, skullIcon);
            ChangeIcons(playersToPlay,  ref count, hexagonIcon, Color.red);
            ChangeIcons(playersPlayed,  ref count, hexagonIcon, Color.green);
        }
    }

    void ChangeIcons(int timesExpected, ref int controlCount, Sprite sprite, Color? color = null){
        while(timesExpected>0){
            Image image = spriteList[controlCount].GetComponent<Image>();
            image.sprite = sprite;
            if(color.HasValue){
                image.color = color.Value;
            }else{
                image.color = Color.white;
            }

            controlCount++;
            timesExpected--;
        }
    }

    public void InstantiateSprites(int length){
        for(int i=0 ; i<length ; i++){
            GameObject go = UnityEngine.Object.Instantiate(UIIconPrefab, new Vector2(0, 0), Quaternion.identity);
            go.name = string.Format("Icon{0}", spriteList.Count);
            go.transform.SetParent(verticalGroup.transform);
            go.transform.localScale = Vector3.one;

            spriteList.Add(go);
        }
    }

    public void RemoveSprites(int length){
        for(int i=0 ; i<length ; i++){
            int spriteIndex = spriteList.Count -1;
            UnityEngine.Object.Destroy(spriteList[spriteIndex]);
            spriteList.RemoveAt(spriteIndex);
        }
    }

    public void UpdateSprites(Sprites[] array){

    }
}