using System;
using System.Collections.Generic;

/// <summary>
/// All possible wall states in bits
/// </summary>
[Flags]
public enum WallState
{
      LEFT = 1,     //0000 0001
      RIGHT = 2,    //0000 0010
      UP = 4,       //0000 0100
      DOWN = 8,     //0000 1000

      VISITED = 128 //1000 0000
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
/// TODO summary
/// </summary>
public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}

/// <summary>
/// The class responsible for generating maze layout
/// </summary>
public static class MazeGenerator
{
    #region RecursiveBacktrackerAlgorithm
    /// <summary>
    /// Returns a neighbors wall from the other side
    /// </summary>
    /// <param name="wall">The wall from which we would like to get the wall that is on the other side</param>
    /// <returns>A wall that is on the other side</returns>
    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.LEFT; //A fallback just in case, even though this should never occur
        }
    }

    /// <summary>
    /// Core logic of the Recursive Backtracker algorithm
    /// </summary>
    /// <param name="maze">Maze we would like to modify</param>
    /// <param name="width">Width of given maze in nodes/param>
    /// <param name="height">Height of given maze in nodes</param>
    /// <returns>Returns a maze layout, which should be rendered</returns>
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, uint width, uint height)
    {
        Random rng = new Random();
        Stack<Position> positionStack = new Stack<Position>();
        Position position = new Position { X = (uint)rng.Next(0, (int)width), Y = (uint)rng.Next(0, (int)height) }; //TODO check if uint here is ok

        maze[position.X, position.Y] |= WallState.VISITED;  //1000 <whatever WallState value> 
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
                maze[randomPosition.X, randomPosition.Y] |= WallState.VISITED;

                positionStack.Push(randomPosition);
            }
        }

        return maze;
    }

    /// <summary>
    /// Returns a list of unvisited neighbors //TODO fill params
    /// </summary>
    /// <param name="p"></param>
    /// <param name="maze"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns>A list of unvisited neighbors</returns>
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, uint width, uint height)
    {
        List<Neighbour> list = new List<Neighbour>();

        if (p.X > 0) // left
        {
            if (!maze[p.X - 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X - 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

        if (p.Y > 0) // DOWN
        {
            if (!maze[p.X, p.Y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        if (p.Y < height - 1) // UP
        {
            if (!maze[p.X, p.Y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X,
                        Y = p.Y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        if (p.X < width - 1) // RIGHT
        {
            if (!maze[p.X + 1, p.Y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        X = p.X + 1,
                        Y = p.Y
                    },
                    SharedWall = WallState.RIGHT
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
    public static WallState[,] Generate(uint width, uint height, MazeRenderer.Algorithms algorithm)
    {
        WallState[,] maze = new WallState[width, height];
        WallState initial = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;   // 1111
        for (uint i = 0; i < width; ++i)
        {
            for (uint j = 0; j < height; ++j)
            {
                maze[i, j] = initial;
            }
        }

        switch (algorithm)
        {
            case MazeRenderer.Algorithms.RecursiveBacktracking:
                return ApplyRecursiveBacktracker(maze, width, height);
            case MazeRenderer.Algorithms.testA:
                //TODO change here
                return ApplyRecursiveBacktracker(maze, width, height);
            case MazeRenderer.Algorithms.testB:
                //TODO change here
                return ApplyRecursiveBacktracker(maze, width, height);
            default:
                return ApplyRecursiveBacktracker(maze, width, height); //A fallback just in case, even though this should never occur
        }
    }
    #endregion
}