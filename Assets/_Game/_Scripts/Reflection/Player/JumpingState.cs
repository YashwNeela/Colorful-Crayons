using UnityEngine;

namespace TMKOC.Reflection
{
    public class JumpingState : IPlayerState
    {
        private bool canMoveToIdle;
        public void Enter(PlayerStateMachine player)
        {
            player.SetAnimatorParameters(ground: false, speed: 0);
            canMoveToIdle = false;
            player.StartCoroutine(StaticCoroutine.Co_GenericCoroutine(1, () =>
            {
                CanMoveToIdle();
            }));

        }

        public void Exit(PlayerStateMachine player)
        {
            // Cleanup when exiting Jumping state
        }

        public void Jump(PlayerStateMachine player)
        {

        }

        public void Update(PlayerStateMachine player)
        {
            float moveX = player.moveX;

            if (player.IsGrounded())
            {

                // if (Mathf.Abs(moveX) > 0.1f)
                // {
                //     player.Move(moveX);
                //    // player.ChangeState(new WalkingState());
                // }
                
                {
                    if (canMoveToIdle)
                        player.ChangeState(new IdleState());
                }
            }
            else
            {
                player.Move(moveX);
            }
        }

        private void CanMoveToIdle()
        {
            canMoveToIdle = true;
        }

    }
}
