using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

/*
 * Handles the UI for the player panels in the player selection screen
 */
public class PlayerPanel : MonoBehaviour
{
    public string name;
    public bool isAI;

    private void Update()
    {
        name = GetComponentInChildren<TMP_InputField>().text;
        isAI = GetComponentInChildren<Toggle>().isOn;
    }
}
