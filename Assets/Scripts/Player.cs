using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public string PlayerName = "Player";
    public int playerNumber;
    public bool AIplayer = false;
    public int aiPlayerLevel = 0;
    public int playerCash = 1000;
    public int playerInfluence = 0;
    public Color PlayerColor;
 

    public List<int> tileCounts = new List<int>
    {
        0,0,0,0,0,0 //Count of each builing type: [0] empty land, [1] Residential, [2] Industrial, [3] PowerSupply, [4] Civic, [5] landmark
    };

}
