using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Dice : MonoBehaviour
{
    protected Image image;
    void Start()
    {
        image = GetComponent<Image>();
    
    }

    protected short getDiceValue()
    {
        return (short)Random.Range(1, 7);
    }
    public int Roll()
    {
        return 0;
    }
}
