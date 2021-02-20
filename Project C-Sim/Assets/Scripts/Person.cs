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

    public GameObject home { get; set; }
    public GameManager gameManager { get; set; }

    private float moveTimer;
    private float coolDown;
    private bool moving;
    private bool inHouse;
    private Vector2 destination;
    private float speed;
    private Rigidbody2D rb;
    
    // Start is called before the first frame update
    public void Start()
    {
        moveTimer = 0;
        coolDown = Random.Range(1.0f, 5.0f);
        moving = false;
        inHouse = true;
        rb = GetComponent<Rigidbody2D>();
        speed = 5f;
    }

    public void AssignRandomAttributes()
    {
        this.Sex = Random.Range(0, 2) == 0 ? Sex.Male : Sex.Female;
        this.Age = Random.Range(1, 90);
    }
    
    // Update is called once per frame
    public void Update()
    {
        if (!moving)
        {
            moveTimer += Time.deltaTime;
        }
        // The person is moving.
        else
        {
            Move();
        }


        // Start moving.
        if(moveTimer > coolDown)
        {
            moveTimer = 0;
            coolDown = Random.Range(1.0f, 5.0f);
            moving = true;
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
            Debug.Log(destination);
            inHouse = !inHouse;
        }
    }

    public void Move()
    {
        //If destination is reached
        if(Vector3.SqrMagnitude((Vector2)transform.position - destination) < 0.1f * 0.1f)
        {
            moving = false;
            rb.velocity = Vector3.zero;
            return;
        }

        Vector2 netForce;
        netForce = Seek(destination);
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
