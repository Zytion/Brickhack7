﻿using UnityEngine;

public enum Sex
{
    Male,
    Female
}
public class Person : MonoBehaviour
{
    private int age;
    public int Age {
        get { return age; }
        set { age = value; this.transform.localScale = new Vector3(Mathf.Clamp(value/120f, 0.4f, 0.8f), Mathf.Clamp(value / 120f, 0.4f, 0.8f), 1);}
    }
    private Sex sex;
    public Sex Sex {
        get { return sex; }
        set { if(value == Sex.Female)
            {
                this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WPerson");
            }
            sex = value;
        } 
    }
    private bool hasMask;
    public bool HasMask
    {
        get { return hasMask; }
        set
        {
            if (value)
            {
                if(sex == Sex.Male)
                {
                    this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("MaskPerson");
                }
                else
                {
                    this.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("WMPerson");
                }
            }
            hasMask = value;
        }
    }
    private bool infected;
    public bool Infected
    {
        get { return infected; }
        set
        {
            if (value)
            {
                //Change color
                //Particle Effect
                gameManager.GetComponent<AudioSource>().Play();
                GameObject par = (GameObject)Instantiate(Resources.Load("InfectedParticle"), this.transform.position, Quaternion.identity);
                par.transform.SetParent(this.transform);
                recoverTime = Random.Range(10f, 45f);
                GetComponentInChildren<SpriteRenderer>().color = Color.red;
            }
            infected = value;
        }
    }

    public bool Quarantining { get; set; }
    private bool recovered;
    public bool Recovered
    {
        get { return recovered; }
        set
        {
            if (value)
            {
                //Change color
                GameObject par = (GameObject)Instantiate(Resources.Load("RecoveredEffect"), this.transform.position, Quaternion.identity);
                GetComponentInChildren<SpriteRenderer>().color = Color.yellow;
            }
            recovered = value;
        }
    }

    public GameObject home { get; set; }
    public GameManager gameManager { get; set; }
    public bool Moving { get; set; }
    public bool SocialDistancing { get; set; }
    public bool CloseToDest => closeToDest;
    public bool InHouse => inHouse;

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
    private bool goToHosptial;
    
    // Start is called before the first frame update
    public void Start()
    {
        moveTimer = 0;
        coolDown = Random.Range(1.0f, 5.0f);
        Moving = false;
        inHouse = true;
        rb = GetComponent<Rigidbody2D>();
        speed = 5f;
        closeToDest = true;
        SocialDistancing = false;
        HasMask = false;
        destinationRadius = gameManager.SeperationDistance;
        infected = false;
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
            if(!Quarantining || !inHouse)
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
                if(Random.Range(0,1.0f) < GetDeathChance())
                {
                    //DEAD
                    if (Recovered)
                        gameManager.NumRecovered--;
                    home.GetComponent<House>().RemovePerson(gameObject);
                    gameManager.KillPerson(gameObject);
                }
                // Recovered
                else
                {
                    gameManager.NumInfected--;
                    if(!Recovered)
                        gameManager.NumRecovered++;
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
                // Is the person going to the hospital?
                if (gameManager.HospitalPositions.Contains(destination))
                {
                    goToHosptial = true;
                }
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
            if (goToHosptial && infected)
            {
                home.GetComponent<House>().QuarantineResidents();
            }
            goToHosptial = false;
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

    public float GetDeathChance()
    {
        float deathChance = 0;
        if (Age <= 9)
            deathChance = 0.1f / 100.0f;
        else if (Age <= 19)
            deathChance = 0.1f / 100.0f;
        else if (Age <= 29)
            deathChance = 0.1f / 100.0f;
        else if (Age <= 39)
            deathChance = 0.4f / 100.0f;
        else if (Age <= 49)
            deathChance = 1.0f / 100.0f;
        else if (Age <= 59)
            deathChance = 2.4f / 100.0f;
        else if (Age <= 69)
            deathChance = 6.7f / 100.0f;
        else if (Age <= 79)
            deathChance = 16.6f / 100.0f;
        else
            deathChance = 28.7f / 100.0f;

        if (Sex == Sex.Male)
            deathChance *= 1.125f;
        else
            deathChance *= 0.9f;

        return deathChance;
    }
}
