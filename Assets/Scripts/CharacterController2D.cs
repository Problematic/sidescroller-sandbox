using UnityEngine;
using System.Linq;

[System.Flags]
public enum CollisionDirections
{
	None = 0,
	Up = 1,
	Left = 2,
	Right = 4,
	Down = 8
}

public class CharacterController2D : MonoBehaviour
{
	[SerializeField]
	protected float groundSpeed;
	[SerializeField]
	protected float jumpForce;
	[SerializeField]
	protected LayerMask groundLayers;
	[SerializeField]
	protected float walltouchDampTime;

	[Header ("State Vars")]
	[SerializeField] 
	protected bool isJumpScheduled;
	[SerializeField]
	protected CollisionDirections collisions;
	[SerializeField]
	protected float wallTouchAt;

	protected Rigidbody2D body;
	new protected Collider2D collider;

	protected void Awake ()
	{
		body = GetComponent<Rigidbody2D> ();
		collider = GetComponent<Collider2D> ();
	}

	protected void Update ()
	{
		if (Input.GetButtonDown ("Jump")) {
			isJumpScheduled = true;
		}
	}

	protected void FixedUpdate ()
	{
		if (isJumpScheduled) {
			isJumpScheduled = false;

			if (IsGrounded) {
				body.AddForce (Vector2.up * jumpForce, ForceMode2D.Impulse);
			}
		}

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical")).normalized;

		Vector2 velocity = body.velocity;
		velocity.x = input.x * groundSpeed;

		if (velocity.y <= 0) {
			if ((velocity.x > 0 && IsTouching (CollisionDirections.Right)) || (velocity.x < 0 && IsTouching (CollisionDirections.Left))) {
				float t = (Time.time - wallTouchAt) / walltouchDampTime;
				velocity = Vector2.Lerp (velocity, Vector2.Scale (velocity, Vector2.up), t);
			}
		}

		body.velocity = velocity;
	}

	protected void OnCollisionEnter2D (Collision2D coll)
	{
		Debug.LogFormat ("OnCollisionEnter: {0}", coll);

		if (!collider.IsTouchingLayers (groundLayers.value)) {
			return;
		}

		foreach (ContactPoint2D cp in coll.contacts) {
			if (cp.normal == Vector2.down) {
				collisions |= CollisionDirections.Up;
			} else if (cp.normal == Vector2.up) {
				collisions |= CollisionDirections.Down;
			} else if (cp.normal == Vector2.right) {
				collisions |= CollisionDirections.Left;
				wallTouchAt = Time.time;
			} else if (cp.normal == Vector2.left) {
				collisions |= CollisionDirections.Right;
				wallTouchAt = Time.time;
			}
		}
	}

	protected void OnCollisionExit2D (Collision2D coll)
	{
		Debug.LogFormat ("OnCollisionExit: {0}", coll);

		foreach (ContactPoint2D cp in coll.contacts) {
			if (cp.normal == Vector2.down) {
				collisions &= ~CollisionDirections.Up;
			} else if (cp.normal == Vector2.up) {
				collisions &= ~CollisionDirections.Down;
			} else if (cp.normal == Vector2.right) {
				collisions &= ~CollisionDirections.Left;
			} else if (cp.normal == Vector2.left) {
				collisions &= ~CollisionDirections.Right;
			}
		}
	}

	public void Jump ()
	{
		isJumpScheduled = true;
	}

	public bool IsGrounded {
		get {
			return IsTouching (CollisionDirections.Down);
		}
	}

	public bool IsTouching (CollisionDirections direction)
	{
		return (collisions & direction) == direction;
	}
}
