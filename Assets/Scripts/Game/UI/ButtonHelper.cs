using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHelper : UIHelper
{
    readonly Button buttonComponent;
    readonly Image imageComponent;
    readonly Text textComponent;

    public string Text{
        get { return textComponent.text; }
        set { textComponent.text = value; }
    }

    public ButtonHelper(GameObject uiObject) : base(uiObject){
        this.buttonComponent = base.uiObject.GetComponent<Button>();
        this.imageComponent = base.uiObject.GetComponent<Image>();
        this.textComponent = base.uiObject.transform.Find("Text").GetComponent<Text>();

        preMadeButtons = new Dictionary<ButtonType, PreMadeButton>();
        preMadeButtons.Add(ButtonType.Default, new PreMadeButton(Color.white, Color.black));
        preMadeButtons.Add(ButtonType.Attack, new PreMadeButton(Color.red, Color.white));
        preMadeButtons.Add(ButtonType.DontAttack, new PreMadeButton(Color.blue, Color.white));
        preMadeButtons.Add(ButtonType.Connect, new PreMadeButton(Color.white, Color.black));

        SetButtonToPreMade(ButtonType.Default);
    }

    //////////////////////////////////////////////////
    //// Generic setups
    public override void ResetUIComponent(){
        SetUIComponent(ButtonType.Default);
    }
    public void SetUIComponent(ButtonType type){
        IsActive = true;
        IsVisible = true;
        CleanCallbacks();
        SetButtonToPreMade(type);
    }


    //////////////////////////////////////////////////
    //// Customize UI logic
    public void InsertCallback(BaseAction callBack){
        this.buttonComponent.onClick.AddListener(delegate(){callBack();});
    }

    public void CleanCallbacks(){
        this.buttonComponent.onClick.RemoveAllListeners();
    }

    //////////////////////////////////////////////////
    //// Customize UI visual
    public enum ButtonType {Default, Attack, DontAttack, Connect}
    Dictionary<ButtonType, PreMadeButton> preMadeButtons;
    struct PreMadeButton{
        public Color buttonColor;
        public Color textColor;

        public PreMadeButton(Color buttonColor, Color textColor){
            this.buttonColor = buttonColor;
            this.textColor = textColor;
        }
    }

    public void SetButtonToPreMade(ButtonType type, string buttonText = null){
        PreMadeButton buttonConfig;
        preMadeButtons.TryGetValue(type, out buttonConfig);

        this.ChangeButtonColor(buttonConfig.buttonColor);
        this.ChangeTextColor(buttonConfig.textColor);

        if(buttonText != null){
            string buttonString;
            if(string.IsNullOrEmpty(buttonText)){
                buttonString = "";
            }else{
                buttonString = buttonText;
            }
            this.Text = buttonString;
        }
    }


    public void AdaptButtonColor(Color newColor){
        new System.Exception("Not yet implemented!");
        // https://gamedev.stackexchange.com/questions/38536/given-a-rgb-color-x-how-to-find-the-most-contrasting-color-y
    }
    public void AdaptButtonColor(float r, float g, float b, float a){
        AdaptButtonColor(new Color(r,g,b,a));
    }

    public void ChangeButtonColor(Color newColor){
        imageComponent.color = newColor;
    }
    public void ChangeButtonColor(float r, float g, float b, float a){
        ChangeButtonColor(new Color(r,g,b,a));
    }
    
    public void ChangeTextColor(Color newColor){
        textComponent.color = newColor;
    }
    public void ChangeTextColor(float r, float g, float b, float a){
        ChangeTextColor(new Color(r,g,b,a));
    }
}
