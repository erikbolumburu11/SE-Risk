using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/*
 * Handles the UI and game logic for rolling dice.
 */
public class DiceRollUI : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    public TMP_Text titleText;
    public DiceRollType diceRollType;

    private void Update()
    {
        if(diceRollType == DiceRollType.ATTACK)
        {
            titleText.text = gameManager.currentlyAttackingTerritory.owner.name + " Attack With X Dice";
        }
        if(diceRollType == DiceRollType.DEFENSE)
        {
            titleText.text = gameManager.currentlyDefendingTerritory.owner.name + " Defend With X Dice";
        }
    }

    /*
     * Shows the Dice Roll UI
     */
    public void Show()
    {
        gameObject.SetActive(true);
    }

    /*
     * Hides the Dice Roll UI
     */
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /*
     * Button functionality to roll one dice
     */
    public void RollOneButton()
    {
        if(diceRollType == DiceRollType.ATTACK)
        {
            gameManager.attackerDiceRoll = DiceRollChoiceState.ONE;
        }
        if(diceRollType == DiceRollType.DEFENSE)
        {
            gameManager.defenderDiceRoll = DiceRollChoiceState.ONE;
        }
    }

    /*
     * Button functionality to roll two dice
     */
    public void RollTwoButton()
    {
        if(gameManager.currentlyAttackingTerritory.unitCount > 2)
        {
            if(diceRollType == DiceRollType.ATTACK)
            {
                gameManager.attackerDiceRoll = DiceRollChoiceState.TWO;
            }
        }
        if(gameManager.currentlyDefendingTerritory.unitCount > 1)
        {
            if(diceRollType == DiceRollType.DEFENSE)
            {
                gameManager.defenderDiceRoll = DiceRollChoiceState.TWO;
            }
        }
    }

    /*
     * Button functionality to roll Three dice
     */
    public void RollThreeButton()
    {
        if(gameManager.currentlyAttackingTerritory.unitCount > 3)
        {
            if(diceRollType == DiceRollType.ATTACK)
            {
                gameManager.attackerDiceRoll = DiceRollChoiceState.THREE;
            }
        }
    }
}
