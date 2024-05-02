using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Handles the logic and UI for the main menu UI
 */
public class MainMenuUI : MonoBehaviour
{
    /*
     * Loads the player selection scene
     */
    public void StartGame()
    {
        SceneManager.LoadScene("PlayerSelection");
    }
}
