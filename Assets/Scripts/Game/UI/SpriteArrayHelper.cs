using UnityEngine;
using UnityEngine.UI;

public class SpriteArrayHelper : UIHelper
{
    GameObject verticalGroup;

    public SpriteArrayHelper(GameObject uiObject) : base(uiObject){
        verticalGroup = uiObject;
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;
    }
}