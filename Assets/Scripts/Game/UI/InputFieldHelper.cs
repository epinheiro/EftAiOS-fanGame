using UnityEngine;
using UnityEngine.UI;

public class InputFieldHelper : UIHelper
{
    readonly Text placeholderTextComponent;

    public string PlaceholderText{
        get { return placeholderTextComponent.text; }
        set { placeholderTextComponent.text = value; }
    }

    readonly Text textComponent;

    public string Text{
        get { return textComponent.text; }
        set { textComponent.text = value; }
    }

    public InputFieldHelper(GameObject uiObject) : base(uiObject){
        this.placeholderTextComponent = base.uiObject.transform.Find("Placeholder").GetComponent<Text>();
        this.textComponent = base.uiObject.transform.Find("Text").GetComponent<Text>();
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;
    }
}
