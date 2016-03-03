using UnityEngine;
using System.Collections;

public class AnimatorControllerScript : MonoBehaviour {

	Animator anim;
	Rigidbody2D rb2d;
	Vector2 inputDirection = Vector2.zero;
	Vector2 moveDirection = Vector2.zero;
	Vector2 moveForce = new Vector2 (50f, 100f);						// Linear Drag = 5

	Vector2 velocity = Vector2.zero;

	[SerializeField]
	bool useCustomDrag = false;
	[SerializeField]
	float customDrag = 0.9f;

	void Awake () {
		anim = this.GetComponent<Animator> ();
		rb2d = this.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void FixedUpdate () {

		if (useCustomDrag) {
			velocity = rb2d.velocity;
			velocity.x *= customDrag;
			rb2d.velocity = velocity;
		}
	}

	bool isAlive = false;
	bool isGrounded = true;
	bool triggerProtection = false;
	bool triggerRage = false;
	
	// Update is called once per frame
	void Update () {
	
		if (Input.GetButtonDown ("Jump"))
			isGrounded = !isGrounded;

		if (Input.GetButtonDown ("Fire1"))
			isAlive = !isAlive;

		if (Input.GetButtonDown ("Fire2"))
			triggerProtection = !triggerProtection;

		if (Input.GetButtonDown ("Fire3"))
			triggerRage = !triggerRage;

		inputDirection.x = Input.GetAxis ("Horizontal");

		moveDirection.x = inputDirection.x * moveForce.x;
		moveDirection.y = inputDirection.y * moveForce.y;

//		if(Mathf.Abs(rb2d.velocity.x) <= 4.0f)
			rb2d.AddForce (moveDirection, ForceMode2D.Force);			// Linear Drag = 5

		anim.SetFloat ("hSpeed", rb2d.velocity.x);
		anim.SetFloat ("hInput", inputDirection.x);
		anim.SetBool ("isGrounded", isGrounded);
		anim.SetBool ("isAlive", isAlive);

		if (triggerProtection) {
			triggerProtection = !triggerProtection;
			anim.SetTrigger ("triggerProtection");
		}

		if (triggerRage) {
			triggerRage = !triggerRage;
			anim.SetTrigger ("triggerRage");
		}


		CheckBeam();

//		if (InputAndVelocitySameDirection (inputDirection.x, rb2d.velocity.x)) {
//
//		}
		bool skiddingLastFrame = isSkidding;
		isSkidding = CheckSkidding (inputDirection.x, rb2d.velocity.x);

		if (isSkidding != skiddingLastFrame)
			if (isSkidding)
				skiddingTrigger = true;			// set flag
		
		
		if (skiddingTrigger) {					// check Trigger (and remove flag)
			frictionSmoke.Play ();
			audioSource.PlayOneShot (skidClip);
			skiddingTrigger = false;
		}

		// Flip
		if (inputDirection.x < 0)
			localScale.x = -1f;
		else if (inputDirection.x > 0)
			localScale.x = 1f;
		
		transform.localScale = localScale;

		// Flip Sprite only
//		SpriteRenderer spriteRender;
//		spriteRender.flipX =

		if (!isSkidding) {
			frictionSmoke.Stop ();
		}

	}

	Vector3 localScale = Vector3.one;

	[SerializeField]
	bool isSkidding = false;
	[SerializeField]
	bool skiddingTrigger = false;

	[SerializeField]
	AudioClip skidClip;

	[SerializeField]
	AudioSource audioSource;

	[SerializeField]
	ParticleSystem frictionSmoke = null;

	public ParticleSystem FrictionSmoke {
		get { return frictionSmoke; }
		set { frictionSmoke = value; }
	}

	bool CheckSkidding (float inputX, float velocityX)
	{
		if (Mathf.Abs (inputX) > 0.1f) {
			// Input == (Rechts oder Links)

			if (InputAndVelocitySameDirection (inputX, velocityX)) {
				return false;
			} else {
				//
				return true;
			}

		} else {
			// keine Eingabe
			return false;
		}
			
	}

	bool InputAndVelocitySameDirection (float inputX, float velocityX) 
	{
		if (Mathf.Sign (inputX) == Mathf.Sign (velocityX))
			return true;
		return false;
	}

	bool beamEnabled = true;
	bool useUnityPhysics = true;

	Vector3 playerPosition;
	Vector3 playerPositionBeforeBeam;


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

		playerPositionBeforeBeam = playerPosition;

		// Beam
		// 0.5 = half player size (pivot.x)
		// if players pos < leftborder+0.5
		// beam to rightborder-0.5

		while (playerPosition.x <= - 10.5f)
			playerPosition.x += 20f;

		while (playerPosition.x > 10.5f)
			playerPosition.x -= 20f;

		while (playerPosition.y <= - 7.5f)
			playerPosition.y += 15f;

		while (playerPosition.y > 7.5f)
			playerPosition.y -= 15f;

		//        if (playerPosition.x < -10.5f)
		//        {
		//            playerPosition.x += 20f;
		//        }
		//        else if (playerPosition.x > 10.5f)
		//        {
		//            playerPosition.x -= 20f;
		//        }
		//
		//        if (playerPosition.y < -7.5f)
		//        {
		//            playerPosition.y += 15f;
		//        }
		//        else if (playerPosition.y > 7.5f)
		//        {
		//            playerPosition.y -= 15f;
		//        }

		if (playerPosition != playerPositionBeforeBeam)
		{
			// position has changed

			if (useUnityPhysics)
				rb2d.MovePosition (playerPosition);
			else
				transform.position = playerPosition;
		}
	}
}
