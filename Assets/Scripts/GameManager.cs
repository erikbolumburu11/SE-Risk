using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

enum State
{
    INITIAL_UNIT_PLACEMENT,
    BEFORE_PLAYER_TURN,
    PLAYER_TURN,
    AFTER_PLAYER_TURN,
    PLAYER_WON
}

public class GameManager : MonoBehaviour
{
    WorldMap map;
    UIManager uiManager;

    public List<Player> players;
    public Player currentTurnsPlayer;
    int currentTurnsPlayerIndex;

    State state = State.INITIAL_UNIT_PLACEMENT;

    void Start()
    {
        map = GetComponent<WorldMap>();
        uiManager = GetComponent<UIManager>();

        players = new List<Player>
        {
            new Player("Player 1", Color.red),
            new Player("Player 2", Color.cyan),
            new Player("Player 3", Color.green)
        };

        ChangeState(state);
    }

    void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.INITIAL_UNIT_PLACEMENT:
                StartCoroutine(InitialUnitPlacement());
                break;

            case State.BEFORE_PLAYER_TURN:
                BeforePlayerTurn();
                break;

            case State.PLAYER_TURN:
                StartCoroutine(PlayerTurn());
                break;

            case State.AFTER_PLAYER_TURN:
                StopCoroutine(PlayerTurn());
                AfterPlayerTurn();
                break;
                
            case State.PLAYER_WON:
                PlayerWon();
                break;
        }
    }

    IEnumerator InitialUnitPlacement()
    {
        // Roll Dice To Pick Who Picks First
        currentTurnsPlayerIndex = Random.Range(0, players.Count);
        currentTurnsPlayer = players[currentTurnsPlayerIndex];

        int unitsPerPlayer = 35;
        switch (players.Count)
        {
            case 3:
                // unitsPerPlayer = 35; // Commented for testing
                unitsPerPlayer = 5;
                break;

            case 4:
                unitsPerPlayer = 30;
                break;

            case 5:
                unitsPerPlayer = 25;
                break;

            case 6:
                unitsPerPlayer = 20;
                break;
        }

        // Remove one unit to account for territory claiming
        unitsPerPlayer--;

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
            selectedTerritory.owner = currentTurnsPlayer;
            selectedTerritory.unitCount = 1;
            currentTurnsPlayer.ownedTerritories.Add(selectedTerritory);

            NextPlayer();
        }

        // Distribute Army 
        for (int i = 0; i < unitsPerPlayer * players.Count; i++)
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
                        if (hitTerritory.owner == currentTurnsPlayer) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }

            selectedTerritory.unitCount++;

            NextPlayer();
        }

        ChangeState(State.PLAYER_TURN);
    }

    void BeforePlayerTurn()
    {
        ChangeState(State.PLAYER_TURN);
    }

    IEnumerator PlayerTurn()
    {
        bool hasPossibleMove = true;

        bool hasTerritoryToAttackWith = false;
        foreach (Territory t in currentTurnsPlayer.ownedTerritories)
        {
            if (t.unitCount > 1) hasTerritoryToAttackWith = true;
        }
        if (!hasTerritoryToAttackWith) hasPossibleMove = false;

        if (!hasPossibleMove) ChangeState(State.AFTER_PLAYER_TURN);

        Territory attackingTerritory;
        Territory defendingTerritory;
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
                        if (hitTerritory.owner == currentTurnsPlayer && hitTerritory.unitCount > 1) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }

            attackingTerritory = selectedTerritory; 

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
                        if (hitTerritory.owner != currentTurnsPlayer) selectedTerritory = hitTerritory;
                    }
                }
                yield return null;
            }

            defendingTerritory = selectedTerritory; 

        }

        // Attack territory
        defendingTerritory.unitCount--;

        // Claim territory 
        if (defendingTerritory.unitCount <= 0)
        {
            defendingTerritory.owner.ownedTerritories.Remove(defendingTerritory);
            attackingTerritory.owner.ownedTerritories.Add(defendingTerritory);

            defendingTerritory.owner = attackingTerritory.owner;

            defendingTerritory.unitCount = attackingTerritory.unitCount - 1;
            attackingTerritory.unitCount = 1;
        }

        ChangeState(State.AFTER_PLAYER_TURN);

        yield return null;
    }

    void AfterPlayerTurn()
    {
        // Remove defeated players
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ownedTerritories.Count == 0)
            {
                players.RemoveAt(i);
            }
        }

        // Check win condition and change player
        if(players.Count == 1)
        {
            ChangeState(State.PLAYER_WON);
        }

        NextPlayer();
        ChangeState(State.BEFORE_PLAYER_TURN);
    }

    void PlayerWon()
    {
        Debug.Log("Player Won!");
    }

    void NextPlayer()
    {
        currentTurnsPlayerIndex++;
        if(currentTurnsPlayerIndex >= players.Count)
        {
            currentTurnsPlayerIndex = 0;
        }
        currentTurnsPlayer = players[currentTurnsPlayerIndex];
    }
}
