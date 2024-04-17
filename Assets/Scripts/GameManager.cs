using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
    INITIAL_UNIT_PLACEMENT,
    BEFORE_AI_TURN,
    AI_TURN,
    AFTER_AI_TURN,
    BEFORE_PLAYER_TURN,
    PLAYER_TURN,
    AFTER_PLAYER_TURN,
    PLAYER_WON
}

public class GameManager : MonoBehaviour
{
    WorldMap map;

    public UIManager uiManager;
    public UnitMovementUI unitMovementUI;

    [SerializeField] PlayerDataManager playerDataManager;

    public bool initialUnitPlacementComplete = false;
    public bool playerTurnComplete = false;

    public List<Player> players;
    public Player currentTurnsPlayer;
    int currentTurnsPlayerIndex;

    public Territory currentlyDefendingTerritory;
    public Territory currentlyAttackingTerritory;

    State state = State.INITIAL_UNIT_PLACEMENT;

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

    public void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.INITIAL_UNIT_PLACEMENT:
                StartCoroutine(InitialUnitPlacement());
                break;

            case State.BEFORE_PLAYER_TURN:
                currentTurnsPlayer.BeforePlayerTurn(this);
                break;

            case State.PLAYER_TURN:
                StartCoroutine(PlayerTurn());
                break;

            case State.AFTER_PLAYER_TURN:
                StopCoroutine(currentTurnsPlayer.PlayerTurn(this));
                currentTurnsPlayer.AfterPlayerTurn(this);
                break;
                
            case State.PLAYER_WON:
                PlayerWon();
                break;
        }
    }

    IEnumerator InitialUnitPlacement()
    {
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

        StartCoroutine(currentTurnsPlayer.InitialUnitPlacement(this, unitsPerPlayer));

        while (!initialUnitPlacementComplete) yield return null;
        initialUnitPlacementComplete = false;
        ChangeState(State.BEFORE_PLAYER_TURN);
    }

    IEnumerator PlayerTurn()
    {

        StartCoroutine(currentTurnsPlayer.PlayerTurn(this));

        while (!playerTurnComplete) yield return null;
        playerTurnComplete = false;
        ChangeState(State.AFTER_PLAYER_TURN);
    }

    void PlayerWon()
    {
        Debug.Log("Player Won!");
    }

    public void NextPlayer()
    {
        currentTurnsPlayerIndex++;
        if(currentTurnsPlayerIndex >= players.Count)
        {
            currentTurnsPlayerIndex = 0;
        }
        currentTurnsPlayer = players[currentTurnsPlayerIndex];
    }
}
