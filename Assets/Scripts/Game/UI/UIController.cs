using UnityEngine;
using System;

public class UIController : MonoBehaviour
{
    public enum Layout {Default, TwoButtons, InsertText, OnlyText}

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
    void Start()
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
                SetUpUIElements(true, true, true, null, (ButtonHelper.ButtonType) ButtonHelper.ButtonType.DontAttack);
                break;
            case Layout.OnlyText:
                SetUpUIElements(false, false, true, null, null);
                break;
        }
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
