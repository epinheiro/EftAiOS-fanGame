using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class UIHelper 
{
    public delegate void BaseAction();

    protected bool _isVisible = true;
    protected readonly GameObject uiObject;
    List<GameObject> childObjects;
    List<Component> objectComponents;
    public bool IsActive{
        get { return uiObject.activeSelf; }
        set { uiObject.SetActive(value); }
    }

    public bool IsVisible{
        get { return _isVisible; }
        set {
            if( _isVisible != value){
                SetElementVisibility(value);
                _isVisible = value;
            }
        }
    }

    protected void SetElementVisibility(bool IsActive){
        Debug.Log("Hide elements");
        foreach(GameObject child in childObjects){
            child.SetActive(IsActive);
        }

        foreach(Component component in objectComponents){
            Type type = component.GetType();

            if(type == typeof(Image)){
                ((Image)component).enabled = IsActive;
            }else if(type == typeof(Button)){
                ((Button)component).enabled = IsActive;
            }else if(type == typeof(InputField)){
                ((InputField)component).enabled = IsActive;
            }else {
                throw new Exception(string.Format("Type {0} not valid", type.ToString()));
            }
        }
    }

    public UIHelper(GameObject uiObject){
        this.uiObject = uiObject;

        childObjects = new List<GameObject>();
        for( int i=0; i<uiObject.transform.childCount; i++){
            childObjects.Add(uiObject.transform.GetChild(i).gameObject);
        }

        objectComponents = new List<Component>();
        Image imageComponent = uiObject.GetComponent<Image>();
        if(imageComponent != null) objectComponents.Add(imageComponent);
        Button buttonComponent = uiObject.GetComponent<Button>();
        if(buttonComponent != null) objectComponents.Add(buttonComponent);
        InputField inputFieldComponent = uiObject.GetComponent<InputField>();
        if(inputFieldComponent != null) objectComponents.Add(inputFieldComponent);

    }

    public virtual void ResetUIComponent(){
        throw new System.Exception("Specific class do not have its own Reset");
    }
}
