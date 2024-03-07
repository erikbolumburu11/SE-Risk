using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text currentPlayerText;
    GameManager gameManager;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
    }

    void Update()
    {
        if(gameManager.currentTurnsPlayer != null)
        {
            currentPlayerText.text = "Turn: " + gameManager.currentTurnsPlayer.name;
            currentPlayerText.color = gameManager.currentTurnsPlayer.color;
        }
    }
}
