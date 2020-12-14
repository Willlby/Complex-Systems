using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

// This class will represent connections in our waypoint map. The A* algorithm will use the connections to determine routes through the waypoint map.


public class Connection
{
    private float cost = 0;
    public float Cost
    {
        get
        {
            if (cost == 0)
            {
                cost = Vector3.Distance(FromNode.transform.position, ToNode.transform.position);
            }
            return cost;
        }
        set { cost = value; }
    }
    private GameObject fromNode;
    public GameObject FromNode
    {
        get { return fromNode; }
        set
        {
            fromNode = value;
            cost = 0;
        }
    }

    private GameObject toNode;
    public GameObject ToNode
    {
        get { return toNode; }
        set
        {
            toNode = value;
            cost = 0;
        }
    }
    // Default constructor.
    public Connection()
    {
    }
}