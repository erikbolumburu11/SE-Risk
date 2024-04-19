using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Used to carry player data from the player selection menu scene to the game
 * scene.
 */
public class PlayerDataManager : MonoBehaviour
{
    public List<Player> players;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        players = new List<Player>();
    }
}
