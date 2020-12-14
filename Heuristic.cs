using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class will store our heuristic function. The Heuristic function will determine the straight-line distance between two nodes.
public class Heuristic
{
    public Heuristic()
    {
    }
    public float Estimate(GameObject StartNode, GameObject GoalNode)
    {
        return Vector3.Distance(StartNode.transform.position, GoalNode.transform.position);
    }
}