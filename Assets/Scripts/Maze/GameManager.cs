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
    /// Is the game paused?
    /// </summary>
    private bool m_isPause = false;

    /// <summary>
    /// Does player have flare?
    /// </summary>
    private bool m_hasFlare = false;
                                            
    /// <summary>
    /// Amount of notes that were already picked up
    /// </summary>
    private byte m_notesPickedUp = 0;

    /// <summary>
    /// Speed of flare shaking in hand
    /// </summary>
    private float m_shakeSpeed = 10f;

    /// <summary>
    /// Amplitude of flare shaking in hand
    /// </summary>
    private float m_shakeAmplitude = 0.1f;

    /// <summary>
    /// Initial position of flare in hand
    /// </summary>
    private Vector3 m_initialPosition;

    /// <summary>
    /// Flare game object
    /// </summary>
    [SerializeField]
    private GameObject m_flare = null;

    /// <summary>
    /// Pause menu game object
    /// </summary>
    [SerializeField]
    private GameObject m_pauseMenu = null;

    /// <summary>
    /// Finish line object
    /// </summary>  
    private GameObject m_finishLine = null;

    /// <summary>
    /// Player game object
    /// </summary>  
    [SerializeField]
    private GameObject m_player = null;

    /// <summary>
    /// Flare camera
    /// </summary>
    [SerializeField]
    private Camera m_flareCamera = null;

    /// <summary>
    /// Player camera
    /// </summary>
    [SerializeField]
    private Camera m_playerCamera = null;
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
        if (m_player == null)
        {
            Debug.LogError("Player is not assigned!");
            m_player = GameObject.Find("PlayerCapsule");
        }
        if (m_pauseMenu == null)
        {
            Debug.LogError("Pause menu is not assigned!");
            m_pauseMenu = GameObject.Find("Pause menu").transform.GetChild(0).gameObject;
        }
        if (m_playerCamera == null)
        {
            Debug.LogError("Player camera is not assigned!");
            m_playerCamera = m_player.transform.GetChild(0).GetComponent<Camera>();
        }
        if (m_flareCamera == null)
        {
            Debug.LogError("Flare camera is not assigned!");
            m_flareCamera = m_player.transform.GetChild(1).GetComponent<Camera>();
        }
        if (m_flare == null)
        {
            Debug.LogError("Flare  is not assigned!");
            m_flare = m_player.transform.GetChild(2).gameObject;
        }

        m_initialPosition = m_flare.transform.position;
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Cancel"))  //Pauses the game if it's unpaused and vice versa
        {
            PauseGame();
        }
        if (CrossPlatformInputManager.GetButtonDown("Fire1") && m_hasFlare)  //Shoots the flare
        {
            StartCoroutine(FlareUse());
        }

        if (m_hasFlare)
        {
            ShakeAnimation();
        }
    }

    #region Flare
    /// <summary>
    /// Plays the zooming out animation
    /// </summary>
    private IEnumerator FlareUse()
    {
        m_hasFlare = false;

        float width = MazeRenderer.instance.GetMazeSize().x;
        float height = MazeRenderer.instance.GetMazeSize().y;
        float aspectRatio = Screen.width / (float)Screen.height;
        float targetSize;

        if (width > height) // Landscape orientation
        {
            targetSize = width / 2f / aspectRatio;
        }
        else // Portrait orientation
        {
            targetSize = height / 2f;
        }
        targetSize += 1f; // adds padding

        Vector3 originalPosition = m_playerCamera.transform.position;
        Vector3 targetPosition = new Vector3(m_playerCamera.transform.position.x, targetSize, m_playerCamera.transform.position.z);

        float timeToLerp = 15f;
        float passedTime = 0f;
        float rotationLerp;

        m_playerCamera.gameObject.SetActive(false);
        m_flareCamera.gameObject.SetActive(true);
        while (passedTime < timeToLerp + 0.5f)
        {
            rotationLerp = 1f * Time.deltaTime;
            //m_flareCamera.transform.rotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(90f, 0f, 0f), rotationLerp);
            m_flareCamera.transform.LookAt(m_player.transform);
            m_flareCamera.transform.position = Vector3.Lerp(originalPosition, targetPosition, passedTime / timeToLerp);
            passedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        m_playerCamera.gameObject.SetActive(true);
        m_flareCamera.gameObject.SetActive(false);
    }

    /// <summary>
    /// Logic after picking up the flare
    /// </summary>
    public void FlarePickedUp()
    {
        m_flare.SetActive(true);
        m_hasFlare = true;
    }
    
    /// <summary>
    /// Animation of flare shaking in hand
    /// </summary>
    private void ShakeAnimation()
    {
        float perlinX = Mathf.PerlinNoise(Time.time * m_shakeSpeed, 0);
        float perlinY = Mathf.PerlinNoise(0, Time.time * m_shakeSpeed);

        float shakeOffsetX = (perlinX * 2 - 1) * m_shakeAmplitude;
        float shakeOffsetY = (perlinY * 2 - 1) * m_shakeAmplitude;

        m_flare.transform.position = m_initialPosition + new Vector3(shakeOffsetX, shakeOffsetY, 0);
    }
    #endregion

    #region Note
    /// <summary>
    /// Logic after picking up a note
    /// </summary>
    public void NotePickedUp()
    {
        byte notesNeeded = LevelStart.instance.NotesNeeded();

        m_notesPickedUp++;

        if (m_notesPickedUp >= Mathf.FloorToInt(notesNeeded / 2) && m_difficulty.HasFlag(DifficultySettigns.spawnMonster) && !m_isMonster)    //spawns the monster mid game
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
    #endregion

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