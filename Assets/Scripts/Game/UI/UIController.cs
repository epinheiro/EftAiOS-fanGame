using System.Collections.Generic;
using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    public enum Layout {
        /////// Client
        // ToConnect
        InsertText,
        // BeginTurnState
        AllInactive,
        // Playing state
        TwoButtons,

        /////// Server
        // SetUp
        ConditionalButton,

        /////// Generic layout
        BoardDefault,
        ClientDefault,

        /////// Currently unused
    }

    ///// Role Popup /////
    RoleUIController roleUI;

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

    ///// AlienFeedbackGroup /////
    AlienFeedbackGroupHelper alienFeedbackGroup;

    ///// Footer /////
    Transform footerGroup;
    SimpleTextHelper progressText;
    ProgressBarHelper progressBar;

    Transform clientFooterGroup;
    UnityEngine.UI.Button clientFooterHelperButton;
    Transform backButtonGroup;

    SimpleTextHelper loadingText;

    void Setup(){
        RoleUISetup();

        HeaderSetup();
        AlienFeedbackGroupSetup();
        FooterSetup();

        LoadingUISetup();

        SetPresetLayout(Layout.AllInactive);
    }

    void RoleUISetup(){
        roleUI = transform.Find("RoleUI").GetComponent<RoleUIController>();
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

    void AlienFeedbackGroupSetup(){
        alienFeedbackGroup = new AlienFeedbackGroupHelper(transform.Find("AlienFeedbackGroup").gameObject);
    }

    void FooterSetup(){
        footerGroup = transform.Find("FooterGroup");
        progressBar = new ProgressBarHelper(footerGroup.transform.Find("ProgressBar").gameObject);
        progressText = new SimpleTextHelper(footerGroup.transform.Find("ProgressText").gameObject);

        clientFooterGroup = transform.Find("ClientFooterGroup");
        clientFooterHelperButton = clientFooterGroup.Find("Help").GetComponent<UnityEngine.UI.Button>();
        backButtonGroup = transform.Find("BackToMainMenu");
    }

    void LoadingUISetup(){
        loadingText = new SimpleTextHelper(transform.Find("LoadingText").gameObject);
    }

    //////////////////


    void Awake()
    {
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        Setup();
    }

    public void SetPresetLayout(Layout layout){
        switch(layout){
            case Layout.BoardDefault:
                SetHeaderUIElements(true, false, null, null);
                SetFooterUIElements(true);
                SetClientFooterGroup(false);
                SetActiveLoadingText(false);
                break;
            case Layout.ClientDefault:
                SetHeaderUIElements(true, false, null, null);
                SetFooterUIElements(false);
                SetClientFooterGroup(false);
                SetActiveLoadingText(false);
                break;

            /////////////////////////////
            case Layout.TwoButtons:
                SetHeaderUIElements(false, false, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Attack, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                SetFooterUIElements(false);
                SetClientFooterGroup(false);
                SetActiveLoadingText(false);
                break;
            case Layout.InsertText:
                SetHeaderUIElements(false, true, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                SetFooterUIElements(false);
                SetClientFooterGroup(false);
                SetActiveLoadingText(false);
                break;
            case Layout.ConditionalButton:
                SetHeaderUIElements(true, false, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.Attack);
                SetFooterUIElements(false);
                SetClientFooterGroup(false);
                SetActiveLoadingText(false);
                break;
            case Layout.AllInactive:
                SetHeaderUIElements(false, false, null, null);
                SetFooterUIElements(false);
                SetClientFooterGroup(false);
                SetActiveLoadingText(false);
                break;
        }
    }

    // CLIENT - Role UI /////////////////////////
    public void SetPlayerRole(ClientController.PlayerState role){
        roleUI.SetRole(role);
    }
    public void ShowRolePopup(){
        roleUI.ShowPopup();
    }
    public void HideRolePopup(){
        roleUI.HidePopup();
    }

    public bool IsRolePopupVisible(){
        return roleUI.PopupIsVisible;
    }

    //////////////////////////////////////////////////

    public void SetPlayerColor(PlayerTurnData.UIColors playerColor){
        roleUI.SetPlayerColor(playerColor);
        ChangeFooterButtonColor(playerColor);
    }

    // CLIENT - ToConnect state /////////////////////////
    public void SetInsertTextLayout(string placeholderText, string insertedText, string buttonText, string infoText, UIHelper.BaseAction buttonCallback){
        SetPresetLayout(Layout.InsertText);
        this.infoText.Text = infoText;
        textInput.PlaceholderText = placeholderText;
        textInput.Text = insertedText;
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

    // CLIENT - BeginTurnState /////////////////////////
    public void SetAllInactiveLayout(){
        SetPresetLayout(Layout.AllInactive);
    }

    // CLIENT - Playing state /////////////////////////
    public void SetTwoButtonsLayout(string leftButtonText, UIHelper.BaseAction leftButtonCallback, string rightButtonText, UIHelper.BaseAction rightButtonCallback){
        SetPresetLayout(Layout.TwoButtons);
        button1.Text = leftButtonText;
        button1.InsertCallback(leftButtonCallback);
        button2.Text = rightButtonText;
        button2.InsertCallback(rightButtonCallback);
    }

    public void SetTwoButtonsVisibility(bool isVisible){
        button1.IsVisible = isVisible;
        button2.IsVisible = isVisible;
    }

    // SERVER - SetUp state /////////////////////////
    public void SetConditionalButtonLayout(string buttonText, string infoText, UIHelper.BaseAction buttonCallback){
        SetPresetLayout(Layout.ConditionalButton);
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

    /////////////////////////////////////////////////////
    // Generic methods //////////////////////////////////

    //////// HEADER
    // Line 1 //
    // InfoText
    public void SetInfoText(string newString = null){
        if(string.IsNullOrEmpty(newString)){
            InfoTextSetActive(false);
        }else{
            InfoTextSetActive(true);
            infoText.Text = newString;
        }
    }
    public void SetInfoTextColor(Color newColor){
        infoText.ChangeTextColor(newColor);
    }
    // InfoGroup
    public void UpdateSpriteArray(ServerController serverController, int numberOfPlayers){
        InitialInfoGroupSetup(serverController, numberOfPlayers);
    }

    public void SetPlayersStatus(ServerController serverController, int playersToPlay = 0, int playersPlayed = 0, int playersDied = 0, int playersEscaped = 0){
        SetInfoGroup(serverController, playersToPlay, playersPlayed, playersDied, playersEscaped);
    }

    // Line 2 //
    // Button1
    public void SetButton1(UIHelper.BaseAction callback, string buttonText = "",  Nullable<ButtonHelper.ButtonType> buttonType = null){
        SetButton(button1, callback, buttonText, buttonType);
    }
    // Button2
    public void SetButton2(UIHelper.BaseAction callback, string buttonText = "",  Nullable<ButtonHelper.ButtonType> buttonType = null){
        SetButton(button2, callback, buttonText, buttonType);
    }
    // Generic button
    void SetButton(ButtonHelper button, UIHelper.BaseAction callback, string buttonText = "",  Nullable<ButtonHelper.ButtonType> buttonType = null){
        button.InsertCallback(callback);
        button.Text = buttonText;
        if(buttonType.HasValue) button.SetButtonToPreMade(buttonType.Value);
    }
    // Input

    //////// ALIEN FEEDBACK GROUP
    public void SetAlienFeedback(List<Color> colors){
        alienFeedbackGroup.SetAlienFeedbacks(colors);
    }

    //////// FOOTER
    public void SetProgressBarValues(int currentValue, int maxValue){
        footerGroup.gameObject.SetActive(true);

        int remainingValue = (maxValue-currentValue);

        progressBar.SetValues(remainingValue, maxValue);

        progressText.Text = string.Format("oxygen level: {0}", remainingValue.ToString());
    }

    public void SetActiveClientFooterGroup(bool IsActive){
        SetClientFooterGroup(IsActive);
    }

    public void DeactiveBackButton(){
        Destroy(backButtonGroup.gameObject);
    }

    public void ActivateLoadingScreen(){
        SetPresetLayout(Layout.AllInactive);
        if(backButtonGroup != null && backButtonGroup.gameObject != null) DeactiveBackButton();
        SetActiveLoadingText(true);
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

    void InitialInfoGroup(){
        infoGroup.IsActive = false;
    }

    void InitialInfoGroupSetup(ServerController serverController, int players){
        infoGroup.IsActive = true;
        infoGroup.SetInitialSprites(serverController, players);
    }

    void SetInfoGroup(ServerController serverController, int playersToPlay = 0, int playersPlayed = 0, int playersDied = 0, int playersEscaped = 0){
        if(playersToPlay == 0  && playersPlayed == 0  && playersDied == 0  && playersEscaped == 0){
            infoGroup.IsActive = false;
        }else{
            infoGroup.IsActive = true;
            infoGroup.SetUIComponent(serverController, playersToPlay, playersPlayed, playersDied, playersEscaped);
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

    void SetHeaderUIElements(bool infoText, bool inputField, ButtonHelper.ButtonType? newButton1 = null, ButtonHelper.ButtonType? newButton2 = null){
        // Line 1
        if(!infoText && infoGroup==null) {
            Line1SetActive(false);
        }else{
            Line1SetActive(true);
            InfoTextSetActive(infoText);
            InitialInfoGroup();
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

    void SetClientFooterGroup(bool IsActive){
        clientFooterGroup.gameObject.SetActive(IsActive);
    }

    void ChangeFooterButtonColor(PlayerTurnData.UIColors playerColor){
        clientFooterHelperButton.image.color = FileAsset.GetMaterialOfSoundParticleByColorName(System.Enum.GetName(typeof(PlayerTurnData.UIColors), playerColor)).color;
    }

    void SetActiveLoadingText(bool IsActive){
        loadingText.IsActive = IsActive;
    }
}
