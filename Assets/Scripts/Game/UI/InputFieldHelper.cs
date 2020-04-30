using UnityEngine;
using UnityEngine.UI;

public class InputFieldHelper : UIHelper
{
    readonly Text placeholderTextComponent;

    public string PlaceholderText{
        get { return placeholderTextComponent.text; }
        set { placeholderTextComponent.text = value; }
    }

    readonly InputField inputField;

    public string Text{
        get { return inputField.text; }
        set { inputField.text = value; }
    }

    public InputFieldHelper(GameObject uiObject) : base(uiObject){
        this.placeholderTextComponent = base.uiObject.transform.Find("Placeholder").GetComponent<Text>();
        this.inputField = base.uiObject.GetComponent<InputField>();
    }

    public override void ResetUIComponent(){
        IsActive = true;
        IsVisible = true;
    }
}
