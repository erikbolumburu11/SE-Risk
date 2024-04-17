using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player
{
    public List<Territory> ownedTerritories;
    public string name;
    public Color color;
    public bool isAI;

    public Player(string playerName, Color playerColor, bool playerIsAI)
    {
        ownedTerritories = new List<Territory>();
        name = playerName;
        color = playerColor;
        isAI = playerIsAI;
    }
}
