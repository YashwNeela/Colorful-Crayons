using UnityEngine;

namespace TMKOC.Reflection
{
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

            float moveX = 0;
            #if UNITY_EDITOR
                moveX = Input.GetAxis("Horizontal");
            #else
                moveX = player.moveX;
            #endif
            // Move the player
            player.Move(moveX);

            if (Mathf.Abs(moveX) <= 0.1f)
            {
                player.ChangeState(new IdleState());
            }
        }
    }
}