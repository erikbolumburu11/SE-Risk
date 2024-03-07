using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Territory : MonoBehaviour
{
    public List<Territory> adjacentTerritories;
    public int unitCount;
    public Player owner;
    [SerializeField] TMP_Text unitCountText;
    Image image;

    void Start()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        unitCountText.text = unitCount.ToString();
        if(owner != null) image.color = owner.color;
    }
}
