using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PathfindingTester : MonoBehaviour
{
    // The A* manager.
    private AStarManager AStarManager = new AStarManager();
    // Array of possible waypoints.
    private List<GameObject> Waypoints = new List<GameObject>();
    // Array of waypoint map connections. Represents a path.
    private List<Connection> ConnectionArray = new List<Connection>();
    // The start and end nodes.
    [SerializeField]
    private GameObject start;
    [SerializeField]
    private GameObject end;
    // Debug line offset.

    private const float CLOSE_DISTANCE = 10;
    Vector3 OffSet = new Vector3(0, 0.3f, 0);



    // Speed variables
    private const float MAX_SPEED = 5;
    private float currentSpeed = MAX_SPEED;
    private bool slowedDown = false;
  


    // Start is called before the first frame update
    void Start()
    {
        if (start == null || end == null)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }
        // Find all the waypoints in the level.
        GameObject[] GameObjectsWithWaypointTag;
        GameObjectsWithWaypointTag = GameObject.FindGameObjectsWithTag("Waypoint");
        foreach (GameObject waypoint in GameObjectsWithWaypointTag)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            if (tmpWaypointCon)
            {
                Waypoints.Add(waypoint);
            }
        }
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            // Loop through a waypoints connections.
            foreach (GameObject WaypointConNode in tmpWaypointCon.Connections)
            {
                Connection aConnection = new Connection();
                aConnection.FromNode = waypoint;
                aConnection.ToNode = WaypointConNode;
                AStarManager.AddConnection(aConnection);
            }
        }
        // Run A Star...
        // ConnectionArray stores all the connections in the route to the goal / end node.
        ConnectionArray = AStarManager.PathfindAStar(start, end);
    }
    // Draws debug objects in the editor and during editor play (if option set).
    void OnDrawGizmos()
    {
        // Draw path.
        foreach (Connection aConnection in ConnectionArray)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine((aConnection.FromNode.transform.position + OffSet), (aConnection.ToNode.transform.position + OffSet));
        }
    }
    // Update is called once per frame
    void Update()
    {
        // Display the distance to pineapple
        Vector3 direction = ConnectionArray[0].ToNode.transform.position - transform.position;
            //closest.transform.position - transform.position;
        // Determine the distance of the vector
        float distance = direction.magnitude;

        // Calculate the normalised direction to the target from a game object.
        Vector3 normDirection = direction / distance;

        // Move the game object.
        transform.position = transform.position + normDirection * currentSpeed * Time.deltaTime;

        // Rotate the object.
        direction.y = 0;
        Quaternion rotation = Quaternion.LookRotation(-direction, Vector3.up);
        transform.rotation = rotation;

        if (distance < CLOSE_DISTANCE)
        {
            
        }
    }
}