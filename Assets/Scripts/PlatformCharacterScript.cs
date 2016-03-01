using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

using UnityStandardAssets.CrossPlatformInput;

public class PlatformCharacterScript : NetworkBehaviour {

    //TODO überschreibt vererbung, zum cachen
    new public Transform transform;
    public Rigidbody2D rb2d;
    [SerializeField]
    PlatformUserControl inputScript;

    [SerializeField]
    ParticleSystem frictionSmoke = null;

    [SyncVar] [SerializeField]
    public bool grounded = false;

//    bool walled = false;
    public Vector2 wallCheckPosition = new Vector2(0.5f, 0); // Position, where the the Wall will be checked

    [SerializeField]
    bool beamEnabled = true;

    public Animator anim;                                   // Animator State Machine
    [SyncVar(hook = "OnChangeDirection")]
    public bool facingRight = true;                         // keep DrawCalls low, Flip textures scale: texture can be used for both directions 
    public bool changedRunDirection = false;
	public AudioClip changeRunDirectionSound;	// Skid Sound
    public AudioClip jumpSound;					// Jump Sound



    private bool useUnityPhysics = true;
    private float rigibodyMoveForce = 40f; // 7
    private float rigibodyJumpForce = 800f; // 7
    private float rigibodyMaxSpeed = 7.5f; // 7

    public Vector3 moveDirection = Vector3.zero;

    Collider2D[] foundColliderArray = new Collider2D[2];

    [SerializeField]
    private BoxCollider2D bodyCollider2D;
    public BoxCollider2D BodyCollider2D { get {return bodyCollider2D; } set { bodyCollider2D = value; } }
    [SerializeField]
    BoxCollider2D myGroundStopperCollider;
    public BoxCollider2D GroundStopperCollider { get {return myGroundStopperCollider; } set { myGroundStopperCollider = value; } }
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    public SpriteRenderer SpriteRenderer { get { return spriteRenderer; } set { spriteRenderer = value; } }



    void Awake ()
    {
		this.transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        AnimatorScriptsInitialisierung();
		myGroundStopperCollider = transform.Find(TagManager.Instance.name_groundStopper).GetComponent<BoxCollider2D>();
        bodyCollider2D = transform.FindChild(TagManager.Instance.name_body).GetComponent<BoxCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();

        inputScript = GetComponent<PlatformUserControl>();
    }

    void AnimatorScriptsInitialisierung()
    {
        SpawnStateScript spawnScript = anim.GetBehaviour<SpawnStateScript>();
        if (spawnScript != null)
        {
            spawnScript.myCharacter = this;
        }
        else
            Debug.LogError("spawnScript not found!!!");

        SpawnDelayStateScript spawnDelayScript = anim.GetBehaviour<SpawnDelayStateScript>();
        if (spawnDelayScript != null)
        {
            spawnDelayScript.myCharacter = this;
        }
        else
            Debug.LogError("spawnDelayScript not found!!!");
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        
    }

    void ReadInput ()
    {
        
    }

    void FixedUpdate ()
    {
        Simulate();
    }


    void Simulate ()
    {
        // Move (need check position ??)
        SimulateMovementWithRigidbodyPhysics();

        // Check Beam 
        CheckBeam();

        // Check new Position for Animation
        CheckPosition();

        // Set Movementanimation Flags
        SimulateAnimation();
    }

    void SimulateMovementWithRigidbodyPhysics()
    {
        //TODO
        // NEW
        //TODO
        //moveDirection.x = CrossPlatformInputManager.GetAxis ("Horizontal");
        moveDirection.x = inputScript.GetInputHorizontal ();
        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (moveDirection.x * rb2d.velocity.x < rigibodyMaxSpeed)
            // ... add a force to the player.
            rb2d.AddForce(Vector2.right * moveDirection.x * rigibodyMoveForce);

        if (grounded && (rb2d.velocity.y <= 0.1f))
        {
            //if (CrossPlatformInputManager.GetButton("Jump"))
            if (inputScript.inputJump)
            {
                SyncJump();
                moveDirection.y = rigibodyJumpForce;

                // remove current forces
                rb2d.velocity = new Vector2(rb2d.velocity.x, 0f);
                // Add a vertical force to the player.
                rb2d.AddForce(new Vector2(0f, rigibodyJumpForce));
                moveDirection.y = 0f;
            }
        }
    }

    public void SyncJump()
    {
        // Do Jump
        if (jumpSound != null)
            AudioSource.PlayClipAtPoint(jumpSound, transform.position, 1);              //JumpSound
        else
            Debug.LogError("jumpSound nicht gesetzt!");
        if (anim == null)
        {
            Debug.LogError("Animator not set");
        }
        else
        {
            anim.SetBool(HashID.groundedBool, false);
        }
    }

    void CheckBeam()
    {
        if (!beamEnabled)
            return;

        //playerPos spriterenderer boundaries
        //Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);
        if (useUnityPhysics)
            playerPosition = rb2d.position;
        else
            playerPosition = transform.position;

        // Beam
        // 0.5 = half player size (pivot.x)
        // if players pos < leftborder+0.5
        // beam to rightborder-0.5
        if (playerPosition.x < -10.5f)
        {
            playerPosition.x += 20f;
        }
        else if (playerPosition.x > 10.5f)
        {
            playerPosition.x -= 20f;
        }

        if (playerPosition.y < -7.5f)
        {
            playerPosition.y += 15f;
        }
        else if (playerPosition.y > 7.5f)
        {
            playerPosition.y -= 15f;
        }

        if (useUnityPhysics)
            rb2d.position = playerPosition;
        else
            transform.position = playerPosition;
    }

    enum Edge
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }

    Vector3 GetColliderEdgeWorldPosition (Vector3 goPosition, BoxCollider2D boxCollider, Edge edge)
    {
        Vector3 edgeWorldPos = Vector3.zero;
        switch (edge)
        {
            case Edge.TopLeft:
                //edgeWorldPos.x = goPosition.x - boxCollider.size.x * 0.5f + boxCollider.offset.x;
                //edgeWorldPos.y = goPosition.y + boxCollider.size.y * 0.5f + boxCollider.offset.y;
                edgeWorldPos.x = boxCollider.bounds.max.x - boxCollider.bounds.size.x;
                edgeWorldPos.y = boxCollider.bounds.max.y;
                break;
            case Edge.TopRight:
                //edgeWorldPos.x = goPosition.x + boxCollider.size.x * 0.5f + boxCollider.offset.x;
                //edgeWorldPos.y = goPosition.y + boxCollider.size.y * 0.5f + boxCollider.offset.y;
                edgeWorldPos.x = boxCollider.bounds.max.x;
                edgeWorldPos.y = boxCollider.bounds.max.y;
                break;
            case Edge.BottomLeft:
                //edgeWorldPos.x = goPosition.x - boxCollider.size.x * 0.5f + boxCollider.offset.x;
                //edgeWorldPos.y = goPosition.y - spriteRenderer.bounds.extents.y * 1.2f; 
                edgeWorldPos.x = boxCollider.bounds.min.x;
                edgeWorldPos.y = boxCollider.bounds.min.y;
                break;
            case Edge.BottomRight:
                //edgeWorldPos.x = goPosition.x + boxCollider.size.x * 0.5f + boxCollider.offset.x;
                //edgeWorldPos.y = goPosition.y - spriteRenderer.bounds.extents.y * 1.2f; 
                edgeWorldPos.x = boxCollider.bounds.min.x + boxCollider.bounds.size.x;
                edgeWorldPos.y = boxCollider.bounds.min.y;
                break;
            default:

                break;
        }
        return edgeWorldPos;
    }

    //bool useCustomPlatformJumperScript = false;

    Vector2 playerPosition;
    Vector2 playerColliderTopLeftPos;
    Vector2 playerColliderBottomRightPos;
#if UNITY_EDITOR
    Vector2 playerColliderTopRightPos;
    Vector2 playerColliderBottomLeftPos;
#endif
    Vector2 playerColliderOffset = new Vector2(0.1f, 0.0f);     // FIX player jump at wall and get grounded

    void OnDrawGizmos()
    {
		if (myGroundStopperCollider == null)
			return;
		
        Vector3 center = myGroundStopperCollider.bounds.center;
        Vector3 size = myGroundStopperCollider.size;
        WireCube(center, size, Color.red);

        // BottomLeft
        //center = myGroundStopperCollider.bounds.min;
        //size = myGroundStopperCollider.size;
        //WireCube(center, size, Color.yellow);

        // TopRight
        //center = myGroundStopperCollider.bounds.max;
        //size = myGroundStopperCollider.size;
        //WireCube(center, size, Color.blue);

        // Center
        // myGroundStopperCollider.bounds.center == myGroundStopperCollider.transform.position + (Vector3) myGroundStopperCollider.offset
        //center = myGroundStopperCollider.bounds.center;
        //size = myGroundStopperCollider.size;
        //WireCube(center, size, Color.yellow);
        // Center
        //center = myGroundStopperCollider.transform.position + (Vector3) myGroundStopperCollider.offset;
        //size = myGroundStopperCollider.size;
        //WireCube(center, size, Color.blue);
    }

    void WireCube (Vector3 center, Vector3 size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawWireCube(center, size);
    }

    void CheckPosition()
    {

        //Rigidbody.position allows you to get and set the position of a Rigidbody using the physics engine. If you change the position of a Rigibody using Rigidbody.position, the transform will be updated after the next physics simulation step. This is faster than updating the position using Transform.position, as the latter will cause all attached Colliders to recalculate their positions relative to the Rigidbody. 
        if (useUnityPhysics)
            playerPosition = rb2d.position;
        else
            playerPosition = transform.position;

        playerColliderTopLeftPos = GetColliderEdgeWorldPosition(playerPosition, myGroundStopperCollider, Edge.TopLeft);
        playerColliderBottomRightPos = GetColliderEdgeWorldPosition(playerPosition, myGroundStopperCollider, Edge.BottomRight);

#if UNITY_EDITOR
        playerColliderTopRightPos = GetColliderEdgeWorldPosition(playerPosition, myGroundStopperCollider, Edge.TopRight);
        playerColliderBottomLeftPos = GetColliderEdgeWorldPosition(playerPosition, myGroundStopperCollider, Edge.BottomLeft);
       
        Debug.DrawLine(playerColliderTopLeftPos + playerColliderOffset, playerColliderTopRightPos - playerColliderOffset, Color.white);
        Debug.DrawLine(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, Color.white);
        Debug.DrawLine(playerColliderTopRightPos - playerColliderOffset, playerColliderBottomLeftPos + playerColliderOffset, Color.white);
        Debug.DrawLine(playerColliderBottomLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, Color.white);
#endif

        /**
		 * check if standing on activ jumpPlatform
		 **/

        //Collider2D foundCollider = Physics2D.OverlapArea(playerColliderTopLeftPos, playerColliderBottomRightPos, jumpOnPlatform);

        bool platformGrounded = false;

        //int overlapCount = Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos, playerColliderBottomRightPos, foundColliderArray, jumpOnPlatform );
        foundColliderArray[0] = null;
		int numberOfColider = Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, foundColliderArray, LayerManager.Instance.whatIsJumpOnPlatform);

        for (int i=0; i< numberOfColider; i++)
        {
            if (foundColliderArray[0] != null)
            {
                // Collider aus JumpOnPlatform Ebene (Laye) inerhalb des definierten Bereiches gefunden
                // yellow zone collids with jumpOnPlatform

                // kontrollieren ob Kollision zwischen Platform und groundStopper ignoriert wird (Ignorierung/Berücksichtigung wird von PlatformJumperScript durchgeführt)
                // wenn Kollision ignoriert wird dann ist der Character zurzeit nicht am Fallen sondern am Springen

                if (Physics2D.GetIgnoreCollision(foundColliderArray[i], myGroundStopperCollider))
                {
                    // true => Kollision mit gefundener JumpOnPlatform wird ignoriert (deaktiviert)
                    platformGrounded = false;
                }
                else
                {
                    // false => Kollision mit gefundener JumpOnPlatform wird berücksichtigt (aktiv)
                    //if (moveDirection.y <= 0)
                    //    platformGrounded = true;

                    if (rb2d.velocity.y <= 0)
                        platformGrounded = true;
                }
            }
        }


        /**
		 * 	Checking if standing on solid/static groundCollider
		 **/
        bool tempLastGrounded = grounded;
        grounded = false;

        if (!platformGrounded)
        {
            // KEINEN Collider aus JumpOnPlatform Ebene (Laye) inerhalb des definierten Bereiches gefunden
            // yellow zone doesn't collid with jumpOnPlatform Collider

            platformGrounded = false;
            foundColliderArray[0] = null;
			Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, foundColliderArray, LayerManager.Instance.whatIsStaticGround);
            if (foundColliderArray[0] != null)
            {
                grounded = true;
            }
            else
            {
                grounded = false;
            }
        }

        if (grounded || platformGrounded)       // grounded unnötig
        {
            grounded = true;
        }

        // Should be Tested
        //if (rb2d.velocity.y > 0)
        //    grounded = false;

        if (tempLastGrounded != grounded)
            CmdSyncGrounded(grounded);          // sync only if changed!
    }

    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSyncGrounded(bool groundedState)
    {
        grounded = groundedState;
    }

    void SimulateAnimation()
    {
        anim.SetBool(HashID.groundedBool, grounded);
//        anim.SetBool(HashID.walledBool, walled);

        anim.SetFloat(HashID.hSpeedFloat, rb2d.velocity.x);
        if (facingRight && moveDirection.x < 0)
        {
            Flip(false);
        }
        else if (!facingRight && moveDirection.x > 0)
        {
            Flip(true);
        }
    }

    void Flip(bool newFacingRightState)
    {

        // Drift sound abspielen
        if (grounded)
        {
            changedRunDirection = true;
            if (anim == null)
            {
                Debug.LogError("Animator not set");
            }
            else
            {
                anim.SetTrigger(HashID.changeRunDirectionTrigger);  // Start Change Run Direction Animation
            }
            if (changeRunDirectionSound != null)
                AudioSource.PlayClipAtPoint(changeRunDirectionSound, transform.position, 1);                //ChangeDirection
            else
                Debug.LogError("change run direction sound fehlt!");

            frictionSmoke.Play();
        }

        // Richtungvariable anpassen
        facingRight = newFacingRightState;

        // Transform spiegeln
        Vector3 theScale = transform.localScale;
        theScale.x = Mathf.Abs(theScale.x);
        if (!facingRight)
            theScale.x *= -1;

        transform.localScale = theScale;

        if (isLocalPlayer)
            CmdSyncFlip(facingRight);   // sync with server, [SyncVar] get changed and updated to Clients
    }

    [Command(channel = Channels.DefaultUnreliable)]
    void CmdSyncFlip (bool facingRightState)
    {
        facingRight = facingRightState;
    }

    // called by SyncVar hook
    void OnChangeDirection(bool newFacingRight)
    {
        if (isLocalPlayer)
            return;

        Flip(newFacingRight);
    }

    internal void StartBetterSpawnDelay()
    {
        throw new NotImplementedException();
    }

    internal void Protection()
    {
        throw new NotImplementedException();
    }

    public void SetSmwCharacterSO(SmwCharacter characterSO)
    {
        //throw new NotImplementedException();
		Debug.LogError ("NotImplementedException !!!");
    }
}
