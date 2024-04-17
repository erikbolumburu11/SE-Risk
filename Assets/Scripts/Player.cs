using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public IEnumerator InitialUnitPlacement(GameManager gm, int unitsPerPlayer)
    {
        // Roll Dice To Pick Who Picks First

        // Choose Territories
        // TODO: Change to iterate 42 times when map is expanded to full size
        for (int i = 0; i < 3; i++)
        {
            Territory selectedTerritory = null;
            // Select Territory With Mouse
            while(selectedTerritory == null)
            {
                // If LMB pressed
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if(hit.collider != null)
                    {
                        Territory hitTerritory = hit.collider.GetComponent<Territory>();
                        if (hitTerritory.owner == null) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }
            
            // Claim Territory
            selectedTerritory.owner = gm.currentTurnsPlayer;
            selectedTerritory.unitCount = 1;
            gm.currentTurnsPlayer.ownedTerritories.Add(selectedTerritory);

            gm.NextPlayer();

        }

        // Distribute Army 
        for (int i = 0; i < unitsPerPlayer * gm.players.Count; i++)
        {
            // Wait for input, if player clicks a territory it owns add 1 unit
            Territory selectedTerritory = null;
            // Select Territory With Mouse
            while(selectedTerritory == null)
            {
                // If LMB pressed
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if(hit.collider != null)
                    {
                        Territory hitTerritory = hit.collider.GetComponent<Territory>();
                        if (hitTerritory.owner == gm.currentTurnsPlayer) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }

            selectedTerritory.unitCount++;

            gm.NextPlayer();
        }

        gm.initialUnitPlacementComplete = true;
    }

    public void BeforePlayerTurn(GameManager gm)
    {
        gm.ChangeState(State.PLAYER_TURN);
    }

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

        // Pick what territory to attack with
        {
            Territory selectedTerritory = null;
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
                        if (hitTerritory.owner == gm.currentTurnsPlayer && hitTerritory.unitCount > 1) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }

            gm.currentlyAttackingTerritory = selectedTerritory; 

        }
        {
            // Pick who to attack
            Territory selectedTerritory = null;
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
                        if (hitTerritory.owner != gm.currentTurnsPlayer) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }

            gm.currentlyDefendingTerritory = selectedTerritory; 

        }

        // Attack territory
        gm.currentlyDefendingTerritory.unitCount--;

        // Claim territory 
        if (gm.currentlyDefendingTerritory.unitCount <= 0)
        {
            gm.currentlyDefendingTerritory.owner.ownedTerritories.Remove(gm.currentlyDefendingTerritory);
            gm.currentlyAttackingTerritory.owner.ownedTerritories.Add(gm.currentlyDefendingTerritory);

            gm.currentlyDefendingTerritory.owner = gm.currentlyAttackingTerritory.owner;

            gm.unitMovementUI.Show();

            while(gm.unitAmountToMoveConfirmed == false)
            {
                yield return null;
            }

            gm.unitMovementUI.Hide();
            gm.unitAmountToMoveConfirmed = false;

            gm.currentlyDefendingTerritory.unitCount = gm.unitMovementUI.unitsToMove;
            gm.currentlyAttackingTerritory.unitCount -= gm.unitMovementUI.unitsToMove;

            //currentlyDefendingTerritory.unitCount = currentlyAttackingTerritory.unitCount - 1;
            //currentlyAttackingTerritory.unitCount = 1;
        }

        gm.playerTurnComplete = true;
    }

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
        if(gm.players.Count == 1)
        {
            gm.ChangeState(State.PLAYER_WON);
        }

        gm.NextPlayer();
        gm.ChangeState(State.BEFORE_PLAYER_TURN);
    }
}
