using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*
 * Handles the logic for displaying the results of dice rolls from the Dice
 * Roll UI
 */
public class DiceResultsUI : MonoBehaviour
{
    [SerializeField] TMP_Text attackerNameText;
    [SerializeField] TMP_Text defenderNameText;

    [SerializeField] GameManager gameManager;

    [SerializeField] Transform attackerGridTransform;
    [SerializeField] Transform defenderGridTransform;

    [SerializeField] GameObject dicePrefab;

    public List<GameObject> diceObjects;

    private void Update()
    {
        attackerNameText.text = gameManager.currentlyAttackingTerritory.owner.name;
        defenderNameText.text = gameManager.currentlyDefendingTerritory.owner.name;
    }

    /*
     * Displays The Results Of The Dice Rolls Using The Two Parameratized Lists
     */
    public void Show(List<int> attackerRoll, List<int> defenderRoll)
    {
        foreach (GameObject go in diceObjects)
        {
            Destroy(go);
        }

        diceObjects = new List<GameObject>();

        foreach (int i in attackerRoll)
        {
            GameObject dice = Instantiate(dicePrefab, attackerGridTransform);
            dice.GetComponentInChildren<TMP_Text>().text = i.ToString();
            diceObjects.Add(dice);
        }
        foreach (int i in defenderRoll)
        {
            GameObject dice = Instantiate(dicePrefab, defenderGridTransform);
            dice.GetComponentInChildren<TMP_Text>().text = i.ToString();
            diceObjects.Add(dice);
        }

        gameObject.SetActive(true);
    }

    /*
     * Hides the Dice Results UI
     */
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    /*
     * Functionality of the continue button. Sets the diceResultsShown flag to true
     * so that the player turn logic knows to continue.
     */
    public void ContinueButton()
    {
        gameManager.diceResultsShown = true;
    }
}
