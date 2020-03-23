using UnityEngine;

public class UIHelper 
{
    public delegate void BaseAction();

    protected readonly GameObject uiObject;
    public bool IsActive{
        get { return uiObject.activeSelf; }
        set { uiObject.SetActive(value); }
    }

    public UIHelper(GameObject uiObject){
        this.uiObject = uiObject;
    }

    public virtual void ResetUIComponent(){
        throw new System.Exception("Specific class do not have its own Reset");
    }
}
