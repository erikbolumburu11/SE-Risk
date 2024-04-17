using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitMovementUI : MonoBehaviour
{

    public int unitsToMove = 0;
    [SerializeField] GameManager gameManager;
    [SerializeField] TMP_Text unitAmountText;

    public void Update()
    {
        unitAmountText.text = unitsToMove.ToString();    
    }

    public void Show()
    {
        gameObject.SetActive(true);
        //unitsToMove = DICE_COUNT;
        unitsToMove = 1;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void IncrementAmount()
    {
        if(unitsToMove < gameManager.currentlyAttackingTerritory.unitCount - 1) unitsToMove++;
    }

    public void DecrementAmount()
    {
        if(unitsToMove > 1) unitsToMove--;
    }

    public void ConfirmAmount()
    {
        gameManager.unitAmountToMoveConfirmed = true;
    }
}
