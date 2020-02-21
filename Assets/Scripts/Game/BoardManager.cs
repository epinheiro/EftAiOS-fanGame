using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public enum PossibleTypes {EventTile, SilentTile, AlienNest, HumanDorm, EscapePod, Empty}
    public Color[] colors = {Color.gray, Color.white, Color.red, Color.blue, Color.green, Color.black};

    public readonly Vector2 oddRowSpacing = new Vector2(1.9f, 3.3f);
    public readonly Vector2 evenRowSpacing = new Vector2(0.95f, 1.65f);


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
