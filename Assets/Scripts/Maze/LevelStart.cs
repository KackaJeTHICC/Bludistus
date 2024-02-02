using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Handles everything in between maze generation and game start
/// </summary>
public class LevelStart : MonoBehaviour
{
    #region Instance
    public static LevelStart instance;
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
    /// Cutscene camera object
    /// </summary> 
    [SerializeField]
    private Camera m_cutsceneCamera = null;

    /// <summary>
    /// Renderer camera object
    /// </summary> 
    [SerializeField]
    private GameObject m_rendererCamera = null;

    /// <summary>
    /// Player game object
    /// </summary>
    [SerializeField]
    private GameObject m_player = null;

    /// <summary>
    /// Amount of notes needed to open the door
    /// </summary>
    private byte m_notesNeeded = 0;
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

    /// <summary>
    /// Sets the amount of nodes needed to unlock the finish line
    /// </summary>
    /// <returns></returns>
    public byte NotesNeeded()
    {
        return m_notesNeeded;
    }
    #endregion

    #region Methods
    private void Start()
    {   
        //This cases should never happen and the fallbacks will most likely fail, as some of the game objects should be disabled by default
        if (m_player == null)   
        {
            Debug.LogError("Player isn't assigned!");
            m_player = GameObject.Find("PlayerCapsule");    
        }
        if (m_cutsceneCamera == null)
        {
            Debug.LogError("Cutscene camera isn't assigned!");
            m_cutsceneCamera = GameObject.Find("CutsceneCamera").GetComponent<Camera>();
        }
        if (m_cutsceneCamera == null)
        {
            Debug.LogError("Cutscene camera isn't assigned!");
            m_rendererCamera = GameObject.Find("Rendered Image Camera");
        }
    }

    /// <summary>       
    /// Sets up everything needed to start playing the game after maze has been generated
    /// </summary>                                   
    /// <param name="playerStartLocation">Starting location of a player</param>    
    /// <param name="widht">Width of a maze in nodes</param>    
    /// <param name="height">Height of a maze in nodes</param>
    /// <param name="difficulty">Custom difficulty settings flag</param>
    public void StartLevel(Vector3 playerStartLocation, uint widht, uint height, DifficultySettigns difficulty)
    {
        if (difficulty.HasFlag(DifficultySettigns.showMaze))
        {
            StartCoroutine(SetUpCamera(playerStartLocation, widht, height));
        }
        else
        {
            PlayerSpawn(playerStartLocation);
        }
        GameManager.instance.Difficulty(difficulty);
    }

    /// <summary>
    /// Plays the zooming out animation
    /// </summary>
    /// <param name="playerStartLocation">Starting location of a player</param>    
    /// <param name="widht">Width of a maze in nodes</param>    
    /// <param name="height">Height of a maze in nodes</param>
    private IEnumerator SetUpCamera(Vector3 playerStartLocation, uint widht, uint height)
    {
        float aspectRatio = Screen.width / (float)Screen.height;
        float targetSize;

        if (widht > height) // Landscape orientation
        {
            targetSize = widht / 2f / aspectRatio;
        }
        else // Portrait orientation
        {
            targetSize = height / 2f;
        }
        targetSize += 1f; //adds padding

        float orthographicSize = m_cutsceneCamera.orthographicSize;

        float timeToLerp = 15f;
        float passedTime = 0f;

        while (passedTime < timeToLerp + 0.5f)  //smoothly zooms the camera out, so the entire maze fits on screen
        {
            orthographicSize = Mathf.Lerp(orthographicSize, targetSize, passedTime/timeToLerp);
            m_cutsceneCamera.orthographicSize = orthographicSize;
            passedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        PlayerSpawn(playerStartLocation);
    }

    /// <summary>
    /// Spawns the player
    /// </summary>
    /// <param name="playerStartLocation">Location, where player should be spawned</param>
    private void PlayerSpawn(Vector3 playerStartLocation)   //switches to the player cam
    {
        m_player.transform.localPosition = playerStartLocation;
        m_player.gameObject.SetActive(true);
        m_rendererCamera.gameObject.SetActive(true);
        m_cutsceneCamera.gameObject.SetActive(false);
    }
    
    private Vector3 RandomSpot()
    {
        print("TODO");
        return Vector3.zero;
    }
    #endregion
}
