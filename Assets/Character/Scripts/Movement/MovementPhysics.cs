using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PhysicalStatus{
    OnGround = 1,
    OnAir = 2,
}

public enum PhysicalSubStatus
{
    GrIdle = 11,
    GrWalk = 12,
    GrRun = 13,

    AirUp = 21,
    AirTop = 22,
    AirDown = 23,
}

public class MovementPhysics : MonoBehaviour
{
    Rigidbody rb;

    MovementBasics mb;

    CapsuleCollider bodyCol;

    SphereCollider detectionCol;

    Vector3 selfPos => transform.position;

    // PHYSICAL STATUS
    [Header("Physical Status")]
    public PhysicalStatus phyStat = PhysicalStatus.OnAir;
    PhysicalStatus phyStatPrev = PhysicalStatus.OnAir;

    public PhysicalSubStatus phySubStat = PhysicalSubStatus.AirTop;

    [SerializeField] LayerMask groundLayer;
    bool onGround = false;
    Vector3 groundCheckOffset;
    float groundCheckRad;

    bool phyStatChanged = false;

    // WALK
    [SerializeField] Vector3 moveDir;
    [Header("Walk")]
    [SerializeField] float moveForceCoeff;
    [SerializeField] float speedLimitDefault;
    float moveForce;
    float speedLimit;
    [SerializeField] float airMoveConst;
    [SerializeField] float runConst;
    float moveCoeff;

    Vector3 xzVelocity; 

    [SerializeField] Vector3 brakeDir;

    RaycastHit groundHit;
    Vector3 groundNormal;
    float moveDirectY;

    //JUMP
    [Header("Jump")]
    [SerializeField] float jumpHeight;
    float jumpSpeed;
    public float fallGravity;
    [SerializeField] float verSpeed => rb.velocity.y;

    [Header("Detection")]
    //Detection
    [SerializeField] float detectionDist;
    [SerializeField] float seeDist;
    public float detectionDistance => detectionDist;
    public float seeDistance => seeDist;

    //ROTATE
    Quaternion bodyRotation => bodyCol.transform.rotation;
    Vector3 faceDir;
    Quaternion faceRotation;

    //GRAVITY (DEL)
    float gravMag;
    
    [SerializeField] Vector3 gravDirect;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        mb = transform.GetComponent<MovementBasics>();
        bodyCol = transform.Find("Body").GetComponent<CapsuleCollider>();
        detectionCol = transform.Find("DetectionCol").GetComponent<SphereCollider>();

        groundCheckOffset = new Vector3(0, -bodyCol.height / 2 + 0.1f, 0);
        groundCheckRad = bodyCol.radius;

        jumpSpeed = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);

        detectionCol.radius = detectionDist;

        gravMag = Physics.gravity.magnitude;
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        PhysicalStatusUpdate();

        SetMoveDirect();
        PlaneMovement();
        AutoBrake();

        RotateBody();
        BrickFall();

        //GravityDirectSetter();

    }


    // ADD TARGET
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == mb.enemy)
        {
            mb.AddTarget(other.GetComponentInParent<Character>());
        }
    }


    //CHECK

    //TO PREVENT SLIDING ON SLOPE (OR HAVING SNAP TO GROUND EFFECT)

    //void GravityDirectSetter()
    //{

    //    if (phyStat.Equals(PhysicalStatus.OnGround)) {
    //        gravDirect = gravMag * -1 * groundNormal.normalized;
    //    }
    //    else {
    //        gravDirect = gravMag * Vector3.down;
    //    }

    //    if(Physics.gravity != gravDirect)
    //    {
    //        Physics.gravity = gravDirect;
    //    }

    //}


    void PhysicalStatusUpdate()
    {

        onGround = Physics.CheckSphere(selfPos + groundCheckOffset, groundCheckRad, groundLayer);
        if (onGround) {
            phyStat = PhysicalStatus.OnGround;

            // SUBSTATUS
            if (moveDir.magnitude != 0)
            {
                if (mb.runInp) { phySubStat = PhysicalSubStatus.GrRun; }
                else { phySubStat = PhysicalSubStatus.GrWalk; }
            }
            else { phySubStat = PhysicalSubStatus.GrIdle;  }

        }
        else {
            phyStat = PhysicalStatus.OnAir;

            //SUBSTATUS
            if(verSpeed > 4) { phySubStat = PhysicalSubStatus.AirUp; }
            else if (verSpeed < -4) { phySubStat = PhysicalSubStatus.AirDown; }
            else { phySubStat = PhysicalSubStatus.AirTop; }
        }

        // STATUS IS CHANGED CHECK
        PhyStatChangeDetector();

    }

    void PhyStatChangeDetector()
    {
        phyStatChanged = (phyStat != phyStatPrev) ? true : false;
        phyStatPrev = phyStat;
    }

    void SetMoveDirect()
    {
        if (mb.moveDirect.magnitude == 0) {
            moveDir = Vector3.zero;
        }
        else {
            Physics.Raycast(selfPos + groundCheckOffset, Vector3.down, out groundHit, groundCheckRad, groundLayer);
            groundNormal = groundHit.normal;
            moveDirectY = (Quaternion.AngleAxis(90, bodyCol.transform.right) * groundNormal).y;
            moveDir = (mb.moveDirect + (moveDirectY * Vector3.up)).normalized;
        }
    }

    void PlaneMovement()
    {
        if (phyStat == PhysicalStatus.OnAir) { moveCoeff = airMoveConst; }
        else if (mb.runInp) { moveCoeff = runConst; }
        else { moveCoeff = 1; }

        speedLimit = speedLimitDefault * moveCoeff;

        if(moveDir.magnitude != 0)
        {
            moveForce = moveForceCoeff * (speedLimit - Mathf.Abs(rb.velocity.magnitude));
            if (Mathf.Abs(rb.velocity.magnitude) < speedLimit)
            {
                rb.AddForce(moveDir * rb.mass * moveForce * moveCoeff);
            }
        }

        xzVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

    }

    void AutoBrake()
    {

        if (phyStat.Equals(PhysicalStatus.OnGround))
        {
            if (moveDir.magnitude == 0)
            {
                brakeDir = -1 * rb.velocity.normalized;
            }
            else
            {
                brakeDir = -1 * (rb.velocity.normalized - moveDir).normalized;
            }

            if (Mathf.Abs((rb.velocity.normalized - moveDir).magnitude) > 0.2f)
            {
                rb.AddForce(brakeDir * rb.mass * 40);
            }
        }
        else if (phyStat.Equals(PhysicalStatus.OnAir))
        {
            if (Mathf.Abs((xzVelocity.normalized - mb.moveDirect).magnitude) > 0.2f)
            {
                brakeDir = -1 * (xzVelocity.normalized - mb.moveDirect).normalized;
                rb.AddForce(brakeDir * rb.mass * 20);
            }
        }
    }

    void Jump()
    {
        if(phyStat.Equals(PhysicalStatus.OnGround) && mb.jumpInp == 1)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }
    }

    void BrickFall()
    {
        if (phyStat == PhysicalStatus.OnAir
            && (mb.jumpInp == -1 || phySubStat != PhysicalSubStatus.AirUp)){
            rb.AddForce(new Vector3(0, Physics.gravity.y, 0) * fallGravity * rb.mass);
        }


    }

    void RotateBody()
    {

        if(mb.moveDirect.magnitude != 0)
        {
            faceDir = mb.moveDirect;
        }
        if(faceDir.magnitude != 0)
        {
            faceRotation = Quaternion.LookRotation(faceDir);
        }
        
        bodyCol.transform.rotation = Quaternion.Slerp(bodyRotation, faceRotation, 0.2f);

    }

       

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(transform.position, seeDist);
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, detectionDist);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawLine(transform.position, transform.position + brakeDir * 3);

    }

}
