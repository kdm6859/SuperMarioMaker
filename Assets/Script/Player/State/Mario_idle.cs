using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario_idle : Mario_state
{
	public Mario_idle(Mario mario, Mario_stateMachine stateMachine, string animBool) : base(mario, stateMachine, animBool)
	{
	}

	public override void Enter()
	{
		base.Enter();
	}

	public override void Exit()
	{
		base.Exit();
	}

	public override void Update()
	{
		base.Update();

		if (xInput != 0)
		{
			stateMachine.ChangeState(mario.walkState);
		}

		// ����
		if (Input.GetKeyDown(KeyCode.Space))
		{
			stateMachine.ChangeState(mario.jumpState);
		}
	}
}
