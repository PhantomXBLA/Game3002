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

    GameObject NetObject;

    [SerializeField]
    Vector3 initialVelocity;
    float distancetoTarget = 0f;

    //Final rotation values which ball will be launched at
    float finalrotY;
    float finalrotX;
    float finalrotZ;

    Vector3 spawnPos;
        //(22.82355f, -3.49f, 10.3596f);

    
    GameObject textVariables;
    Text textVariablesComponent;

    GameObject goaltriggerComponent;

    // Start is called before the first frame update
    void Start()
    {
        NetObject = GameObject.Find("Net");
        distancetoTarget = (NetObject.transform.position - transform.position).magnitude;

        textVariables = GameObject.Find("HorizontalUIAngle");
        textVariablesComponent = textVariables.GetComponent<Text>();
        spawnPos = GetComponent<Rigidbody>().transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (phase == "rotation")
        {
            if (temphorizontalRot != maxrightRot && selectedhorizontalRot != maxrightRot)
            {
                temphorizontalRot += 0.5f;
                initialVelocity.x -= (0.25f);
            }

            else if (temphorizontalRot >= maxrightRot)
            {
                selectedhorizontalRot = temphorizontalRot;
            }

            if (selectedhorizontalRot >= maxrightRot && temphorizontalRot >= maxleftRot)
            {
                temphorizontalRot -= 0.5f;
                initialVelocity.x += (0.25f);
            }

            else if (temphorizontalRot <= maxleftRot && selectedhorizontalRot >= maxrightRot)
            {
                selectedhorizontalRot = maxleftRot;
            }
        }

        if(phase == "height")
        {
            if (tempverticalRot != maxupRot && selectedverticalRot != maxupRot)
            {
                tempverticalRot += 0.25f;
                initialVelocity.y += (0.25f);
            }

            else if (tempverticalRot >= maxupRot)
            {
                selectedverticalRot = tempverticalRot;
            }

            if (selectedverticalRot >= maxupRot && tempverticalRot >= maxdownRot)
            {
                tempverticalRot -= 0.25f;
                initialVelocity.y -= (0.25f);
            }

            else if (tempverticalRot <= maxdownRot && selectedverticalRot >= maxupRot)
            {
                selectedverticalRot = maxdownRot;
            }
        }

        if(phase == "power")
        {
            if(tempPower != maxkickPower && kickPower != maxkickPower)
            {
                tempPower += 2;
            }
            
            else if (tempPower >= maxkickPower)
            {
                kickPower = tempPower;
            }

            if (kickPower >= maxkickPower && tempPower >= minkickPower)
            {
                tempPower -= 2;
            }

            else if(tempPower <= minkickPower && kickPower >= maxkickPower)
            {
                kickPower = minkickPower;
            }
        }

        if (Input.GetKeyDown("space"))
        {
            if(activated == false)
            {
                nextPhase();
            }
        }

        if (Input.GetKeyDown("r"))
        {
                resetBall();
        }

        transform.rotation = Quaternion.Euler(tempverticalRot, temphorizontalRot, finalrotZ);
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

            launchBall();

        }
    }

    private Vector3 GetLandingPosition()
    {
        float fTime = 2f * (0f - initialVelocity.y / Physics.gravity.y);
        Vector3 vFlatVel = initialVelocity;
        vFlatVel.y = 0f;
        vFlatVel *= fTime;
        return transform.position + vFlatVel;
    }


    void launchBall()
    {
        initialVelocity.z += (-kickPower / 50);
        initialVelocity.y /= 5;
        GetComponent<Rigidbody>().velocity = initialVelocity;
        activated = true;
    }

    void OnTriggerEnter(Collider other)
    {
        bool goal = true;
        goaltriggerComponent = GameObject.Find("GoalTrigger");
        if (goal == true)
        {
            goaltriggerComponent.GetComponent<AudioBehaviour>().PlayYay();
            goal = false;
        }

        Invoke("resetBall", 3);
    }

    void resetBall()
    {
        SceneManager.LoadScene("SceneOne");
    }
}
