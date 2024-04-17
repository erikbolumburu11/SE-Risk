using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    PLAYER_INITIAL_UNIT_PLACEMENT,
    BEFORE_PLAYER_TURN,
    PLAYER_TURN,
    AFTER_PLAYER_TURN,
    PLAYER_WON
}

public class GameManager : MonoBehaviour
{
    WorldMap map;

    UIManager uiManager;
    UnitMovementUI unitMovementUI;

    [SerializeField] PlayerDataManager playerDataManager;

    public List<Player> players;
    public Player currentTurnsPlayer;
    int currentTurnsPlayerIndex;

    public Territory currentlyDefendingTerritory;
    public Territory currentlyAttackingTerritory;

    State state = State.PLAYER_INITIAL_UNIT_PLACEMENT;

    public bool unitAmountToMoveConfirmed = false;

    void Start()
    {
        map = GetComponent<WorldMap>();
        uiManager = GetComponent<UIManager>();
        unitMovementUI = uiManager.unitMovementUI.GetComponent<UnitMovementUI>();
        playerDataManager = FindAnyObjectByType<PlayerDataManager>();

        players = playerDataManager.players;
        ChangeState(state);
    }

    void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.PLAYER_INITIAL_UNIT_PLACEMENT:
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

            currentlyAttackingTerritory = selectedTerritory; 

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

            currentlyDefendingTerritory = selectedTerritory; 

        }

        // Attack territory
        currentlyDefendingTerritory.unitCount--;

        // Claim territory 
        if (currentlyDefendingTerritory.unitCount <= 0)
        {
            currentlyDefendingTerritory.owner.ownedTerritories.Remove(currentlyDefendingTerritory);
            currentlyAttackingTerritory.owner.ownedTerritories.Add(currentlyDefendingTerritory);

            currentlyDefendingTerritory.owner = currentlyAttackingTerritory.owner;

            unitMovementUI.Show();

            while(unitAmountToMoveConfirmed == false)
            {
                yield return null;
            }

            unitMovementUI.Hide();
            unitAmountToMoveConfirmed = false;

            currentlyDefendingTerritory.unitCount = unitMovementUI.unitsToMove;
            currentlyAttackingTerritory.unitCount -= unitMovementUI.unitsToMove;

            //currentlyDefendingTerritory.unitCount = currentlyAttackingTerritory.unitCount - 1;
            //currentlyAttackingTerritory.unitCount = 1;
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
