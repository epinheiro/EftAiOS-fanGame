using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowSoundTileBehavior : MonoBehaviour
{

    public ClientController controller;

    void Start()
    {
        if(controller==null) controller = GameObject.Find("Client").GetComponent<ClientController>();
    }

    void OnMouseDown(){
        Debug.Log(string.Format("CLIENT {0} - clicked tile {1}", controller.ClientId, this.name));
        controller.PlayerNextSound = this.name;
        controller.BoardManagerRef.CleanMovementGlowTiles(this.name);
    }
}