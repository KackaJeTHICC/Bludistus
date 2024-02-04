using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

/// <summary>
/// Manages everything after player spawn
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Instance
    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    #region Variables
    /// <summary>
    /// Game difficulty settings
    /// </summary>
    private DifficultySettigns m_difficulty;

    /// <summary>
    /// Is the monster spawned
    /// </summary>
    private bool m_isMonster = false;  
    
    /// <summary>
    /// Amount of notes that were already picked up
    /// </summary>
    private byte m_notesPickedUp = 0;

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

    /// <summary>
    /// Finish line object
    /// </summary>  
    private GameObject m_finishLine = null;
    #endregion

    #region Getters/Setters
    /// <summary>
    /// DifficultySettigns setter                                                
    /// </summary>
    /// <param name="difficulty">Game difficulty enum</param>
    public void Difficulty(DifficultySettigns difficulty)
    {
         m_difficulty = difficulty;
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
        if (CrossPlatformInputManager.GetButtonDown("Cancel"))  //Pauses the game if it's unpaused and vice versa
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Logic after picking up a note
    /// </summary>
    public void NotePickedUp()
    {
        byte notesNeeded = LevelStart.instance.NotesNeeded();

        m_notesPickedUp++;

        if(m_notesPickedUp >= Mathf.FloorToInt(notesNeeded / 2) && m_difficulty.HasFlag(DifficultySettigns.spawnMonster) && !m_isMonster)    //spawns the monster mid game
        {
            m_isMonster = true;
            Instantiate(Resources.Load("Prefabs/Monster"), LevelStart.instance.RandomSpot(), Quaternion.Euler(0f, 0f, 0f));
        }

        if (m_notesPickedUp >= notesNeeded)   //opens the gate if all notes are picked up
        {
            m_finishLine = GameObject.Find("Finish Line");
            m_finishLine.GetComponentInChildren<Animator>().SetBool("All notes collected", true);
            m_finishLine.GetComponent<AudioSource>().Play();
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
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            m_pauseMenu.SetActive(true);      
            m_player.SetActive(false);
        }
    }
    #endregion
}