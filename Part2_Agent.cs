using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Part2_Agent : MonoBehaviour
{

    float fps; // Used to store Frames Per Second (FPS).
    float totalDistanceTravelled = 0;

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
    public GameObject end;

    // Debug line offset.
    private GameObject swap;
 
    [SerializeField]
    public GameObject PersonPrefab;
    GameObject PersonInstance;
    public int passengersToSpawn;


    public GameObject[] potentialPassengers;
    private List<GameObject> spawnedNodes = new List<GameObject>();

    int passengers = 0;
    Vector3 start_position;
    Vector3 end_position;
    Vector3 old_position;
    
  
    // Initialising text handlers (UI).
    Text t_Passengers;
    Text t_Returning;
    Text t_Runtime;
    Text t_ConnectionsTravelled;
    Text t_DistanceTravelled;
    Text t_node_start;
    Text t_node_end;
    Text t_fps;
    Text t_Speed;

    // The game object which stores the bus. This is the GameObject we attach our Agent script to.
    GameObject bus; 

    Vector3 OffSet = new Vector3(0, 0.5f, 0);

    Boolean arrived = false;
    Boolean returning = false;
    Boolean playTimer = true; //
    Boolean isPlaying = true; // Used to control the Update method.

    int connectionsTravelled = 0;

    // Speed variables
    private const float MAX_SPEED = 20;
    private float currentSpeed = MAX_SPEED;
    private bool slowedDown = false;
    private const float CLOSE_DISTANCE = 1;


    IEnumerator FinishSimulation()
    {
        
        Debug.Log("FinishSimulation() has been called.");
        for (int i = 0; i < passengers;)
        {
            Instantiate(PersonPrefab, start_position - new Vector3(0.0f, 0f, 10.0f + passengers), Quaternion.identity);
            passengers -= 1;          
        }
        //Instantiate(People[0], start_position - new Vector3(0.0f, 0f, 10.0f), Quaternion.identity);
        
        t_Passengers.text = ("Passengers: " + passengers + "/10");

        yield return new WaitForSeconds(5);

        UnityEditor.EditorApplication.isPlaying = false;

    }

    void CollectPassenger()
    {
        potentialPassengers = GameObject.FindGameObjectsWithTag("Passenger");
        connectionsTravelled++;
        t_ConnectionsTravelled.text = "Connections Travelled: " + connectionsTravelled;
        arrived = true;
        for (int j = 0; j < potentialPassengers.Length; j++)
        {
            if (Vector3.Distance(bus.transform.position, potentialPassengers[j].transform.position) < 10)
            {
                Destroy(potentialPassengers[j].gameObject);
                passengers += 1;
                currentSpeed = MAX_SPEED - (MAX_SPEED / 10 * passengers); 
            }
        }
    }


    void ReturnToStart()
    {
        returning = true;

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
        ConnectionArray = AStarManager.PathfindAStar(end, start);
        Debug.Log(ConnectionArray.Count);

    }

    // Start is called before the first frame update
    void Start()
    {
        
        Application.targetFrameRate = 120;
        Debug.Log(Waypoints);
        // Get coordinates for the start and end nodes in order to spawn bus/child at start/end nodess respectively.
        bus = GameObject.Find("SchoolBus");
        start_position = start.transform.position;
        end_position = end.transform.position;
        old_position = transform.position;

        bus.transform.position = start.transform.position;


        Debug.Log(end_position);
        t_Passengers = GameObject.Find("Canvas/Passengers").GetComponent<Text>();
        t_Returning = GameObject.Find("Canvas/Returning").GetComponent<Text>();
        t_Runtime = GameObject.Find("Canvas/Runtime").GetComponent<Text>();
        t_ConnectionsTravelled = GameObject.Find("Canvas/ConnectionsTravelled").GetComponent<Text>();
        t_DistanceTravelled = GameObject.Find("Canvas/DistanceTravelled").GetComponent<Text>();
        t_node_start = GameObject.Find("Canvas/Node_START").GetComponent<Text>();
        t_node_end = GameObject.Find("Canvas/Node_END").GetComponent<Text>();
        t_fps = GameObject.Find("Canvas/FPS").GetComponent<Text>();
        t_Speed = GameObject.Find("Canvas/Speed").GetComponent<Text>();

        t_node_start.text = "Start Node: " + start.ToString();
        t_node_end.text = "End Node: " + end.ToString();
  
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
        Debug.Log(ConnectionArray.Count);


        System.Random rand = new System.Random();
        potentialPassengers = new GameObject[passengersToSpawn];

        for (int i = 0; i < passengersToSpawn; i++)
        {
            
            GameObject randomNode;
            do
             {
                 randomNode = Waypoints[rand.Next(1, 12)];
             } while (spawnedNodes.Contains(randomNode));
            
            spawnedNodes.Add(randomNode);
            Debug.Log("SpawnNode: " + randomNode);
            potentialPassengers[i] = Instantiate(PersonPrefab, randomNode.transform.position - new Vector3(-6.0f, 0f, 0f), Quaternion.identity);
            potentialPassengers[i].gameObject.tag = "Passenger";
        }
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
       if (isPlaying == true)
        {

            fps = 1 / Time.unscaledDeltaTime;
            t_fps.text = "Framerate: " + fps.ToString("00");
            t_DistanceTravelled.text = "Distance Travelled: " + totalDistanceTravelled.ToString("00");
            t_Passengers.text = "Passengers: " + passengers.ToString() + "/10";
            t_Returning.text = "Returning: " + returning;
            t_Speed.text = "Speed: " + currentSpeed;
            
 
            float distanceTravelled = Vector3.Distance(old_position, transform.position);
            totalDistanceTravelled += distanceTravelled;
            old_position = transform.position;


            if (playTimer == true)
            {
                t_Runtime.text = ("Runtime: " + Time.time);
                
            }


            if (connectionsTravelled < ConnectionArray.Count)
            {
                // Display the distance to waypoint
                Vector3 direction = ConnectionArray[connectionsTravelled].ToNode.transform.position - transform.position;

                //closest.transform.position - transform.position;
                // Determine the distance of the vector
                float distance = direction.magnitude;

                // Calculate the normalised direction to the target from a game object.
                Vector3 normDirection = direction / distance;

                // Move the game object.
                transform.position = transform.position + normDirection * currentSpeed * Time.deltaTime;

                // Rotate the object.
                direction.y = 0;
                Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = rotation;

                {
                    if (distance < CLOSE_DISTANCE && arrived == false)
                    {
                        CollectPassenger();
                    }

                    if (distance > CLOSE_DISTANCE)
                    {
                        arrived = false;
                    }

                }

            }
            else
            {

                if (returning == true)
                {
                    Debug.Log("FINISHED");
                    
                    playTimer = false;

                    StartCoroutine(FinishSimulation());

                    isPlaying = false;




                    //Application.Quit()
                }
                else
                {
                    Debug.Log("HALFWAY THERE, CA Count: " + ConnectionArray.Count);

                    if (returning == false)
                    {
                        ReturnToStart();
                    }
             
                    connectionsTravelled = 0;               
                }

            }
        }
        else
        {

        }

    }
}
