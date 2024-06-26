using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/*
 * Stores data about each player.
 * Handles player related state logic like their turn.
 */
public class Player
{
    public List<Territory> ownedTerritories;
    public string name;
    public Color color;
    public bool isAI;

    public Player(string playerName, Color playerColor, bool playerIsAI)
    {
        ownedTerritories = new List<Territory>();
        name = playerName;
        color = playerColor;
        isAI = playerIsAI;
    }

    /*
     * Handles the logic for a player claiming territories and assigning units to
     * claimed territories
     */
    public IEnumerator InitialUnitPlacement(GameManager gm, int unitsPerPlayer)
    {
        // Roll Dice To Pick Who Picks First

        // Choose Territories
        // TODO: Change to iterate 42 times when map is expanded to full size
        for (int i = 0; i < gm.map.territories.Count; i++)
        {
            Territory selectedTerritory = null;

            // PLAYER OPTION
            if (!gm.currentTurnsPlayer.isAI)
            {
                // Select Territory With Mouse
                while (selectedTerritory == null)
                {
                    // If LMB pressed
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (hit.collider != null)
                        {
                            Territory hitTerritory = hit.collider.GetComponent<Territory>();
                            if (hitTerritory.owner == null) selectedTerritory = hitTerritory;
                        }
                    }
                    yield return null;
                }
            }
            // AI LOGIC
            else
            {
                while(selectedTerritory == null)
                {
                    Territory t = gm.map.territories[Random.Range(0, gm.map.territories.Count)];
                    if(t.owner == null)
                    {
                        selectedTerritory = t;
                    }
                }
            }

            // Claim Territory
            selectedTerritory.owner = gm.currentTurnsPlayer;
            selectedTerritory.unitCount = 1;
            gm.currentTurnsPlayer.ownedTerritories.Add(selectedTerritory);

            gm.NextPlayer();

            yield return null;
        }

        // Distribute Army 
        for (int i = 0; i < unitsPerPlayer * gm.players.Count; i++)
        {
            // Wait for input, if player clicks a territory it owns add 1 unit
            Territory selectedTerritory = null;
            // PLAYER TURN
            if (!gm.currentTurnsPlayer.isAI)
            {
                // Select Territory With Mouse
                while (selectedTerritory == null)
                {
                    // If LMB pressed
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (hit.collider != null)
                        {
                            Territory hitTerritory = hit.collider.GetComponent<Territory>();
                            if (hitTerritory.owner == gm.currentTurnsPlayer) selectedTerritory = hitTerritory;
                        }
                    }
                    yield return null;
                }
            }
            // AI LOGIC
            else
            {
                selectedTerritory = gm.currentTurnsPlayer.ownedTerritories[Random.Range(0, gm.currentTurnsPlayer.ownedTerritories.Count)];
            }

            selectedTerritory.unitCount++;

            gm.NextPlayer();
        }

        gm.initialUnitPlacementComplete = true;
    }

    /*
     * Handles logic for things that happen before the players turn like
     * trading in cards
     */
    public IEnumerator BeforePlayerTurn(GameManager gm, int unitsToPlace)
    {
        while(unitsToPlace > 0){
            // Wait for input, if player clicks a territory it owns add 1 unit
            Territory selectedTerritory = null;
            // PLAYER OPTION
            if (!gm.currentTurnsPlayer.isAI)
            {
                // Select Territory With Mouse
                while (selectedTerritory == null)
                {
                    // If LMB pressed
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (hit.collider != null)
                        {
                            if(hit.collider.GetComponent<Territory>().owner == gm.currentTurnsPlayer){
                                selectedTerritory = hit.collider.GetComponent<Territory>();
                            };
                        }
                    }
                    yield return null;
                }
            }
            else
            {
                selectedTerritory = gm.currentTurnsPlayer.ownedTerritories[Random.Range(0, gm.currentTurnsPlayer.ownedTerritories.Count)];
            }
            selectedTerritory.unitCount++;
            unitsToPlace--;
        }
        gm.ChangeState(State.PLAYER_TURN);
    }

    /*
     * Handles the logic for the players turn
     */
    public IEnumerator PlayerTurn(GameManager gm)
    {
        bool hasPossibleMove = true;

        bool hasTerritoryToAttackWith = false;
        foreach (Territory t in gm.currentTurnsPlayer.ownedTerritories)
        {
            if (t.unitCount > 1) hasTerritoryToAttackWith = true;
        }
        if (!hasTerritoryToAttackWith) hasPossibleMove = false;

        if (!hasPossibleMove) gm.ChangeState(State.AFTER_PLAYER_TURN);

        // If AI Turn, stores which territory to attack
        Territory territoryToAttack = null;

        // Pick what territory to attack with
        {
            Territory selectedTerritory = null;
            // PLAYER TURN
            if (!gm.currentTurnsPlayer.isAI)
            {
                // Select Territory With Mouse
                while (selectedTerritory == null)
                {
                    // If LMB pressed
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (hit.collider != null)
                        {
                            Territory hitTerritory = hit.collider.GetComponent<Territory>();
                            // Check if player has enough units to attack
                            if (hitTerritory.owner == gm.currentTurnsPlayer && hitTerritory.unitCount > 1)
                            {
                                bool adjacentEnemies = false;
                                foreach (Territory territory in hitTerritory.adjacentTerritories)
                                {
                                    if (territory.owner != gm.currentTurnsPlayer) adjacentEnemies = true;
                                }
                                if (adjacentEnemies) selectedTerritory = hitTerritory;
                            }
                        }
                    }
                    yield return null;
                }
            }
            else
            {
                while(selectedTerritory == null)
                {
                    Territory t = gm.currentTurnsPlayer.ownedTerritories[Random.Range(0, gm.currentTurnsPlayer.ownedTerritories.Count)];
                    if(t.unitCount > 1)
                    {
                        foreach (Territory at in t.adjacentTerritories)
                        {
                            if(at.owner != gm.currentTurnsPlayer)
                            {
                                territoryToAttack = at;
                            }
                        }
                        if(territoryToAttack != null)
                        {
                            selectedTerritory = t;
                        }
                    }
                }
            }

            gm.currentlyAttackingTerritory = selectedTerritory;
        }
        {
            // Pick who to attack
            Territory selectedTerritory = null;
            // PLAYER TURN
            if (!gm.currentTurnsPlayer.isAI)
            {
                // Select Territory With Mouse
                while (selectedTerritory == null)
                {
                    // If LMB pressed
                    if (Input.GetMouseButtonDown(0))
                    {
                        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                        if (hit.collider != null)
                        {
                            Territory hitTerritory = hit.collider.GetComponent<Territory>();
                            if (hitTerritory.owner != gm.currentTurnsPlayer)
                            {
                                if (hitTerritory.adjacentTerritories.Contains(gm.currentlyAttackingTerritory))
                                {
                                    selectedTerritory = hitTerritory;
                                }
                            }
                        }
                    }
                    yield return null;
                }
            }
            else
            {
                selectedTerritory = territoryToAttack;
                gm.currentlyDefendingTerritory = selectedTerritory;
            }

            gm.currentlyDefendingTerritory = selectedTerritory;

        }

        gm.attackerDiceRoll = DiceRollChoiceState.UNDECIDED;
        gm.defenderDiceRoll = DiceRollChoiceState.UNDECIDED;

        // ATTACK TERRITORY
        if (!gm.currentTurnsPlayer.isAI)
        {
            // PLAYER INPUT ON HOW MANY DICE TO ROLL

            gm.uiManager.diceRollUI.diceRollType = DiceRollType.ATTACK;

            gm.uiManager.diceRollUI.Show();

            // Wait until attacker decides how many dice to attack with
            while (gm.attackerDiceRoll == DiceRollChoiceState.UNDECIDED)
            {
                yield return null;
            }
            gm.uiManager.diceRollUI.Hide();

            if (gm.currentlyDefendingTerritory.owner.isAI)
            {
                gm.uiManager.diceRollUI.diceRollType = DiceRollType.DEFENSE;
                if(gm.currentlyDefendingTerritory.unitCount == 1)
                {
                    gm.defenderDiceRoll = DiceRollChoiceState.ONE;
                }
                else
                {
                    gm.defenderDiceRoll = DiceRollChoiceState.TWO;
                }
            }
            else
            {

                gm.uiManager.diceRollUI.diceRollType = DiceRollType.DEFENSE;
                
                gm.uiManager.diceRollUI.Show();

                // Wait until defender decides how many dice to defend with
                while (gm.defenderDiceRoll == DiceRollChoiceState.UNDECIDED)
                {
                    yield return null;
                }

                gm.uiManager.diceRollUI.Hide();
            }
        }
        else
        {
            if(gm.currentlyAttackingTerritory.unitCount == 2)
            {
                gm.attackerDiceRoll = DiceRollChoiceState.ONE;
            }
            else if(gm.currentlyAttackingTerritory.unitCount == 3)
            {
                gm.attackerDiceRoll = DiceRollChoiceState.TWO;
            }
            else
            {
                gm.attackerDiceRoll = DiceRollChoiceState.THREE;
            }

            gm.currentlyDefendingTerritory = territoryToAttack;
            if (gm.currentlyDefendingTerritory.owner.isAI)
            {
                if(gm.currentlyDefendingTerritory.unitCount == 1)
                {
                    gm.defenderDiceRoll = DiceRollChoiceState.ONE;
                }
                else
                {
                    gm.defenderDiceRoll = DiceRollChoiceState.TWO;
                }
            }
            else
            {
                gm.uiManager.diceRollUI.Show();

                // Wait until defender decides how many dice to defend with
                while (gm.defenderDiceRoll == DiceRollChoiceState.UNDECIDED)
                {
                    yield return null;
                }

                gm.uiManager.diceRollUI.Hide();
            }
        }

        // ROLL DICE & CALCULATE RESULTS
        List<int> attackerRoll = new List<int>(); // New list of attackers dice

        // Generate results for attacker dice roll
        for (int i = 0; i < (int)gm.attackerDiceRoll; i++)
        {
            attackerRoll.Add(Random.Range(1, 7));
        }

        List<int> defenderRoll = new List<int>(); // New list of attackers dice
       // Generate results for defender dice roll
        for (int i = 0; i < (int)gm.defenderDiceRoll; i++)
        {
            defenderRoll.Add(Random.Range(1, 7));
        }

        // SHOW RESULTS
        if(!gm.currentTurnsPlayer.isAI)
        {
            gm.uiManager.diceRollResultsUI.Show(attackerRoll, defenderRoll);

            while(gm.diceResultsShown == false)
            {
                yield return null;
            }

            gm.diceResultsShown = false;

            gm.uiManager.diceRollResultsUI.Hide();
        }

        /*
         * Runs once for every dice a defender rolled.
         * 
         * Compare the highest roll from either side.
         * 
         * If the attackers highest roll is higher than
         * the defenders highest roll, remove one unit from
         * the currently defending territory.
         * In the other case, remove one unit from the currently
         * attacking territory.
         */
        for (int i = 0; i < (int)gm.defenderDiceRoll; i++)
        {
            if (attackerRoll.Count == 0 || defenderRoll.Count == 0) break;
            int attackerMax = attackerRoll.Max();
            int defenderMax = defenderRoll.Max();

            if (attackerMax > defenderMax)
            {
                gm.currentlyDefendingTerritory.unitCount--;

                attackerRoll = Utils.RemoveANumberFromList(attackerRoll, attackerMax);
                defenderRoll = Utils.RemoveANumberFromList(defenderRoll, defenderMax);
            }
            else
            {
                gm.currentlyAttackingTerritory.unitCount--;

                attackerRoll = Utils.RemoveANumberFromList(attackerRoll, attackerMax);
                defenderRoll = Utils.RemoveANumberFromList(defenderRoll, defenderMax);
            }
        }

        // Claim territory 
        if (gm.currentlyDefendingTerritory.unitCount <= 0)
        {
            gm.currentlyDefendingTerritory.owner.ownedTerritories.Remove(gm.currentlyDefendingTerritory);
            gm.currentlyAttackingTerritory.owner.ownedTerritories.Add(gm.currentlyDefendingTerritory);

            gm.currentlyDefendingTerritory.owner = gm.currentlyAttackingTerritory.owner;

            if (!gm.currentTurnsPlayer.isAI)
            {
                gm.unitMovementUI.Show();

                while (gm.unitAmountToMoveConfirmed == false)
                {
                    yield return null;
                }

                gm.unitMovementUI.Hide();
                gm.unitAmountToMoveConfirmed = false;

                gm.currentlyDefendingTerritory.unitCount = gm.unitMovementUI.unitsToMove;
                gm.currentlyAttackingTerritory.unitCount -= gm.unitMovementUI.unitsToMove;
            }
            else
            {
                gm.currentlyDefendingTerritory.unitCount = gm.currentlyAttackingTerritory.unitCount - 1;
                gm.currentlyAttackingTerritory.unitCount = 1;
            }
        }

        gm.attackerDiceRoll = DiceRollChoiceState.UNDECIDED;
        gm.defenderDiceRoll = DiceRollChoiceState.UNDECIDED;

        gm.playerTurnComplete = true;

        yield return new WaitForSeconds(0.5f);

        yield return null;
    }

    /*
     * Handles the logic for things that happen after the players turn
     */
    public void AfterPlayerTurn(GameManager gm)
    {
        // Remove defeated players
        for (int i = 0; i < gm.players.Count; i++)
        {
            if (gm.players[i].ownedTerritories.Count == 0)
            {
                gm.players.RemoveAt(i);
            }
        }

        // Check win condition and change player
        if (gm.players.Count == 1)
        {
            gm.ChangeState(State.PLAYER_WON);
        }

        gm.NextPlayer();
        gm.ChangeState(State.BEFORE_PLAYER_TURN);
    }
}

