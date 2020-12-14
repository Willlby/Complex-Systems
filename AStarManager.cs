using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*  This class will provide a simple interface for our A* algorithm and accompanying classes. 
    You do not have to implement this class to use the A* algorithm; however, it provides a simple way to group things together
    , so we will use it in this example.
*/


public class AStarManager
{
    // The a star algorithm.
    private AStar AStar = new AStar();
    // The waypoint graph.
    private Graph aGraph = new Graph();
    // The Heuristic.
    private Heuristic aHeuristic = new Heuristic();
    public AStarManager()
    {
    }
    // Add Connection.
    public void AddConnection(Connection connection)
    {
        aGraph.AddConnection(connection);
    }
    // Find path.
    public List<Connection> PathfindAStar(GameObject start, GameObject end)
    {
        return AStar.PathfindAStar(aGraph, start, end, aHeuristic);
    }
}