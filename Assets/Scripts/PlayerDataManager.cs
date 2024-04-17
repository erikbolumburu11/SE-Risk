using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public List<Player> players;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        players = new List<Player>();
    }
}
