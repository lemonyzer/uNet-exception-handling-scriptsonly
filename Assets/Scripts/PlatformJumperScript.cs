using UnityEngine;
using System.Collections;
using System;

public class PlatformJumperScript : MonoBehaviour {

    //TODO überschreibt vererbung, zum cachen
    new public Transform transform;
    public Rigidbody2D rb2d;
    Vector3 playerPosition;
    [SerializeField]
    bool usePhysics = true;

    GameObject gameController;
    PlatformCharacterScript myPlatformCharacter;
    //Layer layer;
    //public LayerMask jumpOnPlatform;

    SpriteRenderer spriteRenderer;

    // relevant world stoppers
    BoxCollider2D bodyCollider;
    BoxCollider2D groundStopper;

    Vector2 playerBodyColliderTopLeftPos;
    Vector2 playerBodyColliderBottomRightPos;
    Vector2 playerBodyColliderBottomLeftPos;
    //	Vector2 playerGroundStopperColliderBottomLeftPos;

    Vector2 platformColliderFinderTopLeftPos;
    Vector2 platformColliderFinderBottomRightPos;

    Collider2D[] platformColliderIgnoringArray = new Collider2D[2];
    Collider2D[] platformColliderConsideringArray = new Collider2D[2];

    void Awake ()
    {
        this.transform = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameController = GameObject.FindGameObjectWithTag(Tags.tag_gameController);
        //layer = gameController.GetComponent<Layer>();
        myPlatformCharacter = GetComponent<PlatformCharacterScript>(); // check movedirection (manual physics)
        rb2d = GetComponent<Rigidbody2D>(); // check movedirection (unity physics)

        bodyCollider = transform.Find(Tags.name_body).GetComponent<BoxCollider2D>();
        groundStopper = transform.Find(Tags.name_groundStopper).GetComponent<BoxCollider2D>();

    }

    // Use this for initialization
    void Start () {
        CalculateColliderEdges();
    }

    void CalculateColliderEdges()
    {
        playerBodyColliderTopLeftPos = new Vector2(transform.position.x - bodyCollider.size.x * 0.5f + bodyCollider.offset.x,
                                                   transform.position.y + bodyCollider.size.y * 0.5f + bodyCollider.offset.y);  // Collider Top Left

        playerBodyColliderBottomRightPos = new Vector2(transform.position.x + bodyCollider.size.x * 0.5f + bodyCollider.offset.x,
                                                       transform.position.y - bodyCollider.size.y * 0.5f + bodyCollider.offset.y);  // Collider Bottom Right

        //		playerBodyColliderBottomLeftPos = new Vector2(transform.position.x - bodyCollider.size.x*0.5f + bodyCollider.center.x,
        //		                                              transform.position.y - bodyCollider.size.y*0.5f + bodyCollider.center.y);	// Collider Bottom Left

        //		playerGroundStopperColliderBottomLeftPos = new Vector2(transform.position.x - groundStopper.size.x*0.5f + groundStopper.center.x,
        //		                                              transform.position.y - groundStopper.size.y*0.5f + groundStopper.center.y);	// Collider Bottom Left

        platformColliderFinderTopLeftPos = playerBodyColliderTopLeftPos + new Vector2(-1, 1);       // höhe (nach oben) und breite (nach links) verschieben
        platformColliderFinderBottomRightPos = playerBodyColliderBottomRightPos + new Vector2(1, -1);               // breite (nach rechts) verschieben


    }

    // Update is called once per frame
    void FixedUpdate () {
        JumpAblePlatformV4();
    }


    void JumpAblePlatformV4()
    {
        // Child ColliderFinder with 4 Childs and 2D BoxCollider's... no point calculation, just use 2d boxcollider position +- center.x/.y

        //Physics2D.OverlapArea
        //Physics2D.OverlapCircle
        //Physics2D.OverlapPoint
        //Physics2D.Raycast
        //Physics2D.BoxCast
        //Physics2D.CircleCast

        //Physics.Raycast
        //Physics.OverlapSphere
        //Physics.CheckCapsule
        //Physics.CheckSphere

        /**
		 * find Platform to deactivate
		 **/
         if (usePhysics)
            playerPosition = rb2d.position;
         else
            playerPosition = transform.position;

        //Collider2D platformColliderIgnoring;
        platformColliderFinderTopLeftPos = playerPosition + new Vector3(-0.75f, +0.75f, 0f);
        platformColliderFinderBottomRightPos = playerPosition + new Vector3(+0.75f, -0.75f, 0f);
        //platformColliderIgnoring = Physics2D.OverlapArea(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, jumpOnPlatform);
        platformColliderIgnoringArray[0] = null;
        int found = Physics2D.OverlapAreaNonAlloc(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, platformColliderIgnoringArray, Layer.whatIsJumpOnPlatform);
        //Debug.Log(found);

        //if(platformColliderIgnoring != null)
        if (platformColliderIgnoringArray[0] != null)
        {
            //Physics2D.IgnoreCollision(bodyCollider, platformColliderIgnoringArray[0], true);
            //Physics2D.IgnoreCollision(groundStopper, platformColliderIgnoringArray[0], true);
            for (int i = 0; i < found; i++) {
                Physics2D.IgnoreCollision(bodyCollider, platformColliderIgnoringArray[i], true);
                Physics2D.IgnoreCollision(groundStopper, platformColliderIgnoringArray[i], true);
                Debug.DrawLine(playerPosition, platformColliderIgnoringArray[i].transform.position, Color.green, 1f);
            }
        }

        Color color = Color.red;
#if UNITY_EDITOR
        //Debug.DrawLine(platformColliderFinderTopLeftPos, platformColliderFinderTopLeftPos + new Vector2(0f, -2f), color);
        //Debug.DrawLine(platformColliderFinderTopLeftPos, platformColliderFinderTopLeftPos + new Vector2(2f, 0f), color);
        //Debug.DrawLine(platformColliderFinderBottomRightPos, platformColliderFinderBottomRightPos + new Vector2(0f, +2f), color);
        //Debug.DrawLine(platformColliderFinderBottomRightPos, platformColliderFinderBottomRightPos + new Vector2(-2f, 0f), color);
#endif

        /**
		 * find Platform to activate
		 **/

        /**
            if (groundstopper min height (bottom edge > platformColliderConsideringArray[i].top edge)
                enable collision
            else
                continour ignoring
          **/

        if (myPlatformCharacter.moveDirection.y > 0)            // fix (directly activate collider will result in little beam by UnityPhysikEngine
            return;                                         // and save performance, checking and activating only if needed !!!

        if (rb2d.velocity.y > 0)
            return;

        //Collider2D platformColliderConsidering;
        platformColliderFinderTopLeftPos = playerPosition + new Vector3(-bodyCollider.size.x * 0.5f, -0.4f, 0f);
        platformColliderFinderBottomRightPos = playerPosition + new Vector3(+bodyCollider.size.x * 0.5f, -2f, 0f);
        //platformColliderConsidering = Physics2D.OverlapArea(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, jumpOnPlatform);
        platformColliderConsideringArray[0] = null;
        int found2 = Physics2D.OverlapAreaNonAlloc(platformColliderFinderTopLeftPos, platformColliderFinderBottomRightPos, platformColliderConsideringArray, Layer.whatIsJumpOnPlatform);
        //if(platformColliderConsidering != null)
        if (platformColliderConsideringArray[0] != null)
        {
            //Physics2D.IgnoreCollision(bodyCollider, platformColliderConsideringArray[0], false);
            //Physics2D.IgnoreCollision(groundStopper, platformColliderConsideringArray[0], false);
            for (int i =0; i < found2; i++)
            {
                if (GroundStopperCompletlyHigherThanPlatform (groundStopper, platformColliderConsideringArray[i]))
                {
                    Physics2D.IgnoreCollision(bodyCollider, platformColliderConsideringArray[i], false);
                    Physics2D.IgnoreCollision(groundStopper, platformColliderConsideringArray[i], false);
                    Debug.DrawLine(playerPosition, platformColliderConsideringArray[i].transform.position, Color.magenta, 1f);
                }
                else
                {
                    Debug.DrawLine(playerPosition, platformColliderConsideringArray[i].transform.position, Color.white, 1f);
                    Debug.DrawLine(platformColliderConsideringArray[i].transform.position, platformColliderConsideringArray[i].transform.position + Vector3.down, Color.white, 1f);
                }

            }
        }
        color = Color.green;
#if UNITY_EDITOR
        Debug.DrawLine(platformColliderFinderTopLeftPos, platformColliderFinderTopLeftPos + new Vector2(0f, -1.75f), color);
        Debug.DrawLine(platformColliderFinderTopLeftPos, platformColliderFinderTopLeftPos + new Vector2(bodyCollider.size.x, 0f), color);
        Debug.DrawLine(platformColliderFinderBottomRightPos, platformColliderFinderBottomRightPos + new Vector2(0f, +1.75f), color);
        Debug.DrawLine(platformColliderFinderBottomRightPos, platformColliderFinderBottomRightPos + new Vector2(-bodyCollider.size.x, 0f), color);
#endif

        // DebugCode
        //		if(platformColliderIgnoring != null &&
        //		   platformColliderIgnoring == platformColliderConsidering)
        //		{
        //			Debug.Log(platformColliderConsidering.name + " wurde deaktiviert und sofort wieder aktiviert");
        //			Debug.Log("Physics2D.GetIgnoreCollision() = " + Physics2D.GetIgnoreCollision(groundStopper,platformColliderIgnoring));
        //		}
        //		else
        //		{
        //			if(platformColliderIgnoring != null)
        //			{
        //				Debug.Log(platformColliderIgnoring.name + " wird ignoriert");
        //				Debug.Log("Physics2D.GetIgnoreCollision() = " + Physics2D.GetIgnoreCollision(groundStopper,platformColliderIgnoring));
        //			}
        //			if(platformColliderConsidering != null)
        //			{
        //				Debug.Log(platformColliderConsidering.name + " wird als ground ebene verwendet");
        //				Debug.Log("Physics2D.GetIgnoreCollision() = " + Physics2D.GetIgnoreCollision(groundStopper,platformColliderIgnoring));
        //			}
        //		}
    }

    private bool GroundStopperCompletlyHigherThanPlatform(BoxCollider2D groundStopper, Collider2D collider2D)
    {
        float groundStopperBottomHeight;
        float platformTopHeight;
        //groundStopperBottomHeight = groundStopper.playerPosition.y + groundStopper.offset.y - groundStopper.bounds.size.y * 0.5f;
        groundStopperBottomHeight = rb2d.position.y + groundStopper.offset.y - groundStopper.bounds.size.y * 0.5f;
        platformTopHeight = collider2D.transform.position.y + collider2D.offset.y + collider2D.bounds.size.y * 0.5f;

        // ungenau
        if (groundStopperBottomHeight >= platformTopHeight)
            return true;
        else
            // zusätzlich, genauer!!
            if (Mathf.Pow(groundStopperBottomHeight - platformTopHeight, 2) < 0.05)
                return true;

        Debug.Log("GroundStopp: " + groundStopperBottomHeight + " < Platform: " + platformTopHeight);
        return false;
    }
}
