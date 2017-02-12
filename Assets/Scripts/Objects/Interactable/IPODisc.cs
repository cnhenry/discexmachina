using UnityEngine;
using System.Collections;

public class IPODisc : InteractablePhysicalObject
{
    //public float spinVelocity = 0.0f;
    //public float curveAmount = 5.0f;
        public Rigidbody rb;
        float xAxis, zAxis;
        bool lockedState, xzLock;

        //Old Code
        public float angularVelocityReadout, velocity, xLock, yLock, zLock;

        // vars from paper
        float vX, vY, cl, cd, deltavx, deltavy;

        //Read out variables.
        public float angleOfAttack, velocityReadout;
        public float g = -9.81f, thresholdVelocity = 5, airDensity = 1.23f, areaOfFrisbee = 0.0568f, xChange, yChange, zChange;
        static float CLO = 0.1f, CLA = 1.4f, CDO = 0.08f, CDA = 2.72f, ALPHAO = -4f;//, RHO = 1.23f, AREA = 0.0568f;
        //public float angularVelocity, velocity, xLock, yLock, zLock;

    // Use this for initialization


        void Start() {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update() {
            velocityReadout = rb.velocity.magnitude;
            angularVelocityReadout = rb.angularVelocity.magnitude;
            angleOfAttack = Mathf.Rad2Deg * Mathf.Atan((rb.velocity.magnitude - rb.velocity.y) / rb.velocity.y); //alpha = tan^-1(w/u);

            if (beingInteracted == false)
            {
                if (rb.angularVelocity.magnitude > thresholdVelocity)
                {
                    if (lockedState != true)
                    {
                        lockedState = true;
                        Debug.Log("Entering lock, physics applied");
                    }

                    transform.rotation = Quaternion.Euler(xLock, yLock, transform.rotation.eulerAngles.z);

                    //Only want to act if velocity is beyond threshold, without gravity since we are calculating the physics.
                    rb.useGravity = false;

                    //Velocity in horizontal component
                    vX = (rb.velocity.magnitude - rb.velocity.y);
                    vY = rb.velocity.y;

                    cl = CLO + CLA * angleOfAttack * Mathf.Deg2Rad;
                    cd = CDO + CDA * Mathf.Pow((angleOfAttack - ALPHAO) * Mathf.Deg2Rad, 2);

                    //Lift force + Gravitational Force = Velocity in y-axis
                    deltavy = (airDensity * Mathf.Pow(vY, 2) * areaOfFrisbee * (cl / 2 / rb.mass) + g) * Time.deltaTime;

                    //Change in the horizontal direction, force equasion
                    deltavx = -airDensity * Mathf.Pow(vX, 2) * areaOfFrisbee * cd * Time.deltaTime;

                    xAxis = deltavx * Vector3.Dot(rb.velocity.normalized, Vector3.right);
                    zAxis = deltavx * Vector3.Dot(rb.velocity.normalized, Vector3.forward);

                    xChange = xAxis;
                    zChange = zAxis;
                    yChange = deltavy;

                    rb.AddForce(xAxis, deltavy, zAxis);

                }
                else
                {
                    //Otherwise let general physics engine take over
                    if (lockedState != false)
                    {
                        lockedState = false;
                        Debug.Log("Outside of lock and physics");
                    }

                    rb.useGravity = true;
                }

                /*
                //Old code...
                velocity = rb.velocity.magnitude;
                angularVelocity = rb.angularVelocity.magnitude;
                if ( rb.velocity.magnitude + rb.angularVelocity.magnitude >= 10.0f) { //Lock axis on thresholds

                } //Else the physics engine take care of it 

                */
            }
            else
            { //Configure the lock angles so long as the object is not released
                //Do nothing...
                //rb.useGravity = true;

                //Old code...
                xLock = transform.rotation.eulerAngles.x;
                yLock = transform.rotation.eulerAngles.y;
                zLock = transform.rotation.eulerAngles.z;
            }
        }

    void FixedUpdate() {
        if (beingInteracted == false)
        {
            Rigidbody rb = GetComponent<Rigidbody>();
        }
    }
}