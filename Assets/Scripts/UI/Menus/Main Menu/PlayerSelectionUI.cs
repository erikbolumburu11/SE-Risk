using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Handles the logic and UI for the player selection screen. This is where
 * the amount of players, the information about them and whether they are AI or
 * not is selected.
 */
public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject newPlayerPanelPrefab;
    [SerializeField] PlayerDataManager playerDataManager;

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
            playerDataManager.players.Add(new Player(pp.name, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), pp.isAI));
        }
        SceneManager.LoadScene("Game");
    }
}
