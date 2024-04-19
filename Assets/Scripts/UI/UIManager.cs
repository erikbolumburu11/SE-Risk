using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Stores references to UI elements.
 * Also handles some simple UI elements in Update().
 */
public class UIManager : MonoBehaviour
{
    [SerializeField] TMP_Text currentPlayerText;

    public GameObject unitMovementUI;
    public DiceRollUI diceRollUI;
    public DiceResultsUI diceRollResultsUI;

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
