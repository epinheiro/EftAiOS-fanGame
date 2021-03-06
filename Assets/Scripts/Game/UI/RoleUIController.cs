﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoleUIController : MonoBehaviour
{
    Dictionary<ClientController.PlayerState, RoleData> roles;

    ClientController.PlayerState currentRole;

    Image _PlayerColorIcon;
    Image _UIicon;
    Text _UIrole;
    Text _UIobjective;

    bool _popupIsVisible = false;
    public bool PopupIsVisible{
        get { return _popupIsVisible; }
    }

    void Awake(){
        this.GetComponent<Canvas>().worldCamera = Camera.main;
        this.gameObject.SetActive(false);
        
        this.transform.Find("Button").GetComponent<Button>().onClick.AddListener(OnClickClose);
        
        SetUpUIReferences();

        SetUpRoleData();
    }

    void SetUpUIReferences(){
        _PlayerColorIcon = transform.Find("PlayerColor").GetComponent<Image>();
        _UIicon = transform.Find("RoleIcon").GetComponent<Image>();
        Transform group = transform.Find("InfoTexts");
        _UIrole = group.Find("Role").GetComponent<Text>();
        _UIobjective = group.Find("Objective").GetComponent<Text>();
    }

    void SetUpRoleData(){
        roles = new Dictionary<ClientController.PlayerState, RoleData>();
        roles.Add(ClientController.PlayerState.Alien, 
            new RoleData(
                GetIcon("AlienRole"),
                "you are a ALIEN",
                "KILL all humans or DELAY their escape until oxygen runs out"
            )
        );
        roles.Add(ClientController.PlayerState.Human, 
            new RoleData(
                GetIcon("HumanRole"),
                "you are a human",
                "Escape in green escape pods before oxygen runs out"
            )
        );
    }

    void SetActive(bool isActive){
        this.gameObject.SetActive(isActive);
        _popupIsVisible = isActive;
    }

    Sprite GetIcon(string name){
        string path = "Sprites/UI/";

        string searchPath  = string.Format("{0}{1}", path, name);

        Sprite sprite = (Sprite) Resources.Load<Sprite>(searchPath);

        if(sprite==null) throw new System.Exception(string.Format("File {0} not found!", searchPath));

        return sprite;
    }

    struct RoleData{
        public readonly Sprite icon;
        public readonly string roleText;
        public readonly string objectiveText;

        public RoleData(Sprite sprite, string roleText, string objectiveText){
            this.icon = sprite;
            this.roleText = roleText;
            this.objectiveText = objectiveText;
        }
    }

    ///////////////////////////////
    /////////////////// Public API
    public void SetRole(ClientController.PlayerState role){
        currentRole = role;

        RoleData data;
        if(!roles.TryGetValue(role, out data)){
            throw new System.Exception(string.Format("Role {0} is not a valid player game role", role));
        }

        _UIicon.sprite = data.icon;
        _UIrole.text = data.roleText;
        _UIobjective.text = data.objectiveText;

    }

    public void ShowPopup(){
        SetActive(true);
    }

    public void HidePopup(){
        SetActive(false);
    }

    public void OnClickClose(){
        SetActive(false);
        //Destroy(this.gameObject);
    }

    public void SetPlayerColor(PlayerTurnData.UIColors playerColor){
        _PlayerColorIcon.color = FileAsset.GetMaterialOfSoundParticleByColorName(System.Enum.GetName(typeof(PlayerTurnData.UIColors), playerColor)).color;
    }
}
