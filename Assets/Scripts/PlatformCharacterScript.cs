using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

using UnityStandardAssets.CrossPlatformInput;

public class PlatformCharacterScript : MonoBehaviour {

    //TODO überschreibt vererbung, zum cachen
    new public Transform transform;
    public Rigidbody2D rb2d;

    [SerializeField]
    public bool grounded = false;

    bool walled = false;
    public Vector2 wallCheckPosition = new Vector2(0.5f, 0); // Position, where the the Wall will be checked

    [SerializeField]
    bool beamEnabled = true;

    public Animator anim;                                   // Animator State Machine
    public bool facingRight = true;                         // keep DrawCalls low, Flip textures scale: texture can be used for both directions 
    public bool changedRunDirection = false;
	public AudioClip changeRunDirectionSound;	// Skid Sound
    public AudioClip jumpSound;					// Jump Sound



    private bool useUnityPhysics = true;
    private float rigibodyMoveForce = 40f; // 7
    private float rigibodyJumpForce = 800f; // 7
    private float rigibodyMaxSpeed = 7.5f; // 7

    public Vector3 moveDirection = Vector3.zero;

    Collider2D[] foundColliderArray = new Collider2D[1];

    private BoxCollider2D bodyCollider2D;
    BoxCollider2D myGroundStopperCollider;
    private SpriteRenderer spriteRenderer;


    void Awake ()
    {
		this.transform = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        AnimatorScriptsInitialisierung();
        myGroundStopperCollider = transform.Find(Tags.name_groundStopper).GetComponent<BoxCollider2D>();
        bodyCollider2D = transform.FindChild(Tags.name_body).GetComponent<BoxCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 1;
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
        SimulateWithPhysics();
        CheckBeam();
        CheckPosition();
        SimulateAnimation();
    }

    void SimulateWithPhysics ()
    {
        //TODO
        // NEW
        //TODO
        moveDirection.x = CrossPlatformInputManager.GetAxis ("Horizontal");
        // If the player is changing direction (h has a different sign to velocity.x) or hasn't reached maxSpeed yet...
        if (moveDirection.x * rb2d.velocity.x < rigibodyMaxSpeed)
            // ... add a force to the player.
            rb2d.AddForce(Vector2.right * moveDirection.x * rigibodyMoveForce);

        if (grounded && (rb2d.velocity.y <= 0.1f))
        {
            if (CrossPlatformInputManager.GetButton("Jump"))
            {
                SyncJump();
                moveDirection.y = rigibodyJumpForce;

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
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

        // Beam
        // 0.5 = half player size (pivot.x)
        // if players pos < leftborder+0.5
        // beam to rightborder-0.5
        if (transform.position.x < -10.5f)
        {
            playerPos.x += 20f;
        }
        else if (transform.position.x > 10.5f)
        {
            playerPos.x -= 20f;
        }

        if (transform.position.y < -7.5f)
        {
            playerPos.y += 15f;
        }
        else if (transform.position.y > 7.5f)
        {
            playerPos.y -= 15f;
        }

        transform.position = playerPos;
    }

    void CheckPosition()
    {

        Vector2 playerColliderOffset = new Vector2(0.1f, 0.0f); // FIX player jump at wall and get grounded

        //playerPos spriterenderer boundaries
        Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

        Vector2 groundedOffset = new Vector2(0f, 0.5f);

        Vector2 playerColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x * 0.5f + bodyCollider2D.offset.x,
                                                       transform.position.y);   // Collider Top Left

        Vector2 playerColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x * 0.5f + bodyCollider2D.offset.x,
                                                           transform.position.y - spriteRenderer.bounds.extents.y * 1.2f);  // Collider Bottom Right

        Vector2 playerColliderTopRightPos = new Vector2(transform.position.x + bodyCollider2D.size.x * 0.5f + bodyCollider2D.offset.x,
                                                       transform.position.y);   // Collider Top Right

        Vector2 playerColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider2D.size.x * 0.5f + bodyCollider2D.offset.x,
                                                           transform.position.y - spriteRenderer.bounds.extents.y * 1.2f);  // Collider Bottom Left
#if UNITY_EDITOR
        //		Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
        //		Debug.DrawLine(playerColliderBottomLeftPos, playerColliderTopRightPos, Color.yellow);
        Debug.DrawLine(playerColliderTopLeftPos, playerColliderTopRightPos, Color.yellow);
        Debug.DrawLine(playerColliderTopLeftPos, playerColliderBottomRightPos, Color.yellow);
        Debug.DrawLine(playerColliderTopRightPos, playerColliderBottomLeftPos, Color.yellow);
        Debug.DrawLine(playerColliderBottomLeftPos, playerColliderBottomRightPos, Color.yellow);

        Debug.DrawLine(playerColliderTopLeftPos + playerColliderOffset, playerColliderTopRightPos - playerColliderOffset, Color.white);
        Debug.DrawLine(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, Color.white);
        Debug.DrawLine(playerColliderTopRightPos - playerColliderOffset, playerColliderBottomLeftPos + playerColliderOffset, Color.white);
        Debug.DrawLine(playerColliderBottomLeftPos - playerColliderOffset, playerColliderBottomRightPos + playerColliderOffset, Color.white);
#endif

        /**
		 * check if standing on activ jumpPlatform
		 **/

        //		LayerMask jumpOnPlatform = 1 << layer.jumpAblePlatform;
        //Layer.class
        //Collider2D foundCollider = Physics2D.OverlapArea(playerColliderTopLeftPos, playerColliderBottomRightPos, jumpOnPlatform);

        bool platformGrounded = false;

        //int overlapCount = Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos, playerColliderBottomRightPos, foundColliderArray, jumpOnPlatform );
        foundColliderArray[0] = null;
        Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, foundColliderArray, Layer.whatIsJumpOnPlatform);

        if (foundColliderArray[0] != null)
        {
            // Collider aus JumpOnPlatform Ebene (Laye) inerhalb des definierten Bereiches gefunden
            // yellow zone collids with jumpOnPlatform

            // kontrollieren ob Kollision zwischen Platform und groundStopper ignoriert wird (Ignorierung/Berücksichtigung wird von PlatformJumperScript durchgeführt)
            // wenn Kollision ignoriert wird dann ist der Character zurzeit nicht am Fallen sondern am Springen

            if (Physics2D.GetIgnoreCollision(foundColliderArray[0], myGroundStopperCollider))
            {
                // true => Kollision mit gefundener JumpOnPlatform wird ignoriert (deaktiviert)
                platformGrounded = false;
            }
            else
            {
                // false => Kollision mit gefundener JumpOnPlatform wird berücksichtigt (aktiv)
                if (moveDirection.y <= 0)
                {
                    platformGrounded = true;
                }
            }
        }


        /**
		 * 	Checking if standing on solid/static groundCollider
		 **/


        //		LayerMask ground = 1 << layer.block;
        //		ground |= 1 << layer.ground;
        //Layer.class

        grounded = false;

        if (!platformGrounded)
        {
            // KEINEN Collider aus JumpOnPlatform Ebene (Laye) inerhalb des definierten Bereiches gefunden
            // yellow zone doesn't collid with jumpOnPlatform Collider

            platformGrounded = false;
            foundColliderArray[0] = null;
            Physics2D.OverlapAreaNonAlloc(playerColliderTopLeftPos + playerColliderOffset, playerColliderBottomRightPos - playerColliderOffset, foundColliderArray, Layer.whatIsStaticGround);
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

        /**
		 * Walled
		 **/

        //		walled = Physics2D.OverlapCircle(playerPos+wallCheckPosition, wallRadius, layer.whatIsWall);
        //#if UNITY_EDITOR
        //Debug.DrawLine(playerPos, playerPos+wallCheckPosition + 1*transform.localScale.x * new Vector2(wallRadius,0), Color.green);
        //#endif
    }

    void SimulateAnimation()
    {
        anim.SetBool(HashID.groundedBool, grounded);
        anim.SetBool(HashID.walledBool, walled);

        anim.SetFloat(HashID.hSpeedFloat, moveDirection.x);
        if (facingRight && moveDirection.x < 0)
        {
            Flip();
        }
        else if (!facingRight && moveDirection.x > 0)
        {
            Flip();
        }
    }

    void Flip()
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
        }

        // Richtungvariable anpassen
        facingRight = !facingRight;

        // WallCheck anpassen
        wallCheckPosition *= -1;

        // Transform spiegeln
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;

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
        throw new NotImplementedException();
    }
}
