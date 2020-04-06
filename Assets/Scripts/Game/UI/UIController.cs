using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    enum Layout {TwoButtons, InsertText, OnlyText, ConditionalButton, AllActive, AllInactive}

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

    //////////////////


    void Awake()
    {
        Setup();
    }

    void SetGenericLayout(Layout layout){
        switch(layout){
            case Layout.TwoButtons:
                SetHeaderUIElements(false, null, false, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Attack, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                SetFooterUIElements(false);
                break;
            case Layout.InsertText:
                SetHeaderUIElements(false, null, true, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                SetFooterUIElements(false);
                break;
            case Layout.OnlyText:
                SetHeaderUIElements(true, null, false, null, null);
                SetFooterUIElements(false);
                break;
            case Layout.ConditionalButton:
                SetHeaderUIElements(true, null, false, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Attack);
                SetFooterUIElements(false);
                break;
            case Layout.AllActive:
                SetHeaderUIElements(true, null, true, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Default, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Default);
                SetFooterUIElements(false);
                break;
            case Layout.AllInactive:
                SetHeaderUIElements(false, null, false, null, null);
                SetFooterUIElements(false);
                break;
        }
    }

    // TwoButtons /////////////////////////
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

    // InsertText /////////////////////////
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

    // OnlyText /////////////////////////
    public void SetOnlyTextLayout(string text, Nullable<Color> color = null){
        SetGenericLayout(Layout.OnlyText);
        infoText.Text = text;
        if(color.HasValue) infoText.ChangeTextColor(color.Value);
    }

    public void SetOnlyTextInfoText(string newString = ""){
        infoText.Text = newString;
    }

    // ConditionalButton /////////////////////////
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

    // AllInactive /////////////////////////
    public void SetAllInactiveLayout(){
        SetGenericLayout(Layout.AllInactive);
    }

    /////////////////////////////////////////////////////

    // Line 1 //
    void Line1SetActive(bool IsActive){
        if(IsActive != line1Group.gameObject.activeSelf){
            line1Group.gameObject.SetActive(IsActive);
        }
    }

    void InfoTextSetActive(bool IsActive){
        if(IsActive) {
            line1Group.gameObject.SetActive(true);
            infoText.ResetUIComponent();
        }else{
            infoText.IsActive = false;
        }
    }

    void InfoGroupSetArray(SpriteArrayHelper.Sprites[] array){
        if(array!=null){
            infoGroup.SetUIComponent(array);
        }else{
            infoGroup.IsActive = false;
        }
    }

    // Line 2 //
    void Line2SetActive(bool IsActive){
        if(IsActive != line2Group.gameObject.activeSelf){
            line2Group.gameObject.SetActive(IsActive);
        }
    }

    void Button1SetButton(Nullable<ButtonHelper.ButtonType> newButton = null){
        if(newButton.HasValue) {
            line2Group.gameObject.SetActive(true);
            button1.SetUIComponent(newButton.Value);
        }else{
            button1.IsActive = false;
        }
    }

    void InputFieldSetActive(bool IsActive){
        if(IsActive) {
            textInput.ResetUIComponent();
        }else{
            textInput.IsActive = false;
        }
    }

    void Button2SetButton(Nullable<ButtonHelper.ButtonType> newButton = null){
        if(newButton.HasValue) {
            line2Group.gameObject.SetActive(true);
            button2.SetUIComponent(newButton.Value);
        }else{
            button2.IsActive = false;
        }
    }

    void SetHeaderUIElements(bool infoText, SpriteArrayHelper.Sprites[] infoGroup, bool inputField, ButtonHelper.ButtonType? newButton1 = null, ButtonHelper.ButtonType? newButton2 = null){
        // Line 1
        if(!infoText && infoGroup==null) {
            Line1SetActive(false);
        }else{
            Line1SetActive(true);
            InfoTextSetActive(infoText);
            InfoGroupSetArray(infoGroup);
        }

        // Line 2
        if(!newButton1.HasValue && !inputField && !newButton2.HasValue){
            Line2SetActive(false);
        }else{
            Line2SetActive(true);
            Button1SetButton(newButton1);
            InputFieldSetActive(inputField);
            Button2SetButton(newButton2);
        }
    }

    void SetFooterUIElements(bool slider){ // TODO - change
        footerGroup.gameObject.SetActive(slider); 
    }
}
