// subclass of dice class specifically for attack dice
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class AttackDice : Dice
{
    public AttackDice()
    {
        totalAttackValue = 0;
    }
    protected short shownValue;

    // combines the value of all the defense dice that ar ein the unit scene
    public static short totalAttackValue = 0;
    [SerializeField] TMP_Text attackValueText;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    public void AddToTotalAttackValue()
    {
        totalAttackValue += shownValue;
    }

    public new short Roll()
    {
        shownValue = getDiceValue();
        attackValueText.text = shownValue.ToString();
        return shownValue;
    }
}