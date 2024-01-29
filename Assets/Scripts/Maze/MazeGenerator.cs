using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

#region Enums/Structs
/// <summary>
/// All possible wall states in bits
/// </summary>
[Flags]
public enum WallState
{
      left = 1,     //0000 0001
      right = 2,    //0000 0010
      up = 4,       //0000 0100
      down = 8,     //0000 1000
      visited = 128 //1000 0000
}

/// <summary>
/// Currently selected node in maze
/// </summary>
public struct Position
{
    public uint X;
    public uint Y;
}

/// <summary>
/// Neighbors wall
/// </summary>
public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}
#endregion

/// <summary>
/// The class responsible for generating maze layout
/// </summary>
public static class MazeGenerator
{
    #region Growing Tree
    //TODO:)
    #endregion

    #region Recursive Backtracker Algorithm
    /// <summary>
    /// Returns a neighbors wall from the other side
    /// </summary>
    /// <param name="wall">The wall from which we would like to get the wall that is on the other side</param>
    /// <returns>A wall that is on the other side</returns>
    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.right: return WallState.left;
            case WallState.left: return WallState.right;
            case WallState.up: return WallState.down;
            case WallState.down: return WallState.up;
            default: return WallState.left; //A fallback just in case, even though this should never occur
        }
    }

    /// <summary>
    /// Core logic of the Recursive Backtracker algorithm
    /// </summary>
    /// <param name="maze">Maze we would like to modify</param>
    /// <param name="width">Width of given maze in nodes/param>
    /// <param name="height">Height of given maze in nodes</param>
    /// <returns>Returns a maze layout, which should be rendered</returns>
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, uint width, uint height, int seed)
    {
        Random rng = new Random(seed);
        Stack<Position> positionStack = new Stack<Position>();
        Position position = new Position { X = (uint)rng.Next(0, (int)width), Y = (uint)rng.Next(0, (int)height) };

        maze[position.X, position.Y] |= WallState.visited;  //1000 <whatever WallState value> 
        positionStack.Push(position);

        while (positionStack.Count > 0) //Checks all unvisited neighbors
        {
            var current = positionStack.Pop();
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            if (neighbours.Count > 0) //Remove shared wall, if theres is one
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                var randomPosition = randomNeighbour.Position;
                maze[current.X, current.Y] &= ~randomNeighbour.SharedWall;
                maze[randomPosition.X, randomPosition.Y] &= ~GetOppositeWall(randomNeighbour.SharedWall);
                maze[randomPosition.X, randomPosition.Y] |= WallState.visited;

                positionStack.Push(randomPosition);
            }
        }

        return maze;
    }

    /// <summary>
    /// Returns a list of unvisited neighbors
    /// </summary>
    /// <param name="p">Current position</param>
    /// <param name="maze">A maze layout</param>
    /// <param name="width">Width of maze in nodes</param>
    /// <param name="height">Height of maze in nodes</param>
    /// <returns>A list of unvisited neighbors</returns>
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, uint width, uint height)
    {
        List<Neighbour> list = new List<Neighbour>();

        if (p.X > 0) // left
        {
            if (!maze[p.X - 1, p.Y].HasFlag(WallState.visited))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.left
                });
            }
        }

        if (p.Y > 0) // down
        {
            if (!maze[p.X, p.Y - 1].HasFlag(WallState.visited))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.down
                });
            }
        }

        if (p.Y < height - 1) // up
        {
            if (!maze[p.X, p.Y + 1].HasFlag(WallState.visited))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.up
                });
            }
        }

        if (p.X < width - 1) // right
        {
            if (!maze[p.X + 1, p.Y].HasFlag(WallState.visited))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.right
                });
            }
        }

        return list;
    }
    #endregion

    #region Core
    /// <summary>
    /// The function responsible for generating the maze layout
    /// </summary>
    /// <param name="width">Width of the maze in nodes</param>
    /// <param name="height">Height of the maze in nodes</param>
    /// <returns>A 2D array of WallStates, which can be used to build the maze</returns>
    public static WallState[,] Generate(uint width, uint height, Algorithms algorithm, int seed)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.right | WallState.left | WallState.up | WallState.down;   // 1111
        for (uint i = 0; i < width; ++i)
        {
            for (uint j = 0; j < height; ++j)
            {
                maze[i, j] = initial;
            }
        }

        switch (algorithm)
        {
            case Algorithms.RecursiveBacktracking:
                return ApplyRecursiveBacktracker(maze, width, height, seed);
            case Algorithms.testA:
                return ApplyRecursiveBacktracker(maze, width, height, seed);
            case Algorithms.testB:
                return ApplyRecursiveBacktracker(maze, width, height, seed);
            default:
                Debug.LogError("Invalid algorithm: " + algorithm + ", falling back to Recursive backtracker");
                return ApplyRecursiveBacktracker(maze, width, height, seed); //A fallback just in case, even though this should never occur
        }
    }
    #endregion
}