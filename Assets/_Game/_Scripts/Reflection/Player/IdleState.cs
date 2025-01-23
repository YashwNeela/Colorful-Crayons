using UnityEngine;

namespace TMKOC.Reflection
{

    public class IdleState : IPlayerState
    {
        public void Enter(PlayerStateMachine player)
        {
            player.SetAnimatorParameters(ground: true, speed: 0);
        }

        public void Exit(PlayerStateMachine player)
        {
            // Any cleanup when exiting Idle state
        }

        public void Jump(PlayerStateMachine player)
        {
            if (player.IsGrounded())
            {
                player.Jump();
                player.ChangeState(new JumpingState());
            }
        }

        public void Update(PlayerStateMachine player)
        {
            float moveX = player.moveX;
            if (Mathf.Abs(moveX) > 0.1f && player.IsGrounded())
            {
                player.ChangeState(new WalkingState());
            }
        }
    }
}
