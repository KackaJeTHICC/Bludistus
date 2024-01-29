using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Manages everything after player spawn
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Amount of notes needed to open the door
    /// </summary>
    private byte m_notesNeeded = 1;

    /// <summary>
    /// Is the game paused?
    /// </summary>
    private bool m_isPause = false;

    /// <summary>
    /// Player game object
    /// </summary>  
    [SerializeField]
    private GameObject m_player = null;

    /// <summary>
    /// Pause menu game object
    /// </summary>
    [SerializeField]
    private GameObject m_pauseMenu = null;
    #endregion

    #region Getters/Setters
    /// <summary>
    /// Sets the amount of nodes needed to unlock the finish line
    /// </summary>
    /// <param name="amount">Amount of nodes needed to unlock the finish line</param>
    public void NotesNeeded(string amount)
    {
        m_notesNeeded = byte.Parse(amount);
    }
    #endregion

    #region Methods
    private void Start()    //this should never occur and the .Find() will most likely fail anyway
    {
        if(m_player == null)
        {
            Debug.LogError("Player is not assigned!");
            m_player = GameObject.Find("PlayerCapsule");
        }
        if(m_pauseMenu == null)
        {
            Debug.LogError("Pause menu is not assigned!");
            m_pauseMenu = GameObject.Find("Pause menu").transform.GetChild(0).gameObject;
        }
    }

    private void Update()
    {                                                                       
        //TODO better pause menu??? idk, this might be good enough
        if (CrossPlatformInputManager.GetButtonDown("Cancel"))  //Pauses the game if it's unpaused and vice versa
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Pauses/Unpauses the game
    /// </summary>
    public void PauseGame()
    {
        if (m_isPause)
        {                            
            m_isPause = false;                       
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_player.SetActive(true);
            m_pauseMenu.SetActive(false);
        }
        else
        {
            m_isPause = true;
            m_player.SetActive(false);
            m_pauseMenu.SetActive(true);               
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    #endregion
}