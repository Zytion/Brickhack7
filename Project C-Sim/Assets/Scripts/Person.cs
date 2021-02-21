using UnityEngine;

public enum Sex
{
    Male,
    Female
}
public class Person : MonoBehaviour
{
    public int Age { get; set; }
    public Sex Sex { get; set; }
    private bool infected;
    public bool Infected
    {
        get { return infected; }
        set
        {
            if (value)
            {
                //Change color
                GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
            infected = value;
        }
    }

    private bool recovered;
    public bool Recovered
    {
        get { return recovered; }
        set
        {
            if (value)
            {
                //Change color
                GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
            }
            recovered = value;
        }
    }

    public GameObject home { get; set; }
    public GameManager gameManager { get; set; }
    public bool Moving { get; set; }
    public bool SocialDistancing { get; set; }

    private float moveTimer;
    private float coolDown;
    private bool inHouse;
    private bool closeToDest;
    private float destinationRadius;
    private Vector2 destination;
    private float speed;
    private Rigidbody2D rb;
    private float recoverTimer;
    private float recoverTime;
    
    // Start is called before the first frame update
    public void Start()
    {
        moveTimer = 0;
        coolDown = Random.Range(1.0f, 5.0f);
        Moving = false;
        inHouse = true;
        rb = GetComponent<Rigidbody2D>();
        speed = 5f;
        closeToDest = false;
        destinationRadius = 1f;
        SocialDistancing = true;
        infected = false;
        recoverTime = 60.0f;
    }

    public void AssignRandomAttributes()
    {
        this.Sex = Random.Range(0, 2) == 0 ? Sex.Male : Sex.Female;
        this.Age = Random.Range(1, 90);
    }
    
    // Update is called once per frame
    public void Update()
    {
        // Don't move.
        if (!Moving)
        {
            moveTimer += Time.deltaTime;
        }
        // The person is moving.
        else
        {
            Move();
        }

        if(Infected)
        {
            recoverTimer += Time.deltaTime;
            if(recoverTimer > recoverTime)
            {
                //Check to see if dead
                if(Random.Range(0,1.0f) < 0.05f)
                {
                    //DEAD
                    gameManager.KillPerson(gameObject);
                }

                Infected = false;
                Recovered = true;
            }
        }


        // Start moving.
        if(moveTimer > coolDown)
        {
            moveTimer = 0;
            coolDown = Random.Range(1.0f, 5.0f);
            Moving = true;
            // Person in in house, so move person to a different building
            if (inHouse)
            {
                destination = gameManager.GetRandomBuilding();
            }
            // Move person back to home.
            else
            {
                destination = home.transform.position;
            }
            inHouse = !inHouse;
        }
    }

    public void Move()
    {
        //If destination is reached
        if(Vector3.SqrMagnitude((Vector2)transform.position - destination) < 0.1f * 0.1f)
        {
            Moving = false;
            rb.velocity = Vector3.zero;
            return;
        }

        closeToDest = Vector3.SqrMagnitude((Vector2)transform.position - destination) < destinationRadius * destinationRadius;

        Vector2 netForce;
        netForce = Seek(destination);
        if (!closeToDest && SocialDistancing)
        {
            netForce += gameManager.GetSeperation(this.gameObject);
        }
        rb.AddForce(netForce);
    }

    /// <summary>
    /// Steers the body towards a desired velocity
    /// </summary>
    /// <param name="desiredVelocity">Desired Velocity</param>
    /// <returns>Force to steer the body</returns>
    public Vector2 Steer(Vector2 desiredVelocity)
    {
        //Set up desired velocity
        desiredVelocity = desiredVelocity.normalized;
        desiredVelocity *= speed;

        //Calc steering force
        Vector2 steeringForce = desiredVelocity - rb.velocity;

        //Return force
        return steeringForce;
    }

    /// <summary>
    /// Seeks a target destination
    /// </summary>
    /// <param name="target">Target to seek</param>
    /// <returns>Force to seek the target</returns>
    public Vector2 Seek(Vector2 target)
    {
        Vector3 desiredVelocity = target - (Vector2)transform.position;
        return Steer(desiredVelocity);
    }
}
