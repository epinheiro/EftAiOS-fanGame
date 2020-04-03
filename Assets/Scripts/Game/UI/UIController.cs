using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    enum Layout {Default, TwoButtons, InsertText, OnlyText, ConditionalButton, AllInactive}

    ///// Header /////
    Transform headerGroup;
    // Line 1 group
    Transform line1Group;
    SimpleTextHelper infoText;
    SpriteArrayHelper infoGroup;
    // Line 2 group
    Transform line2Group;
    ButtonHelper button1;
    InputFieldHelper textInput;
    ButtonHelper button2;

    ///// Footer /////
    Transform footerGroup;
    SimpleTextHelper progressText;
    ProgressBarHelper progressBar;
    SimpleTextHelper totalText;

    void Setup(){
        HeaderSetup();
        FooterSetup();
        SetGenericLayout(Layout.AllInactive);
    }

    void HeaderSetup(){
        headerGroup = transform.Find("HeaderGroup");
        H_Line1Setup();
        H_Line2Setup();
    }

    void H_Line1Setup(){
        line1Group = headerGroup.Find("Line1Group");
        infoText = new SimpleTextHelper(line1Group.transform.Find("InfoText").gameObject);
        infoGroup = new SpriteArrayHelper(line1Group.transform.Find("InfoGroup").gameObject);
    }

    void H_Line2Setup(){
        line2Group = headerGroup.Find("Line2Group");
        button1 = new ButtonHelper(line2Group.transform.Find("Button1").gameObject);
        textInput = new InputFieldHelper(line2Group.transform.Find("InputField").gameObject);
        button2 = new ButtonHelper(line2Group.transform.Find("Button2").gameObject);
    }

    void FooterSetup(){
        footerGroup = transform.Find("FooterGroup");
        progressText = new SimpleTextHelper(footerGroup.transform.Find("Progress").gameObject);
        progressBar = new ProgressBarHelper(footerGroup.transform.Find("ProgressBar").gameObject);
        totalText = new SimpleTextHelper(footerGroup.transform.Find("Total").gameObject);
    }

    // Start is called before the first frame update
    void Awake()
    {
        Setup();
    }

    void SetGenericLayout(Layout layout){
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
            case Layout.AllInactive:
                SetUpUIElements(false, false, false, null, null);
                break;
        }
    }

    public void SetAllInactiveLayout(){
        SetGenericLayout(Layout.AllInactive);
    }

    public void SetOnlyTextLayout(string text, Nullable<Color> color = null){
        SetGenericLayout(Layout.OnlyText);
        infoText.Text = text;
        if(color.HasValue) infoText.ChangeTextColor(color.Value);
    }

    public void SetOnlyTextInfoText(string newString = ""){
        infoText.Text = newString;
    }

    public void SetTwoButtonsLayout(string leftButtonText, UIHelper.BaseAction leftButtonCallback, string rightButtonText, UIHelper.BaseAction rightButtonCallback){
        SetGenericLayout(Layout.TwoButtons);
        button1.Text = leftButtonText;
        button1.InsertCallback(leftButtonCallback);
        button2.Text = rightButtonText;
        button2.InsertCallback(rightButtonCallback);
    }

    public void SetTwoButtonsVisibility(bool isVisible){
        button1.IsVisible = isVisible;
        button2.IsVisible = isVisible;
    }

    public void SetInsertTextLayout(string placeholderText, string buttonText, string infoText, UIHelper.BaseAction buttonCallback){
        SetGenericLayout(Layout.InsertText);
        this.infoText.Text = infoText;
        textInput.PlaceholderText = placeholderText;
        button2.Text = buttonText;
        button2.InsertCallback(buttonCallback);
    }

    public void SetInsertTextButtonAttributes(string buttonText, Color? buttonColor = null){
        if(buttonText != null) button2.Text = buttonText;
        if(buttonColor.HasValue) button2.ChangeButtonColor(buttonColor.Value);
    }

    public void SetInsertTextButtonVisibility(bool isVisible){
        button2.IsVisible = isVisible;
    }

    public string GetInsertedText(){
        return textInput.Text;
    }

    public void SetConditionalButtonLayout(string buttonText, string infoText, UIHelper.BaseAction buttonCallback){
        SetGenericLayout(Layout.ConditionalButton);
        this.infoText.Text = infoText;
        button2.Text = buttonText;
        button2.InsertCallback(buttonCallback);
    }
    public void SetConditionalButtonText(string text){
        button2.Text = text;
    }
    public void SetConditionalButtonVisibility(bool isHiding){
        button2.IsVisible = isHiding;
    }

    void SetUpUIElements(bool resetHorizontalGroup, bool resetInputField, bool resetText, Nullable<ButtonHelper.ButtonType> resetButton1To = null, Nullable<ButtonHelper.ButtonType> resetButton2To = null){
        infoGroup.IsActive = false; // TODO - change
        footerGroup.gameObject.SetActive(false); // TODO - change
        
        if(resetHorizontalGroup != line2Group.gameObject.activeSelf){
            line2Group.gameObject.SetActive(resetHorizontalGroup);
        }
        
        if(resetInputField) {
            textInput.ResetUIComponent();
        }else{
            textInput.IsActive = false;
        }

        if(resetText) {
            line1Group.gameObject.SetActive(true);
            infoText.ResetUIComponent();
        }else{
            line1Group.gameObject.SetActive(false);
            infoText.IsActive = false;
        }

        if(!resetButton1To.HasValue && !resetButton2To.HasValue){
            button1.IsActive = false;
            button2.IsActive = false;
            line2Group.gameObject.SetActive(false);
        }else{
            if(resetButton1To.HasValue) {
                line2Group.gameObject.SetActive(true);
                button1.ResetUIComponent(resetButton1To.Value);
            }else{
                button1.IsActive = false;
            }

            if(resetButton2To.HasValue) {
                line2Group.gameObject.SetActive(true);
                button2.ResetUIComponent(resetButton2To.Value);
            }else{
                button2.IsActive = false;
            }
        }
    }
}
