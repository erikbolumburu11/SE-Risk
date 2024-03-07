using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Territory> ownedTerritories;
    public string name;
    public Color color;

    public Player(string playerName, Color playerColor)
    {
        ownedTerritories = new List<Territory>();
        name = playerName;
        color = playerColor;
    }
}
