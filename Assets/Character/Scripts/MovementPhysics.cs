using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PhysicalStatus{
    OnGround = 1,
    OnAir = 2,
    OnWall = 3,
}

enum PhysicalSubStatus
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

    Vector3 selfPos => transform.position;

    // PHYSICAL STATUS
    [Header("Physical Status")]
    [SerializeField] PhysicalStatus physicsStat = PhysicalStatus.OnAir;
    PhysicalStatus phyStatPrev = PhysicalStatus.OnAir;

    [SerializeField] PhysicalSubStatus phySubStat = PhysicalSubStatus.AirTop;

    [SerializeField] LayerMask groundLayer;
    bool onGround = false;
    Vector3 groundCheckOffset;
    float groundCheckRad;

    bool phyStatChanged = false;

    // WALK
    Vector3 planeVel;
    [Header("Walk")]
    [SerializeField] float planeMoveForceCoeff;
    float xMoveForce;
    float zMoveForce;
    [SerializeField] float planeSpeedLimitDefault;
    float planeSpeedLimit;
    float xSpeedLimit;
    float zSpeedLimit;
    [SerializeField] float airMoveConst;
    [SerializeField] float runConst;
    float moveCoeff;

    sbyte xBrakeCoeff;
    sbyte zBrakeCoeff;

    //JUMP
    [Header("Jump")]
    [SerializeField] float jumpHeight;
    float jumpSpeed;
    public float fallGravity;
    [SerializeField] float verSpeed => rb.velocity.y;

    private void Awake()
    {
        rb = transform.GetComponent<Rigidbody>();
        mb = transform.GetComponent<MovementBasics>();
        bodyCol = transform.Find("Body").GetComponent<CapsuleCollider>();

        groundCheckOffset = new Vector3(0, -bodyCol.height / 2, 0);
        groundCheckRad = bodyCol.radius + 0.15f;

        jumpSpeed = Mathf.Sqrt(2 * -Physics.gravity.y * jumpHeight);
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        PhysicalStatusUpdate();

        PlaneMovement();
        AutoBrake();

        BrickFall();
    }

    void PhysicalStatusUpdate()
    {

        onGround = Physics.CheckSphere(selfPos + groundCheckOffset, groundCheckRad, groundLayer);
        if (onGround) {
            physicsStat = PhysicalStatus.OnGround;

            moveCoeff = 1;

            // SUBSTATUS
            if (mb.moveDirect.magnitude != 0)
            {
                if (mb.runInp) { phySubStat = PhysicalSubStatus.GrRun; }
                else { phySubStat = PhysicalSubStatus.GrWalk; }
            }
            else { phySubStat = PhysicalSubStatus.GrIdle;  }

        }
        else {
            physicsStat = PhysicalStatus.OnAir;

            moveCoeff = airMoveConst;

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
        phyStatChanged = (physicsStat != phyStatPrev) ? true : false;
        phyStatPrev = physicsStat;
    }

    // SHOULD BE OPTIMIZED
    void PlaneMovement()
    {
        planeVel = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        if (physicsStat == PhysicalStatus.OnAir) { moveCoeff = airMoveConst; }
        else if (mb.runInp) { moveCoeff = runConst; }
        else { moveCoeff = 1; }

        planeSpeedLimit = planeSpeedLimitDefault * moveCoeff;

        if (mb.moveDirect.magnitude != 0)
        {
            xSpeedLimit = planeSpeedLimit * Mathf.Abs(mb.moveDirect.x);
            xMoveForce = planeMoveForceCoeff * (xSpeedLimit - Mathf.Abs(planeVel.x));
            if (Mathf.Abs(planeVel.x) < xSpeedLimit) {
                rb.AddForce(Vector3.right * mb.moveDirect.x * xMoveForce);
            }

            zSpeedLimit = planeSpeedLimit * Mathf.Abs(mb.moveDirect.z);
            zMoveForce = planeMoveForceCoeff * (zSpeedLimit - Mathf.Abs(planeVel.z));
            if (Mathf.Abs(planeVel.z) < zSpeedLimit) {
                rb.AddForce(Vector3.forward * mb.moveDirect.z * zMoveForce);
            }
        }
    }

    void AutoBrake()
    {
        if (Mathf.Abs(planeVel.magnitude) > 0.2f)
        {
            if (mb.moveDirect.x == 0 || mb.moveDirect.x * planeVel.x < 0
            || (planeVel.x > xSpeedLimit && physicsStat == PhysicalStatus.OnGround))
            { rb.AddForce(new Vector3(-10 * planeVel.x, 0, 0)); Debug.Log("[AUTO BRAKE X ]"); }
            if (mb.moveDirect.z == 0 || mb.moveDirect.z * planeVel.z < 0
                || (planeVel.z > zSpeedLimit && physicsStat == PhysicalStatus.OnGround))
            { rb.AddForce(new Vector3(0, 0, -10 * planeVel.z)); Debug.Log("[AUTO BRAKE Z ]");}

            
        }
    }

    //void AutoBrake()
    //{
    //    if (mb.moveDirect.x == 0 || mb.moveDirect.x * planeVel.x < 0
    //        || (planeVel.x > xSpeedLimit && physicsStat == PhysicalStatus.OnGround)) { xBrakeCoeff = -10; }
    //    else { xBrakeCoeff = 0; }
    //    if (mb.moveDirect.z == 0 || mb.moveDirect.z * planeVel.z < 0
    //        || (planeVel.z > zSpeedLimit && physicsStat == PhysicalStatus.OnGround)) { zBrakeCoeff = -10; }
    //    else { zBrakeCoeff = 0; }

    //    if (Mathf.Abs(planeVel.magnitude) > 0.2f)
    //    {
    //        rb.AddForce(new Vector3(xBrakeCoeff * planeVel.x, 0, zBrakeCoeff * planeVel.z));
    //        Debug.Log("[AUTO BRAKE]");
    //    }
    //}

    void Jump()
    {
        if(physicsStat.Equals(PhysicalStatus.OnGround) && mb.jumpInp == 1)
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpSpeed, rb.velocity.z);
        }
    }

    void BrickFall()
    {
        if(physicsStat == PhysicalStatus.OnAir) {
            if (mb.jumpInp == -1 || phySubStat != PhysicalSubStatus.AirUp ) { rb.AddForce(new Vector3(0, Physics.gravity.y, 0) * fallGravity * rb.mass); }
        }
    }

}
