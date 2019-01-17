using UnityEngine;
using System.Collections;

public class FightTile : FightElement {

	// Use this for initialization


    public override void SetName()
    {
        name = "Tile" + XPosition.ToString() + "," + YPosition.ToString();
    }
}
