using UnityEngine;

namespace TMKOC.Reflection{
public class WalkingState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.SetAnimatorParameters(ground: true, speed: 1);
    }

    public void Exit(PlayerStateMachine player)
    {
        // Any cleanup when exiting Walking state
    }

    public void Update(PlayerStateMachine player)
    {
        float moveX = Input.GetAxis("Horizontal");

        // Move the player
        player.Move(moveX);

        if (Mathf.Abs(moveX) <= 0.1f)
        {
            player.ChangeState(new IdleState());
        }
        else if (Input.GetButtonDown("Jump") && player.IsGrounded())
        {
            player.Jump();
            player.ChangeState(new JumpingState());
        }
    }
}
}