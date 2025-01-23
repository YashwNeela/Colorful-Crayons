using UnityEngine;

namespace TMKOC.Reflection{
public class JumpingState : IPlayerState
{
    public void Enter(PlayerStateMachine player)
    {
        player.SetAnimatorParameters(ground: false, speed: 0);
    }

    public void Exit(PlayerStateMachine player)
    {
        // Cleanup when exiting Jumping state
    }

    public void Update(PlayerStateMachine player)
    {
        float moveX = Input.GetAxis("Horizontal");

        if (player.IsGrounded())
        {

            if (Mathf.Abs(moveX) > 0.1f)
            {
                player.ChangeState(new WalkingState());
            }
            else
            {
                player.ChangeState(new IdleState());
            }
        }
        else
        {
            player.Move(moveX);
        }
    }

}}
