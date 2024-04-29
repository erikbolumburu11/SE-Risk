using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

/*
 * Handles the logic and UI for the player selection screen. This is where
 * the amount of players, the information about them and whether they are AI or
 * not is selected.
 */
public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject newPlayerPanelPrefab;
    [SerializeField] PlayerDataManager playerDataManager;

    List<Color> playerColours = new List<Color> { Color.red, Color.blue, Color.green, Color.yellow, Color.magenta, Color.cyan };

    /*
     * Button logic to add a new player to the game
     */
    public void AddNewPlayer()
    {
        Instantiate(newPlayerPanelPrefab, transform);
        playerDataManager.players.Clear();
    }

    /*
     * Button logic to save the configured player data in the PlayerDataManager
     * and to load the Game scene.
     */
    public void StartGame()
    {
        PlayerPanel[] playerPanels = GetComponentsInChildren<PlayerPanel>();
        if (playerPanels.Length < 3) return;
        foreach (PlayerPanel pp in playerPanels)
        {
            if (pp.name.Length == 0) return;
            int index = Random.Range(0, playerColours.Count);
            Color colour = playerColours[index];
            playerColours.RemoveAt(index);
            playerDataManager.players.Add(new Player(pp.name, colour, pp.isAI));
        }
        SceneManager.LoadScene("Game");
    }
}
