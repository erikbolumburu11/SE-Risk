using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * The current game state
 */
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

/*
 * How many dice are rolled 
 */
public enum DiceRollChoiceState
{
    UNDECIDED = -1,
    ONE = 1,
    TWO = 2,
    THREE = 3
}

/*
 * Whether a dice roll is for the purpose of attacking or defending
 */
public enum DiceRollType
{
    ATTACK,
    DEFENSE
}

/*
 * Stores data about the current game state, handles moving between the
 * different states, and stores flags that are used to communicate with other
 * parts of the application.
 */
public class GameManager : MonoBehaviour
{
    WorldMap map;

    public UIManager uiManager;
    public UnitMovementUI unitMovementUI;

    [SerializeField] PlayerDataManager playerDataManager;

    // Event Flags
    public bool initialUnitPlacementComplete = false;
    public bool playerTurnComplete = false;
    public bool diceResultsShown = false;

    public List<Player> players;
    public Player currentTurnsPlayer;
    int currentTurnsPlayerIndex;

    public Territory currentlyDefendingTerritory;
    public Territory currentlyAttackingTerritory;

    State state = State.INITIAL_UNIT_PLACEMENT;

    public DiceRollChoiceState attackerDiceRoll = DiceRollChoiceState.UNDECIDED;
    public DiceRollChoiceState defenderDiceRoll = DiceRollChoiceState.UNDECIDED;

    public bool unitAmountToMoveConfirmed = false;

    /*
     * Initializes important variables
     * Begins state machine
     */
    void Start()
    {
        map = GetComponent<WorldMap>();
        uiManager = GetComponent<UIManager>();
        unitMovementUI = uiManager.unitMovementUI.GetComponent<UnitMovementUI>();
        playerDataManager = FindAnyObjectByType<PlayerDataManager>();

        players = playerDataManager.players;
        ChangeState(state);
    }

    /*
     * Changes state to newState 
     * Executes function pertaining to newState
     */
    public void ChangeState(State newState)
    {
        state = newState;
        switch (state)
        {
            case State.INITIAL_UNIT_PLACEMENT:
                StopCoroutine(InitialUnitPlacement());
                StartCoroutine(InitialUnitPlacement());
                break;

            case State.BEFORE_PLAYER_TURN:
                StopCoroutine(BeforePlayerTurn());
                StartCoroutine(BeforePlayerTurn());
                break;

            case State.PLAYER_TURN:
                StopCoroutine(PlayerTurn());
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

    /*
     * Handles the initial claiming of territories and placing of units for each player
     */
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

    IEnumerator BeforePlayerTurn()
    {
        StartCoroutine(currentTurnsPlayer.BeforePlayerTurn(this, 3));

        while (!playerTurnComplete) yield return null;
        playerTurnComplete = false;
        ChangeState(State.PLAYER_TURN);
    }
    /*
     * Handles the player turn logic
     */
    IEnumerator PlayerTurn()
    {
        StartCoroutine(currentTurnsPlayer.PlayerTurn(this));

        while (!playerTurnComplete) yield return null;
        playerTurnComplete = false;
        ChangeState(State.AFTER_PLAYER_TURN);
    }

    /*
     * Handles the player won logic
     */
    void PlayerWon()
    {
        Debug.Log("Player Won!");
    }

    /*
     * Change to next players turn
     */
    public void NextPlayer()
    {
        currentTurnsPlayerIndex++;
        if(currentTurnsPlayerIndex >= players.Count)
        {
            currentTurnsPlayerIndex = 0;
        }
        currentTurnsPlayer = players[currentTurnsPlayerIndex];
    }

    /*
    * calculate the number of troops a player should receive at the beginning of their turn
    */
    public int CalculateTroopsToReceive()
    {
        int troops = 0;
        troops += Mathf.Max(3, currentTurnsPlayer.ownedTerritories.Count / 3);
        foreach (Continent continent in map.continents)
        {
            if (continent.territories.TrueForAll(t => t.owner == currentTurnsPlayer))
            {
                troops += continent.bonusUnits;
            }
        }
        return troops;
    }
}
