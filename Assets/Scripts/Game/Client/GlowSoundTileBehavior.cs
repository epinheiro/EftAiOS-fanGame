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
        TimeLogger.Log("CLIENT {0} - chose sound tile {1}", controller.ClientId, this.name);
        controller.PlayerNextSound = this.name;
        controller.BoardManagerRef.CleanSoundGlowTiles(this.name);
    }
}