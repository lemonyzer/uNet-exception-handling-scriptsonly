using UnityEngine;
using System.Collections;

public class AnimatorControllerScript : MonoBehaviour {

	Animator anim;
	Rigidbody2D rb2d;
	Vector2 inputDirection = Vector2.zero;
	Vector2 moveDirection = Vector2.zero;
	Vector2 moveForce = new Vector2 (50f, 100f);						// Linear Drag = 5

	void Awake () {
		anim = this.GetComponent<Animator> ();
		rb2d = this.GetComponent<Rigidbody2D> ();
	}

	// Use this for initialization
	void Start () {
	
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
