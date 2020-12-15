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


    
   GameObject myPassenger;

    [SerializeField]
    int passengers = 0;

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

    // The game object which stores the taxi. This is the GameObject we attach our Agent script to.
    GameObject myTaxi; 

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


    void InitialiseText()
    {
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
    }

    GameObject RandomSeed()
    {
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        GameObject randomSeed;
        randomSeed = Waypoints[rand.Next(1, 12)];
        return randomSeed;
    }

    IEnumerator DeliverPassenger()
    {
        playTimer = false;
        isPlaying = false;
        
        myPassenger = Instantiate(PersonPrefab, myTaxi.transform.position - new Vector3(0.0f, 0f, 5.0f), Quaternion.identity);
        myPassenger.gameObject.tag = "Passenger";
        myPassenger.gameObject.name = myTaxi.name + " Passenger.";
        passengers -= 1;
        Debug.Log(myTaxi.name + " has arrived at their destination!");
        t_Passengers.text = ("Passengers: " + passengers + "/10");
        yield return new WaitForSeconds(50);
        UnityEditor.EditorApplication.isPlaying = false;
    }

    void CollectPassenger()
    {     
            arrived = true;          
            Destroy(myPassenger);
            Debug.Log(myPassenger.name + " has been destroyed.");
            passengers += 1;
            currentSpeed = MAX_SPEED - (MAX_SPEED / 10 * passengers);
    }

    void ReturnToStart()
    {
        Debug.Log(myTaxi.name + " is delivering.");
        returning = true;
        
        
        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            // Loop through a waypoints connections.
            foreach (GameObject WaypointConNode in tmpWaypointCon.Connections)
            {
                Connection aConnection = new Connection
                {
                    FromNode = waypoint,
                    ToNode = WaypointConNode
                };
                AStarManager.AddConnection(aConnection);
            }
        }
        // Run A Star...
        // ConnectionArray stores all the connections in the route to the goal / end node.
        ConnectionArray = AStarManager.PathfindAStar(end, start);

    }

    // Start is called before the first frame update
    void Start()
    {
        InitialiseText();
        myTaxi = this.gameObject;
        Application.targetFrameRate = 120;
        old_position = transform.position;
        myTaxi.transform.position = start.transform.position;

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

        GameObject randomNode;
        randomNode = RandomSeed();
        myPassenger = Instantiate(PersonPrefab, randomNode.transform.position - new Vector3(-3.0f, 0f, -3.0f), Quaternion.identity);
        myPassenger.gameObject.tag = "Passenger";
        myPassenger.gameObject.name = myTaxi.name + " Passenger.";

        end = randomNode;
        Debug.Log(myTaxi + " is driving from " + start + ", and collecting from " + end); // THIS WORKS
        t_node_end.text = "End Node: " + end.ToString();


        if (start == null || end == null)
        {
            Debug.Log("No start or end waypoints.");
            return;
        }

        // Go through the waypoints and create connections.
        foreach (GameObject waypoint in Waypoints)
        {
            WaypointCON tmpWaypointCon = waypoint.GetComponent<WaypointCON>();
            // Loop through a waypoints connections.
            foreach (GameObject WaypointConNode in tmpWaypointCon.Connections)
            {
                Connection aConnection = new Connection
                {
                    FromNode = waypoint,
                    ToNode = WaypointConNode
                };
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
                    // Check if we have reached a node.
                    if (distance < CLOSE_DISTANCE && arrived == false)
                    {                
                        connectionsTravelled++;
                        // Check if our node contains our passenger.
                        if (myPassenger != null)
                        {
                            if (Vector3.Distance(myTaxi.transform.position, myPassenger.transform.position) < 10)
                            {                           
                                CollectPassenger();
                            }
                        }                   
                        
                    }

                    if (distance > CLOSE_DISTANCE)
                    {          
                        arrived = false;
                    }

                }

            }
            else
            {
                // if we are already returning...
                if (returning == true)
                {                                       
                    StartCoroutine(DeliverPassenger());                
                }
                else
                {
                    ReturnToStart();
                    connectionsTravelled = 0;               
                }
            }
        }
        else
        {
            return;
        }
    }
}
