using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "Continent", order = 1)]

public class Continent : ScriptableObject
{
    public string name;
    public List<Territory> territories;
    public int bonusUnits;

}
