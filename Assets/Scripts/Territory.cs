using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
    public List<Territory> adjacentTerritories;
    public int unitCount;
    public Player owner;
}
