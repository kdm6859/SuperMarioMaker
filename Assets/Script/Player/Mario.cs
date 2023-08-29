using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Mario : MonoBehaviour
{

	// marioMode_0: smallMario, marioMode_1: bigMario, marioMode_2: fireMario
	
	
    [Header("Move Info")]
    public float moveSpeed = 5;
	public float runSpeed = 6;
	public float jumpPower;
	public float lastXSpeed;

	[Header("Ground Check")]
	public Transform obj_isGround;
	public Transform obj_isPlayerA;
	public Transform obj_isPlayerB;
	public Transform obj_isWallA;
	public Transform obj_isWallB;
	public float groundCheckDist;
	public float playerCheckDist;
	public LayerMask whatIsGround;
	public LayerMask whatIsPlayer;
	

    [Header("Audio source")]
    public AudioSource jump_audioSource;
	public AudioClip[] clips;

	

    [HideInInspector] public Rigidbody2D rb;
	[HideInInspector] public CapsuleCollider2D collider;

	[HideInInspector] public PhysicsMaterial2D PM;
	[HideInInspector] public Animator anim;
	[HideInInspector] public SpriteRenderer spriteRenderer;

	public Mario_stateMachine stateMachine;
 
    public Mario_idle idleState;
    public Mario_run runState;
	public Mario_jump jumpState;
	public Mario_slide slideState;
	public Mario_walk walkState;
	public Mario_kicked kickedState;
	public Mario_sitDown sitDown;
	public Mario_die dieState;
	public Mario_stamp stampState;


	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		collider = GetComponent<CapsuleCollider2D>();
		//PM = rb.GetComponent<PhysicsMaterial2D>();
		//collider.sharedMaterial = PM;
		PM = new PhysicsMaterial2D();
		collider.sharedMaterial = PM;
		//PM = collider.GetComponent<PhysicsMaterial2D>();

		anim = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();

        stateMachine = new Mario_stateMachine();

        idleState = new Mario_idle(this, stateMachine, "Idle");
		walkState = new Mario_walk(this, stateMachine, "Walk");
		runState = new Mario_run(this, stateMachine, "Run");
		jumpState = new Mario_jump(this, stateMachine, "Jump");
		slideState = new Mario_slide(this, stateMachine, "Slide");
		kickedState = new Mario_kicked(this, stateMachine, "Kicked");
		sitDown = new Mario_sitDown(this, stateMachine, "Sit");
		dieState = new Mario_die(this, stateMachine, "Die");
        stampState = new Mario_stamp(this, stateMachine, "Jump");
		
	}
	[PunRPC]
	public void Flip(bool a)
	{
		spriteRenderer.flipX = a;
	}

	private void Start()
	{
		//if(!GetComponent<PhotonView>().IsMine) return ;
        stateMachine.InitState(idleState);
	}

	// Update is called once per frame
	void Update()
    {
		//if (!GetComponent<PhotonView>().IsMine) return;
		//Debug.Log(GetComponent<PhotonView>().IsMine);
		stateMachine.currentState.Update();
	}

	//// 부활 만들기
	//void Respawn()
	//{
	//	if (GetComponent<PhotonView>().IsMine)
	//	{
	//		// 로컬이면 체크 포인트에서 Respawn
	//		//PhotonNetwork.Instantiate()
	//	}
	//}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		//밑에 적이 있음 == 죽으면 안 됨
		if (IsEnemyDetected() != null)
		{
			return;
		}
		else if (collision.gameObject.GetComponent<Enemy_shell>() != null)
		{
			//Debug.Log(collision.gameObject.GetComponent<Rigidbody2D>().velocity.x);
			// 멈춰있는 거북이 등딱지에 맞으면 삶
			if (collision.gameObject.GetComponent<Enemy_shell>().fsecMove == false)
			{
				collision.gameObject.GetComponent<Enemy_shell>().fsecMove = true;
				return;
			}
			else stateMachine.ChangeState(dieState); // 움직이는 거북이 등딱지에 맞으면 죽음
		}

		if (collision.gameObject.tag == "Enemy" && IsEnemyDetected() == null)
		{
			stateMachine.ChangeState(dieState);
		}

	}

	//public bool IsGroundDetected() => Physics2D.Raycast(obj_isGround.position, Vector2.down, groundCheckDist, whatIsGround);
	public bool IsGroundDetected()
	{
		Collider2D[] cols = Physics2D.OverlapAreaAll(obj_isPlayerA.position, obj_isPlayerB.position, LayerMask.GetMask("Ground"));
		//Debug.Log("그라운드 트루");

		if (cols != null && cols.Length > 0) return true;
		//for (int i = 0; i < cols.Length; i++)
		//{
		//	if (cols[i].gameObject.CompareTag("Ground"))
		//	{
		//		Debug.Log("그라운드 true");
		//		return true;
		//	}
		//}

				//Debug.Log("그라운드 false");

		return false;
	}
	public GameObject IsPlayerDetected()
	{
        Collider2D[] cols = Physics2D.OverlapAreaAll(obj_isPlayerA.position, obj_isPlayerB.position, LayerMask.GetMask("Player"));

		for (int i = 0; i < cols.Length; i++)
		{
			if (cols[i].gameObject != this.gameObject && cols[i].gameObject.CompareTag("Player"))
			{
				return cols[i].gameObject;
			}
		}
		return null;
	}

	public GameObject IsEnemyDetected()
	{
		Collider2D[] cols = Physics2D.OverlapAreaAll(obj_isPlayerA.position, obj_isPlayerB.position);
		for (int i = 0; i < cols.Length; i++)
		{
			if (cols[i].gameObject != this.gameObject && cols[i].gameObject.CompareTag("Enemy"))
			{
				if(cols[i].gameObject.GetComponent<Enemy>() != null)
					cols[i].gameObject.GetComponent<Enemy>().Die();
				return cols[i].gameObject;
			}
		}
		return null;
	}

	public bool IsWallDetected()
	{
        Vector3 positionA;
        Vector3 positionB;
		
		if (!stateMachine.currentState.isFlip)
        {
			positionA = obj_isWallA.localPosition;
			positionB = obj_isWallB.localPosition;
		}
        else
        {
			positionA = new Vector3(-obj_isWallA.localPosition.x, obj_isWallB.localPosition.y, obj_isWallA.localPosition.z);
			positionB = new Vector3(-obj_isWallB.localPosition.x, obj_isWallA.localPosition.y, obj_isWallB.localPosition.z);
        }
		
		Debug.DrawLine(transform.position + positionA, transform.position + positionB, Color.cyan);
		Collider2D[] cols = Physics2D.OverlapAreaAll(transform.position + positionA, transform.position + positionB);
		for (int i = 0; i < cols.Length; i++)
		{
			if (cols[i].gameObject != this.gameObject && cols[i].gameObject.CompareTag("Ground"))
			{
				//Debug.Log(cols[i].name);
				return true;
			}
		}
		return false;
	}

	protected virtual void OnDrawGizmos()
	{
		Gizmos.DrawLine(obj_isGround.position,
			new Vector3(obj_isGround.position.x, obj_isGround.position.y - groundCheckDist));
		//Gizmos.DrawLine(obj_isWallA.position, obj_isWallB.position);
	}
}
