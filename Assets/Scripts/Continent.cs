using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Stores data about each continent that appears on the map.
*/
public class Continent
{
    // The territories that make up this continent

    public List<string> territoriesNames;
    // The bonus units that the player receives for owning all territories in this continent
    public int bonusUnits;

}
