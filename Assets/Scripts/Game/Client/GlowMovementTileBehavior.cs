using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowMovementTileBehavior : MonoBehaviour
{

    public ClientController controller;

    void Start()
    {
        if(controller==null) controller = GameObject.Find("Client").GetComponent<ClientController>();
    }

    void OnMouseDown(){
        TimeLogger.Log("CLIENT {0} - chose movement tile {1}", controller.ClientId, this.name);
        controller.PlayerNextPosition = this.name;
        controller.CleanMovementGlowTiles(this.name);
    }
}
