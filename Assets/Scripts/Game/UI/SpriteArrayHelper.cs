using UnityEngine;
using UnityEngine.UI;

public class SpriteArrayHelper : UIHelper
{
    public enum Sprites {Playing, Played, Dead, Escaped}

    GameObject verticalGroup;

    public SpriteArrayHelper(GameObject uiObject) : base(uiObject){
        verticalGroup = uiObject;
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;
    }
}