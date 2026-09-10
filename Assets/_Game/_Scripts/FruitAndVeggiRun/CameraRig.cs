using UnityEngine;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>Side-scrolling follow camera. Locks to the player's X, eases on Y.</summary>
    public class CameraRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float xOffset = 4.2f;
        [SerializeField] private float yFollowSpeed = 2.4f;
        [SerializeField] private float minY = -0.6f;
        [SerializeField] private float maxY = 2.2f;

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 p = transform.position;
            p.x = target.position.x + xOffset;

            float desiredY = Mathf.Clamp(target.position.y * 0.45f, minY, maxY);
            p.y = Mathf.Lerp(p.y, desiredY, yFollowSpeed * Time.deltaTime);

            transform.position = p;
        }

    /// <summary>
        /// Jumps straight to the target, no easing. Used on restart: the camera
        /// normally only catches up in LateUpdate, and a level streamer that runs
        /// before then would see the camera still parked where the player crashed.
        /// </summary>
        public void SnapToTarget()
        {
            if (target == null) return;

            Vector3 p = transform.position;
            p.x = target.position.x + xOffset;
            p.y = Mathf.Clamp(target.position.y * 0.45f, minY, maxY);
            transform.position = p;
        }


        public void SetTarget(Transform t)
        {
            target = t;
        }
    }
}
