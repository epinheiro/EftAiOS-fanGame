using UnityEngine;
using UnityEngine.UI;

public class SimpleTextHelper : UIHelper
{
    readonly Text textComponent;

    public string Text{
        get { return textComponent.text; }
        set { textComponent.text = value; }
    }

    public SimpleTextHelper(GameObject uiObject) : base(uiObject){
        this.textComponent = uiObject.GetComponent<Text>();
    }

    public void ChangeTextColor(Color newColor){
        textComponent.color = newColor;
    }
    public void ChangeTextColor(float r, float g, float b, float a){
        ChangeTextColor(new Color(r,g,b,a));
    }
    
    public override void ResetUIComponent(){
        IsActive = true;
        ChangeTextColor(Color.white);
    }
}
