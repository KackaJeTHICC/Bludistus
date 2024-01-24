using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeRenderer : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// The width of a maze in nodes
    /// </summary>
    [SerializeField] //TODO delete later and replace with getter/setters if needed 
    [Range(1, 100000000)]
    private uint m_width = 10;

    /// <summary>
    /// The height of a maze in nodes
    /// </summary>
    [SerializeField] //TODO delete later and replace with getter/setters if needed 
    [Range(1, 100000000)]
    private uint m_height = 10;

    /// <summary>
    /// The size of a singe maze wall
    /// </summary>
    [SerializeField] //TODO delete later and replace with getter/setters if needed 
    [Range(0.1f, 5f)]
    private float m_size = 1f;

    /// <summary>
    /// A single maze wall prefab
    /// </summary>
    [SerializeField]
    private Transform m_wallPrefab = null;
    #endregion

    #region Methods
    private void Start()
    {
        WallState[,] maze = MazeGenerator.Generate(m_width, m_height);
        Draw(maze); //TODO put this wherever needed
    }

    /// <summary>
    /// Builds the maze, based on the layout
    /// </summary>
    /// <param name="maze">Maze layout</param>
    private void Draw(WallState[,] maze)
    {
        for (uint i = 0; i < m_width; ++i)
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