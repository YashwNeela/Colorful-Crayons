using UnityEngine;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// A bird crossing the flight path as a moving obstacle. It drifts back towards
    /// the player while bobbing up and down, so it sweeps through the airspace the
    /// rocket wants rather than sitting still and waiting to be flown around.
    ///
    /// Touching one crashes the rocket, which costs a life through exactly the same
    /// path as splashing into the water.
    /// </summary>
    public class Bird : MonoBehaviour
    {
        [Header("Flight")]
        [Tooltip("How fast the bird travels back towards the player, in units per second.")]
        [SerializeField] private float speed = 2.4f;
        [Tooltip("How far the bird rises and falls around its spawn height.")]
        [SerializeField] private float bobAmplitude = 0.8f;
        [SerializeField] private float bobSpeed = 2.2f;

        [Header("Housekeeping")]
        [Tooltip("Destroyed once this far behind the camera, so strays never pile up.")]
        [SerializeField] private float despawnBehindCamera = 16f;

        [Tooltip("Squash the body in time with the bob. Leave off when the artwork brings its own flap animation -- two flaps fighting each other looks wrong.")]
        [SerializeField] private bool proceduralFlap;

        private Transform cam;
        private float baseY;
        private float phase;
        private Vector3 baseScale = Vector3.one;
        private bool ready;

        /// <summary>Called by LevelBuilder the moment the bird is spawned.</summary>
        /// <summary>Called by LevelBuilder the moment the bird is spawned.</summary>
        public void Configure(float flySpeed, float amplitude, Transform cameraTransform, bool flap)
        {
            speed = flySpeed;
            bobAmplitude = amplitude;
            cam = cameraTransform;
            proceduralFlap = flap;
        }

        private void Start()
        {
            baseY = transform.position.y;
            phase = Random.Range(0f, Mathf.PI * 2f);
            baseScale = transform.localScale;
            if (cam == null && Camera.main != null) cam = Camera.main.transform;
            ready = true;
        }

        private void Update()
        {
            if (!ready) return;

            float t = Time.time * bobSpeed + phase;

            Vector3 p = transform.position;
            p.x -= speed * Time.deltaTime;
            p.y = baseY + Mathf.Sin(t) * bobAmplitude;
            transform.position = p;

            // stand-in flap for placeholder art. Real artwork animates itself, and
            // scaling the root underneath an Animator just fights it.
            if (proceduralFlap)
            {
                Vector3 s = baseScale;
                s.y *= 1f + Mathf.Sin(t * 3f) * 0.22f;
                transform.localScale = s;
            }

            if (cam != null && p.x < cam.position.x - despawnBehindCamera) Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            RocketPlayer p = other.GetComponentInParent<RocketPlayer>();
            if (p != null && p.Alive) p.Crash();
        }
    }
}
