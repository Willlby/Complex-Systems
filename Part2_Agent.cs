using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Part2_Agent : MonoBehaviour
{

    float fps; // Used to store Frames Per Second (FPS).
    float totalDistanceTravelled = 0;
    public int NumberToSpawn;
    List<GameObject> CollectedPassengers = new List<GameObject>();


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
    
    
    
   List <GameObject> myPassengers = new List<GameObject>();

    [SerializeField]
    int passengers = 0;

    Vector3 old_position;

    // Initialising text handlers (UI).
    Text t_Passengers;
    Text t_Runtime;
    Text t_fps;
    Text t_Speed;

    public GameObject go_Passengers;
    public GameObject go_Speed;
    

    // The game object which stores the taxi. This is the GameObject we attach our Agent script to.
    GameObject myTaxi; 

    Vector3 OffSet = new Vector3(0, 0.5f, 0);

    Boolean arrived = false;
    Boolean returning = false;
    Boolean playTimer = true; //
    Boolean isPlaying = true; // Used to control the Update method.

    public int connectionsTravelled = 0;

    // Speed variables
    private float MAX_SPEED = 20;
    [SerializeField]
    private float currentSpeed = 20;
    private bool slowedDown = false;
    private const float CLOSE_DISTANCE = 1;


    void InitialiseText()
    {
        t_Passengers = go_Passengers.GetComponent<Text>();
        t_Speed = go_Speed.GetComponent<Text>();
        t_Runtime = GameObject.Find("Canvas/Runtime").GetComponent<Text>();      
        t_fps = GameObject.Find("Canvas/FPS").GetComponent<Text>();
        
    }

    GameObject RandomSeed()
    {
        System.Random rand = new System.Random(Guid.NewGuid().GetHashCode());
        GameObject randomSeed;
        randomSeed = Waypoints[rand.Next(2, 12)];       
        NumberToSpawn = rand.Next(1, 4); 
        return randomSeed;
    }

    void CollectPassenger()
    {
        arrived = true;
        List<GameObject> CollectedPassengers = new List<GameObject>();


        for (int i = 0; i < NumberToSpawn; i++)
        {
            CollectedPassengers.Add(myPassengers[i]);
            Destroy(myPassengers[i]);
            Debug.Log(CollectedPassengers[i].name + " has been collected.");
            passengers += 1;
            MAX_SPEED = MAX_SPEED - (MAX_SPEED / 10 * passengers);
        }
        if (myPassengers != null)
        {
            myPassengers.Clear();
        }
       

    }

    void DeliverPassenger()
    {
        playTimer = false;
        isPlaying = false;
        int i=0;

        for (i=0; i < NumberToSpawn; i++)
        {
            CollectedPassengers.Add(Instantiate(PersonPrefab, myTaxi.transform.position - new Vector3(0.0f, 0f, 5.0f + i), Quaternion.identity));
            CollectedPassengers[i].gameObject.tag = "Passenger";
            CollectedPassengers[i].gameObject.name = myTaxi.name + " Passenger " + (i+1);
            passengers -= 1;
        }
        
        
        
        currentSpeed = 0;
        Debug.Log(myTaxi.name + " has completed their trip, and in " + Time.time + " seconds.");
        t_Passengers.text = myTaxi.name + " Passengers: " + passengers.ToString();
        t_Speed.text = myTaxi.name + " Speed: " + currentSpeed.ToString();

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


        for (int i = 0; i < NumberToSpawn; i++)
        {         
            myPassengers.Add(Instantiate(PersonPrefab, randomNode.transform.position - new Vector3(-3.0f + i, 0f, -3.0f), Quaternion.identity));
            myPassengers[i].gameObject.tag = "Passenger";
            myPassengers[i].gameObject.name = myTaxi.name + " Passenger " + (i+1);      
        }

      
        end = randomNode;
        Debug.Log(myTaxi.name + " is collecting " + myPassengers.Count  +   " passengers from " + end.name); // THIS WORKS
        


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
    void OnDrawGizmosSelected()
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
            
            t_Passengers.text = myTaxi.name + " Passengers: " + passengers.ToString();
            t_Speed.text = myTaxi.name + " Speed: " + currentSpeed.ToString();
            
 
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
                CollisionDetection();

                {
                    // Check if we have reached a node.
                    if (distance < CLOSE_DISTANCE && arrived == false)
                    {                
                        connectionsTravelled++;
                        // Check if our node contains our passenger.
                        if (myPassengers.Count != 0)
                        {
                            if (Vector3.Distance(myTaxi.transform.position, myPassengers[0].transform.position) < 10)
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
                    DeliverPassenger();                
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
            connectionsTravelled = 0;
        }
    }

    void CollisionDetection()
    {
        List<float> taxiDistancesToNextNode = new List<float>();
        GameObject[] otherTaxis = GameObject.FindGameObjectsWithTag("Taxi"); // working
        //Debug.Log(otherTaxis.Length);
        bool coming_at_me = false;
        bool crashing = false;
        GameObject currentNode = ConnectionArray[connectionsTravelled].FromNode;
        GameObject nextNode = ConnectionArray[connectionsTravelled].ToNode;



        int i = 0;
        foreach (var otherTaxi in otherTaxis)
        {
            if (otherTaxi.name == this.gameObject.name)
            {
                continue;
            }

            // Gain access to information regarding other taxis.
            Part2_Agent path = otherTaxi.GetComponent<Part2_Agent>();

            if (path == null)
            {
                Debug.Log("path == null");
                continue;
                
            }

            if (path.ConnectionArray[path.connectionsTravelled].ToNode != (nextNode || currentNode))
            {              
                continue;
            }

            if (path.ConnectionArray[path.connectionsTravelled].ToNode == currentNode)
            {              
                coming_at_me = true;
            }
 
            
                
            
          

            float distanceToMe = Vector3.Distance(myTaxi.transform.position, otherTaxi.transform.position);

            if (distanceToMe < 8) // the safety zone
            {
                taxiDistancesToNextNode.Add(Vector3.Distance(otherTaxi.transform.position, nextNode.transform.position));
                i++;
                crashing = true;
            }

        }

        bool closerThanOther = true;
        float myDistanceToNextNode = Vector3.Distance(nextNode.transform.position, transform.position);

        foreach (var otherDistance in taxiDistancesToNextNode)
        {
           
            if (myDistanceToNextNode > otherDistance)
            {
                closerThanOther = false;
            }
        }

        if (crashing == true)
        {
            if (closerThanOther == true)
            {
                Speedup();            
            }

            else if (coming_at_me == true)
            {
                transform.localPosition = transform.localPosition + transform.right;
            }

            else
            {               
                Slowdown();
            }

        }
        
        else
        {
            Speedup();
        }

    }


    void Speedup()
    {
        if (currentSpeed + 2 < MAX_SPEED)
        {
            currentSpeed += 2;
        }
        else 
        {
            currentSpeed = MAX_SPEED;
        }

        if (currentSpeed > MAX_SPEED)
        {
            currentSpeed = MAX_SPEED;
        }
    }

    void Slowdown()
    {
        if (currentSpeed - 2 >= 0)
        {
            currentSpeed -= 2;
        }
        else 
        {
            currentSpeed = 0;
        }
    }
    
}
