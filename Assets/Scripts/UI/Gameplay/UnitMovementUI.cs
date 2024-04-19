using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Handles the logic and UI for selecting the amount of units to move.
 */
public class UnitMovementUI : MonoBehaviour
{

    public int unitsToMove = 0;
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Text unitAmountText;

    public void Update()
    {
        unitAmountText.text = unitsToMove.ToString();    
    }

    /*
     * Shows the Unit Movement UI
     */
    public void Show()
    {
        gameObject.SetActive(true);
        //unitsToMove = DICE_COUNT;
        unitsToMove = 1;
    }

    /*
     * Hides the Unit Movement UI
     */
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /*
     * Button logic to increase how many units the player wants to move
     */
    public void IncrementAmount()
    {
        if(unitsToMove < gameManager.currentlyAttackingTerritory.unitCount - 1) unitsToMove++;
    }

    /*
     * Button logic to decrease how many units the player wants to move
     */
    public void DecrementAmount()
    {
        if(unitsToMove > 1) unitsToMove--;
    }

    /*
     * Button logic to confirm how many units the player wants to move
     */
    public void ConfirmAmount()
    {
        gameManager.unitAmountToMoveConfirmed = true;
    }
}
