using System;
using Unity.AI.Navigation;
using UnityEngine;
using Random = System.Random;

#region Enums
/// <summary>
/// Different algorithm types
/// </summary>
public enum Algorithms
{
    RecursiveBacktracking = 0,
    testA = 1,
    testB = 2
}

/// <summary>
/// Custom difficulty settings flags
/// </summary>
[Flags]
public enum DifficultySettigns
{                     
       customSeed = 1,
       showMaze = 2,
       spawnMonster = 4, 
       spawnFlare = 8,
       respawn = 16
}
#endregion

/// <summary>
/// Handles maze generation and setup before player spawns
/// </summary>
public class MazeRenderer : MonoBehaviour
{
    #region Instance
    public static MazeRenderer instance;
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
    /// Seed used to generate the maze
    /// </summary>
    private int m_seed = 0;

    /// <summary>
    /// The width of a maze in nodes
    /// </summary>
    [Range(10, 250)]
    private uint m_width = 15;

    /// <summary>
    /// The height of a maze in nodes
    /// </summary>
    [Range(10, 250)]
    private uint m_height = 15;

    /// <summary>
    /// The size of a singe maze wall
    /// </summary>
    [Range(0.5f, 1.5f)]
    private float m_size = 1f;

    /// <summary>
    /// Vertical position of a singe maze wall
    /// </summary>
    private float m_vertical = -0.4f;

    /// <summary>
    /// A single maze wall prefab
    /// </summary>
    [SerializeField]
    private Transform m_wallPrefab = null;

    /// <summary>
    /// Custom game difficulty flags
    /// </summary>
    private DifficultySettigns m_difficultySettings;
    #endregion

    #region Getters/Setters
    /// <summary>
    /// Sets the width of a maze
    /// </summary>
    /// <param name="width">Width of a maze in nodes</param>
    public void SetMazeWidth(string width)
    {
        try
        {
            m_width = uint.Parse(width);
            if (m_width < 10)
            {
                m_width = 10;
            }
            else if (m_width > 250)
            {
                m_width = 250;
            }
        }
        catch (Exception)
        {
            m_width = 15;
            Debug.LogError("Parsing height: " + width + " failed, using fallback value of " + m_width);
        }
    }

    /// <summary>
    /// Sets the width of a maze
    /// </summary>
    /// <param name="height">Height of a maze in nodes</param>
    public void SetMazeHeight(string height)
    {
        try
        {
            m_height = uint.Parse(height);
            if (m_height < 10)
            {
                m_height = 10;
            }
            else if (m_height > 250)
            {
                m_height = 250;
            }
        }
        catch (Exception)
        {
            m_height = 15;
            Debug.LogError("Parsing height: " + height + " failed, using fallback value of " + m_height);
        }
    }

    /// <summary>
    /// Returns maze dimensions
    /// </summary>
    /// <returns>Width and height</returns>
    public Vector2 GetMazeSize()
    {
        return new Vector2(m_width, m_height);
    }

    /// <summary>
    /// Sets custom seed
    /// </summary>
    /// <param name="seed">Custom seed</param>
    public void SetCustomSeed(string seed)
    {
        m_seed = int.Parse(seed);
    }

    /// <summary>
    /// Disables custom seed
    /// </summary>
    public void DisableCustomSeed()
    {
        m_difficultySettings &= ~DifficultySettigns.customSeed;
    }

    /// <summary>
    /// Toggles custom seed
    /// </summary>
    public void ToggleCustomSeed(bool toggle)
    {
        if (toggle)
        {
            m_difficultySettings |= DifficultySettigns.customSeed;
        }
        else
        {
            m_difficultySettings &= ~DifficultySettigns.customSeed;
        }

    }

    /// <summary>
    /// Toggles showing the maze at the start
    /// </summary>
    public void ToggleMazeShow(bool toggle)
    {
        if (toggle)
        {
            m_difficultySettings |= DifficultySettigns.showMaze;
        }
        else
        {
            m_difficultySettings &= ~DifficultySettigns.showMaze;
        }

    }

    /// <summary>
    /// Toggles enemy spawning
    /// </summary>
    public void ToggleMonsterSpawn(bool toggle)
    {
        if (toggle)
        {
            m_difficultySettings |= DifficultySettigns.spawnMonster;
        }
        else
        {
            m_difficultySettings &= ~DifficultySettigns.spawnMonster;
        }

    }

    /// <summary>
    /// Toggles flare spawning
    /// </summary>
    public void ToggleSpawnFlare(bool toggle)
    {
        if (toggle)
        {
            m_difficultySettings |= DifficultySettigns.spawnFlare;
        }
        else
        {
            m_difficultySettings &= ~DifficultySettigns.spawnFlare;
        }

    }

    /// <summary>
    /// Toggles respawning
    /// </summary>
    public void ToggleRespawn(bool toggle)
    {
        if (toggle)
        {
            m_difficultySettings |= DifficultySettigns.respawn;
        }
        else
        {
            m_difficultySettings &= ~DifficultySettigns.respawn;
        }

    }
    #endregion

    #region Methods
    /// <summary>
    /// Starts the generation of a maze
    /// </summary>
    public void StartMazeGeneration(int algorithm)
    {
        if (!m_difficultySettings.HasFlag(DifficultySettigns.customSeed))
        {
            m_seed = new Random().Next();
        }
        WallState[,] maze = MazeGenerator.Generate(m_width, m_height, (Algorithms)algorithm, m_seed);   //generation of a layout
        BuildMaze(maze);                                                                                //generation of walls 
    }

    /// <summary>
    /// Builds the walls of the maze, based on the layout
    /// </summary>
    /// <param name="maze">Maze layout</param>
    private void BuildMaze(WallState[,] maze)
    {
        GameObject finishLine = null;
        maze[0, 0] &= ~WallState.left;                        //creates entrance
        maze[m_width - 1, m_height - 1] &= ~WallState.right;  //creates exit

        for (uint i = 0; i < m_width; ++i)  //Builds all walls
        {
            for (uint j = 0; j < m_height; ++j)
            {
                WallState cell = maze[i, j];
                Vector3 position = new Vector3(-m_width / 2 + i, 0, -m_height / 2 + j);

                if (cell.HasFlag(WallState.up))
                {
                    Transform topWall = Instantiate(m_wallPrefab, transform);
                    topWall.position = position + new Vector3(0, m_vertical, m_size / 2);
                    topWall.localScale = new Vector3(m_size, topWall.localScale.y, topWall.localScale.z);
                }
                if (cell.HasFlag(WallState.left))
                {
                    Transform leftWall = Instantiate(m_wallPrefab, transform);
                    leftWall.position = position + new Vector3(-m_size / 2, m_vertical, 0);
                    leftWall.localScale = new Vector3(m_size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }  
                if (i == m_width - 1)   //Places right column walls
                {
                    if (cell.HasFlag(WallState.right))
                    {
                        Transform rightWall = Instantiate(m_wallPrefab, transform);
                        rightWall.position = position + new Vector3(m_size / 2, m_vertical, 0);
                        rightWall.localScale = new Vector3(m_size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }

                    if (j == m_height - 1)  //places the finish line in top right corner
                    {
                        finishLine = Instantiate(Resources.Load("Prefabs/Finish Line"), position + new Vector3(0.5f, 0, 0), Quaternion.Euler(0f, -180f, 0f)) as GameObject;
                        finishLine.name = "Finish Line";
                    }
                }
                if (j == 0)   //Places bottom row walls
                {
                    if (cell.HasFlag(WallState.down))
                    {
                        Transform bottomWall = Instantiate(m_wallPrefab, transform);
                        bottomWall.position = position + new Vector3(0, m_vertical, -m_size / 2);
                        bottomWall.localScale = new Vector3(m_size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
                if (i == 0 && j == 0)   //Checking if we are at the bottom left corner
                {
                    GetComponent<LevelStart>().StartLevel(position + new Vector3(-m_size, 0.17f, 0), m_width, m_height, m_difficultySettings, m_seed);
                }
            }
        }
        if (LevelStart.instance.NotesNeeded() == 0)    //opens the gate if there are no notes to be picked up
        {
            finishLine.GetComponentInChildren<Animator>().SetBool("All notes collected", true);
        }
        GameObject floor = GameObject.Find("Floor");
        floor.transform.localScale = new Vector3(m_width / 4, 0.1f, m_height / 4);
        floor.GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    #endregion
}