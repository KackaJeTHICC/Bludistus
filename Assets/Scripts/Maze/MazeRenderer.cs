using System;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    #region Variables
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
    [Range(0.1f, 5f)]
    private float m_size = 1f;

    /// <summary>
    /// A single maze wall prefab
    /// </summary>
    [SerializeField]
    private Transform m_wallPrefab = null;

    /// <summary>
    /// Player game object
    /// </summary>
    [SerializeField]
    private GameObject m_player = null;

    /// <summary>
    /// Different algorithm types
    /// </summary>
    public enum Algorithms
    {
        RecursiveBacktracking = 0,
        testA = 1,
        testB = 2
    }
    #endregion

    #region Methods
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
    /// Starts the generation of a maze
    /// </summary>
    public void StartMazeGeneration(int algorithm)
    {
        WallState[,] maze = MazeGenerator.Generate(m_width, m_height, (Algorithms)algorithm);  //generation of a layout
        DrawMaze(maze);                                                                        //generation of walls 
    }

    /// <summary>
    /// Builds the walls of the maze, based on the layout
    /// </summary>
    /// <param name="maze">Maze layout</param>
    private void DrawMaze(WallState[,] maze)
    {              
        Vector3 startingPosition = new Vector3(-m_width / 2 - 1, 0, -m_height / 2);//TODO + new Vector3(0, 0, -m_size / 2)
        Vector3 finishingPosition = new Vector3(m_width / 2 - 1, 0, m_height / 2 - 1) + new Vector3(m_size, 0, 0); //TODO
        /*
         something along the lines one "if width is even or odd" or "if length is even or odd" 
         */

        maze[0, 0] &= ~WallState.LEFT;                        //creates entrance
        maze[m_width - 1,  m_height- 1] &= ~WallState.RIGHT;  //creates exit

        Instantiate(Resources.Load("Prefabs/Finish Line"), finishingPosition, new Quaternion(0, 0, 0, 0));//TODO move down

        if (m_player == null)
        {
            Debug.LogError("Player isn't assigned!");
            m_player = GameObject.Find("PlayerCapsule");    //will most likely fail, as player game object should be disabled by default
        }
        m_player.transform.localPosition = startingPosition; //moves player to the start of the maze
        m_player.gameObject.SetActive(true);

        for (uint i = 0; i < m_width; ++i)  //Builds all walls
        {
            for (uint j = 0; j < m_height; ++j)
            {
                WallState cell = maze[i, j];
                Vector3 position = new Vector3(-m_width / 2 + i, 0, -m_height / 2 + j);

                if (cell.HasFlag(WallState.UP))
                {
                    Transform topWall = Instantiate(m_wallPrefab, transform);
                    topWall.position = position + new Vector3(0, 0, m_size / 2);
                    topWall.localScale = new Vector3(m_size, topWall.localScale.y, topWall.localScale.z);
                }
                if (cell.HasFlag(WallState.LEFT))
                {
                    Transform leftWall = Instantiate(m_wallPrefab, transform);
                    leftWall.position = position + new Vector3(-m_size / 2, 0, 0);
                    leftWall.localScale = new Vector3(m_size, leftWall.localScale.y, leftWall.localScale.z);
                    leftWall.eulerAngles = new Vector3(0, 90, 0);
                }

                if (i == m_width - 1)   //Checking if we are at the last column  
                {
                    if (cell.HasFlag(WallState.RIGHT))
                    {
                        Transform rightWall = Instantiate(m_wallPrefab, transform);
                        rightWall.position = position + new Vector3(m_size / 2, 0, 0);
                        rightWall.localScale = new Vector3(m_size, rightWall.localScale.y, rightWall.localScale.z);
                        rightWall.eulerAngles = new Vector3(0, 90, 0);
                    }
                }

                if (j == 0)   //Checking if we are at the first row
                {
                    if (cell.HasFlag(WallState.DOWN))
                    {
                        Transform bottomWall = Instantiate(m_wallPrefab, transform);
                        bottomWall.position = position + new Vector3(0, 0, -m_size / 2);
                        bottomWall.localScale = new Vector3(m_size, bottomWall.localScale.y, bottomWall.localScale.z);
                    }
                }
            }
        }
    }
    #endregion
}