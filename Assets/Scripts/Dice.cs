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
    Image image;
    short shownValue;
    [SerializeField] TMP_Text valueText;
    // Start is called before the first frame update
    void Start()
    {
        image = GetComponent<Image>();
    
    }
    public int Roll()
    {
        shownValue = getDiceValue();
        valueText.text = shownValue.ToString();
        return shownValue;
    }

    void Update()
    {
        Roll();
    }
    short getDiceValue()
    {
        return (short)Random.Range(1, 7);
    }
}
