// subclass of dice class specifically for defense dice
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;

public class DefenseDice : Dice
{
    protected short shownValue;

    // combines the value of all the defense dice that ar ein the unit scene
    public static short totalDefenseValue = 0;
    [SerializeField] TMP_Text defenseValueText;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // function to collect the value of the defense dice
    // and add it to the totalDefenseValue
    public void AddToTotalDefenseValue()
    {
        totalDefenseValue += shownValue;
    }
    public new int Roll()
    {
        shownValue = getDiceValue();
        defenseValueText.text = shownValue.ToString();
        return shownValue;
    }
}