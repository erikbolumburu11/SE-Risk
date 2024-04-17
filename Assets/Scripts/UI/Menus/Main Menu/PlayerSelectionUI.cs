using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSelectionUI : MonoBehaviour
{
    [SerializeField] GameObject newPlayerPanelPrefab;
    [SerializeField] PlayerDataManager playerDataManager;

    public void AddNewPlayer()
    {
        Instantiate(newPlayerPanelPrefab, transform);
        playerDataManager.players.Clear();
    }

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
