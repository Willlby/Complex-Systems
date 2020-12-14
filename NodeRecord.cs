using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will represent a NodeRecord in our waypoint map. The A* algorithm will use the NodeRecord to keep track of a nodes code-so-far and estimated-total-cost.

public class NodeRecord
{
    private GameObject node;
    public GameObject Node
    {
        get { return node; }
        set { node = value; }
    }
    private Connection connection;
    public Connection Connection
    {
        get { return connection; }
        set { connection = value; }
    }
    private float costSoFar;
    public float CostSoFar
    {
        get { return costSoFar; }
        set { costSoFar = value; }
    }
    private float estimatedTotalCost;
    public float EstimatedTotalCost
    {
        get { return estimatedTotalCost; }
        set { estimatedTotalCost = value; }
    }
    public NodeRecord()
    {
    }
}