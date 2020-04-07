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
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;

        playersToPlay = 0;
        playersPlayed = 0;
        playersDied = 0;
        playersEscaped = 0;
    }

    public void SetUIComponent(int playersToPlay, int playersPlayed, int playersDied, int playersEscaped){
        int total = playersToPlay + playersPlayed + playersDied + playersEscaped;
        if( this.playersToPlay != playersToPlay || this.playersPlayed != playersPlayed || this.playersDied != playersDied || this.playersEscaped != playersEscaped ){

            // Debug.Log(string.Format("DEBUG - [Total {0}](ToPlay {2} e Played {3}) (Dead {4} - Escaped {5}) [childs {1}]", total,verticalGroup.transform.childCount,playersToPlay,playersPlayed,playersDied,playersEscaped));

            if(verticalGroup.transform.childCount < total){
                InstantiateSprites(total - verticalGroup.transform.childCount);
            }

            this.playersToPlay = playersToPlay;
            this.playersPlayed = playersPlayed;
            this.playersDied = playersDied;
            this.playersEscaped = playersEscaped;

            int count = 0;

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
        spriteList = new List<GameObject>();
        for(int i=0 ; i<length ; i++){
            GameObject go = UnityEngine.Object.Instantiate(UIIconPrefab, new Vector2(0, 0), Quaternion.identity);
            go.name = string.Format("Icon{0}", i);
            go.transform.SetParent(verticalGroup.transform);
            go.transform.localScale = Vector3.one;

            spriteList.Add(go);
        }
    }

    public void UpdateSprites(Sprites[] array){

    }
}