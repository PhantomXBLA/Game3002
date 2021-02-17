using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BallBehaviourScript : MonoBehaviour
{

    bool activated = false;

    //Values for the horizontal angle the player lands on
    public float maxleftRot = -90;
    public float maxrightRot = 90;
    public float temphorizontalRot = 0;
    float selectedhorizontalRot = 0;
    float horizontalrotchange = 0.25f;

    //Values for the vertical angle the player lands on
    public float maxdownRot = -10;
    public float maxupRot = 70;
    public float tempverticalRot = 0;
    float selectedverticalRot = 0;
    float verticalrotchange = 0.25f;

    //Variables for the power of the kick
    float kickPower = 500;
    public float tempPower = 500;
    public float minkickPower = 500;
    public float maxkickPower = 1500;

    //Variable for which phase the player is in
    string phase = "rotation";

    //Setting the net so I can access it's properties later
    GameObject NetObject;


    //Setting initial velocity and distance from ball to the landing target (net)
    [SerializeField]
    Vector3 initialVelocity;
    float distancetoTarget = 0f;

    //Final rotation values which ball will be launched at
    float finalrotY;
    float finalrotX;
    float finalrotZ;

    Vector3 spawnPos; // the original position for the ball for resetting purposes
        //(22.82355f, -3.49f, 10.3596f);

    
    GameObject textVariables; //Allows me to add variables as UI text
    Text textVariablesComponent; //Actual text component for said UI

    GameObject goaltriggerComponent; //Allows me to call function from the trigger component

    // Start is called before the first frame update
    void Start()
    {
        NetObject = GameObject.Find("Net");
        distancetoTarget = (NetObject.transform.position - transform.position).magnitude; //Gets the distance from ball to net

        textVariables = GameObject.Find("HorizontalUIAngle");
        textVariablesComponent = textVariables.GetComponent<Text>();
        spawnPos = GetComponent<Rigidbody>().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == "rotation")
        {
            if (temphorizontalRot != maxrightRot && selectedhorizontalRot != maxrightRot) //This is what makes the ball rotate to the right
            {
                temphorizontalRot += 0.5f; //adds 0.5f to the temp rotate variable which rotates it right
                initialVelocity.x -= (0.25f);//sets the velocity.x to correspond to the rotation, so it will launch to the x position
            }

            else if (temphorizontalRot >= maxrightRot)
            {
                selectedhorizontalRot = temphorizontalRot;
            }

            if (selectedhorizontalRot >= maxrightRot && temphorizontalRot >= maxleftRot) //when the ball reaches the maximum amount of right rotation, it will start rotating left
            {
                temphorizontalRot -= 0.5f; //subtracts 0.5f to the temp rotate variable which rotates it right
                initialVelocity.x += (0.25f); //sets the velocity.x to correspond to the left rotation, so it will launch to the x position
            }

            else if (temphorizontalRot <= maxleftRot && selectedhorizontalRot >= maxrightRot)
            {
                selectedhorizontalRot = maxleftRot;
            }
        }

        if(phase == "height")
        {
            if (tempverticalRot != maxupRot && selectedverticalRot != maxupRot) //This is what makes the ball rotate up
            {
                tempverticalRot += 0.25f;
                initialVelocity.y += (0.25f); //sets the velocity.y to correspond to the up rotation, so it will launch to the y position (basically it will go up according to the angle)
            }

            else if (tempverticalRot >= maxupRot)
            {
                selectedverticalRot = tempverticalRot;
            }

            if (selectedverticalRot >= maxupRot && tempverticalRot >= maxdownRot) //when the ball reaches the maximum amount of up rotation, it will start rotating down
            {
                tempverticalRot -= 0.25f;
                initialVelocity.y -= (0.25f); //sets the velocity.y to correspond to the down rotation, so it will launch to the y position (basically it will go down according to the angle)
            }

            else if (tempverticalRot <= maxdownRot && selectedverticalRot >= maxupRot)
            {
                selectedverticalRot = maxdownRot;
            }
        }

        if(phase == "power")
        {
            if(tempPower != maxkickPower && kickPower != maxkickPower) //if the power level of the kick isn't max, add to it
            {
                tempPower += 2;
            }
            
            else if (tempPower >= maxkickPower)
            {
                kickPower = tempPower;
            }

            if (kickPower >= maxkickPower && tempPower >= minkickPower) //if it is maxed, subtract from it (this gives it a variable value to keep gameplay more interesting)
            {
                tempPower -= 2;
            }

            else if(tempPower <= minkickPower && kickPower >= maxkickPower)
            {
                kickPower = minkickPower;
            }
        }

        if (Input.GetKeyDown("space")) //advances the phase
        {
            if(activated == false)
            {
                nextPhase();
            }
        }

        if (Input.GetKeyDown("r"))
        {
                resetBall(); //resets the game 
        }

        transform.rotation = Quaternion.Euler(tempverticalRot, temphorizontalRot, finalrotZ); //updates the rotation of the ball constantly to correspond with the values set above
        //Debug.Log("Phase: " + phase + " / Temp Horizontal Rotation: " + temphorizontalRot + " / Horizontal Rotation: " + selectedhorizontalRot + " / Temp Vertical Rotation: " + tempverticalRot + " / Vertical Rotation: " + selectedverticalRot + " / Temp Power: " + tempPower + " / Kick Power:" + kickPower);
        
        textVariablesComponent.text = "Horizontal Angle: " + temphorizontalRot.ToString("F0") + "\nVertical Angle: " + tempverticalRot.ToString("F0") + "\nKick Force: " +tempPower.ToString("F0");
    }

    void nextPhase()
    {
        if(phase == "rotation")
        {
            selectedhorizontalRot = temphorizontalRot;
            finalrotY = selectedhorizontalRot;
            phase = "height";
        }

       else if (phase == "height")
        {
            selectedverticalRot = tempverticalRot;
            finalrotX = selectedverticalRot;
            phase = "power";
        }

        else
        {
            kickPower = tempPower;
            phase = "launch";

            launchBall(); //launches the ball

        }
    }

    void launchBall()
    {
        initialVelocity.z += (-kickPower / 50); //adds the kick power to the z velocity so it can go forward, I had to divide it by 50 to get a realistic launch value.
        initialVelocity.y /= 5; //dividing the velocity.y value because it would often go too high
        GetComponent<Rigidbody>().velocity = initialVelocity; //applies the velocity to the ball
        activated = true;
    }

    void OnTriggerEnter(Collider other)
    {
        bool goal = true;
        goaltriggerComponent = GameObject.Find("GoalTrigger");
        if (goal == true)
        {
            goaltriggerComponent.GetComponent<AudioBehaviour>().PlayYay(); //plays the winning sound effect
            goal = false;
        }

        Invoke("resetBall", 3); //call the resetball 3 seconds after passing the goal trigger
    }

    void resetBall()
    {
        SceneManager.LoadScene("SceneOne"); //resets the scene when called
    }
}
