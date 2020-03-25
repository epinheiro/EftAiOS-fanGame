using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    public enum Layout {Default, TwoButtons, InsertText, OnlyText, ConditionalButton}

    ButtonHelper button1;
    InputFieldHelper textInput;
    ButtonHelper button2;
    SimpleTextHelper infoText;

    Transform horizontalGroup;

    void Setup(){
        Transform VerticalGroup = transform.Find("VerticalGroup");
        horizontalGroup = VerticalGroup.Find("HorizontalGroup");
        button1 = new ButtonHelper(horizontalGroup.transform.Find("Button1").gameObject);
        textInput = new InputFieldHelper(horizontalGroup.transform.Find("InputField").gameObject);
        infoText = new SimpleTextHelper(VerticalGroup.transform.Find("InfoText").gameObject);
        button2 = new ButtonHelper(horizontalGroup.transform.Find("Button2").gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        Setup();
    }

    public void SetGenericLayout(Layout layout){
        switch(layout){
            case Layout.Default:
                SetUpUIElements(true, true, true, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Default, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Default);
                break;
            case Layout.TwoButtons:
                SetUpUIElements(true, false, false, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Attack, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                break;
            case Layout.InsertText:
                SetUpUIElements(true, true, false, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                break;
            case Layout.OnlyText:
                SetUpUIElements(false, false, true, null, null);
                break;
            case Layout.ConditionalButton:
                SetUpUIElements(true, false, true, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Attack);
                break;
        }
    }

    public void SetOnlyTextLayout(string text, Nullable<Color> color = null){
        SetGenericLayout(Layout.OnlyText);
        infoText.Text = text;
        if(color.HasValue) infoText.ChangeTextColor(color.Value);
    }

    public void SetTwoButtonsLayout(string leftButtonText, UIHelper.BaseAction leftButtonCallback, string rightButtonText, UIHelper.BaseAction rightButtonCallback){
        SetGenericLayout(Layout.TwoButtons);
        button1.Text = leftButtonText;
        button1.InsertCallback(leftButtonCallback);
        button2.Text = rightButtonText;
        button2.InsertCallback(rightButtonCallback);
    }

    public void SetInsertTextLayout(string placeholderText, string buttonText, string infoText, UIHelper.BaseAction buttonCallback){
        SetGenericLayout(Layout.InsertText);
        this.infoText.Text = infoText;
        textInput.PlaceholderText = placeholderText;
        button2.Text = buttonText;
        button2.InsertCallback(buttonCallback);
    }

    public void SetConditionalButtonLayout(string buttonText, string infoText, UIHelper.BaseAction buttonCallback){
        SetGenericLayout(Layout.ConditionalButton);
        this.infoText.Text = infoText;
        button2.Text = buttonText;
        button2.InsertCallback(buttonCallback);
    }
    public void SetConditionalButtonVisibility(bool isHiding){
        button2.IsVisible = isHiding;
    }

    void SetUpUIElements(bool resetHorizontalGroup, bool resetInputField, bool resetText, Nullable<ButtonHelper.ButtonType> resetButton1To = null, Nullable<ButtonHelper.ButtonType> resetButton2To = null){
        if(resetHorizontalGroup != horizontalGroup.gameObject.activeSelf){
            horizontalGroup.gameObject.SetActive(resetHorizontalGroup);
        }
        
        if(resetInputField) {
            textInput.ResetUIComponent();
        }else{
            textInput.IsActive = false;
        }

        if(resetText) {
            infoText.ResetUIComponent();
        }else{
            infoText.IsActive = false;
        }

        if(resetButton1To.HasValue) {
            button1.ResetUIComponent(resetButton1To.Value);
        }else{
            button1.IsActive = false;
        }

        if(resetButton2To.HasValue) {
            button2.ResetUIComponent(resetButton2To.Value);
        }else{
            button2.IsActive = false;
        }
    }
}
