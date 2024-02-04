using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

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
    
    /// <summary>
    /// Width of the maze
    /// </summary>
    private uint m_mazeWidth = 5;

    /// <summary>
    /// Height of the maze
    /// </summary>
    private uint m_mazeHeight = 5;

    /// <summary>
    /// Scale of a maze
    /// </summary>
    private float m_size = 1f;

    /// <summary>
    /// List of notes textures
    /// </summary>
    public List<Material> m_noteMaterials;

    /// <summary>
    /// Random number generator
    /// </summary>
    private Random m_rng;
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
        foreach (Material m in Resources.LoadAll("Materials/", typeof(Material)))
        {
            if (m.name.Contains("note") && m.name != "note0")
            {
                m_noteMaterials.Add(m);
            }
        }

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
    /// <param name="width">Width of a maze in nodes</param>    
    /// <param name="height">Height of a maze in nodes</param>
    /// <param name="difficulty">Custom difficulty settings flag</param>
    /// <param name="seed">Seed for the random number generator</param>
    public void StartLevel(Vector3 playerStartLocation, uint width, uint height, float size, DifficultySettigns difficulty, int seed)
    {
        m_rng = new Random(seed);
        m_size = size;
        m_mazeWidth = width;
        m_mazeHeight = height;

        for (int i = 0; i < m_notesNeeded; i++)
        {
            NoteSpawn();
        }
        if (difficulty.HasFlag(DifficultySettigns.spawnFlare))
        {
            FlareSpawn();
        }

        if (difficulty.HasFlag(DifficultySettigns.showMaze))
        {
            StartCoroutine(SetUpCamera(playerStartLocation, width, height));
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
    
    /// <summary>
    /// Spawns the flare
    /// </summary>
    private void FlareSpawn()
    {
        Instantiate(Resources.Load("Prefabs/Flare"), RandomSpot(), Quaternion.Euler(90f, 0f, m_rng.Next(0, 360)));
    }

    /// <summary>
    /// Spawns the note
    /// </summary>
    private void NoteSpawn()
    {
        GameObject note = Instantiate(Resources.Load("Prefabs/Note"), RandomSpot(), Quaternion.Euler(0f, m_rng.Next(0, 360), 0f)) as GameObject;
        note.GetComponent<MeshRenderer>().material = m_noteMaterials.ElementAt(new Random().Next(0, m_noteMaterials.Count));
    }

    /// <summary>
    /// Returns a random spot in maze
    /// </summary>
    /// <returns>position.y has -0.486f offset</returns>
    public Vector3 RandomSpot()
    {
        return new Vector3(m_rng.Next(-Mathf.RoundToInt(m_mazeWidth/2), Mathf.RoundToInt(m_mazeWidth / 2)),
            -0.486f,
            m_rng.Next(-Mathf.RoundToInt(m_mazeHeight / 2), Mathf.RoundToInt(m_mazeHeight / 2)));
    }
    #endregion
}
