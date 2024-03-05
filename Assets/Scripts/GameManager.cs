using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

enum State
{
    INITIAL_UNIT_PLACEMENT,
    PLAYER_TURN
}

public class GameManager : MonoBehaviour
{
    public WorldMap map;

    public List<Player> players;
    Player currentTurnsPlayer;

    State state = State.INITIAL_UNIT_PLACEMENT;

    void Start()
    {
        players = new List<Player>
        {
            new Player(),
            new Player(),
            new Player()
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

            case State.PLAYER_TURN:
                PlayerTurn();
                break;
        }
    }

    IEnumerator InitialUnitPlacement()
    {
        // Roll Dice To Pick Who Picks First
        int playerIndex = Random.Range(0, players.Count);
        Player currentPlayer = players[playerIndex];

        int unitsPerPlayer = 35;
        switch (players.Count)
        {
            case 3:
                unitsPerPlayer = 35;
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
            selectedTerritory.owner = currentPlayer;
            selectedTerritory.unitCount = 1;

            playerIndex = playerIndex + 1;
            if(playerIndex >= players.Count)
            {
                playerIndex = 0;
            }
            currentPlayer = players[playerIndex];
        }

        // Distribute Army 
        for (int i = 0; i < unitsPerPlayer * players.Count; i++)
        {
            // Wait for input, if player clicks a territory it owns add 1 unit

            playerIndex = playerIndex + 1;
            if(playerIndex >= players.Count)
            {
                playerIndex = 0;
            }
            currentPlayer = players[playerIndex];
        }
    }

    void PlayerTurn()
    {
    }
}
