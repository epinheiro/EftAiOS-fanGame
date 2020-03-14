using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlowTileBehavior : MonoBehaviour
{

    public ClientController controller;

    void Start()
    {
        if(controller==null) controller = GameObject.Find("Client").GetComponent<ClientController>();
    }

    void OnMouseDown(){
        controller.PlayerNextPosition = this.name;
    }
}
