using UnityEngine;
using UnityEngine.UI;

namespace TMKOC.BridgeQuest
{
    /// <summary>
    /// Shows the real Tappu IK rig inside the UI.
    ///
    /// The rig is SpriteRenderer-based (IKManager2D + SpriteSkin), and the bridge
    /// lives on a Screen Space - Overlay canvas, so the two cannot share a sorting
    /// order -- a world sprite always draws behind an overlay canvas. So instead of
    /// bending the canvas to the rig, a small dedicated camera films the rig onto a
    /// transparent RenderTexture and the walker slot displays that texture. The
    /// canvas keeps its render mode, BridgeBuilderUI keeps tweening a RectTransform,
    /// and the child still sees a fully animated character.
    ///
    /// The rig sits on its own layer, filmed by its own camera and culled from the
    /// main one, so where it stands in the world is irrelevant -- it is parked well
    /// clear of the play area.
    ///
    /// Idle while standing, walk while crossing. <see cref="BridgeBuilderUI"/> calls
    /// <see cref="PlayWalk"/> as the crossing starts and <see cref="PlayIdle"/> when
    /// the character arrives.
    /// </summary>
    public class BridgeQuestPlayerView : MonoBehaviour
    {
        [Header("Refs")]
        [Tooltip("Animator on the Tappu rig root.")]
        [SerializeField] private Animator animator;

        [Tooltip("The camera that films the rig. Renders only the rig's layer, onto the render texture.")]
        [SerializeField] private Camera rigCamera;

        [Tooltip("The walker slot in the canvas. Its rect decides the texture's shape, so the rig is never stretched.")]
        [SerializeField] private RawImage target;

        [Header("Animator states")]
        [Tooltip("Tappu_Side has no parameters and no transitions, so states are played by name rather than driven by a bool.")]
        [SerializeField] private string idleState = "TappuIdle";
        [SerializeField] private string walkState = "TappuWalk";

        [Tooltip("Seconds to blend between idle and walk. 0 cuts straight to the state.")]
        [SerializeField] private float blendDuration = 0.15f;

        [Header("Framing")]
        [Tooltip("Headroom around the rig, as a multiplier on its height. 1 = tight crop.")]
        [SerializeField] private float verticalMargin = 1.10f;

        [Tooltip("Render texture pixels per canvas pixel. 2-3 keeps the rig crisp on a high-DPI screen.")]
        [SerializeField] private int supersample = 3;

        [Tooltip("Animate on unscaled time. The crossing and every card in this game run with the world frozen, and a scaled Animator freezes with it.")]
        [SerializeField] private bool animateUnscaled = true;

        private RenderTexture owned;   // only textures created here are destroyed here
        private bool warned;

        private void Awake()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>(true);

            EnsureTexture();
            FrameRig();
        }

        /// <summary>
        /// The first pose is set here rather than in Awake on purpose. An Animator
        /// initialises to its controller's default state after Awake, so a Play() in
        /// Awake is silently thrown away -- which left the rig running Tappu_Side's
        /// default TappuRun instead of standing still.
        /// </summary>
        private void Start()
        {
            ApplyUpdateMode();
            PlayIdle();
        }

        private void ApplyUpdateMode()
        {
            if (animator == null || !animateUnscaled) return;

            // the crossing, every card and the storyboard all run on a frozen world;
            // a scaled Animator freezes with it and the character stops mid-stride
            if (animator.updateMode != AnimatorUpdateMode.UnscaledTime)
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        private void OnDestroy()
        {
            if (owned == null) return;

            // never leave the camera or the UI pointing at a texture that is about to die
            if (rigCamera != null && rigCamera.targetTexture == owned) rigCamera.targetTexture = null;
            if (target != null && target.texture == owned) target.texture = null;

            owned.Release();
            Destroy(owned);
            owned = null;
        }

        // ---- animation -------------------------------------------------------

        /// <summary>Standing still.</summary>
        public void PlayIdle() { Play(idleState); }

        /// <summary>Crossing the bridge.</summary>
        public void PlayWalk() { Play(walkState); }

        private void Play(string state)
        {
            if (animator == null || string.IsNullOrEmpty(state)) return;

            // a name that is not in the controller silently does nothing, which reads
            // as "the character froze" -- say so once instead
            if (!animator.HasState(0, Animator.StringToHash(state)))
            {
                if (!warned)
                {
                    warned = true;
                    Debug.LogWarning("[BridgeQuest] Animator has no state '" + state
                        + "' on layer 0 -- the character will not animate.", this);
                }
                return;
            }

            ApplyUpdateMode();

            if (blendDuration > 0f) animator.CrossFadeInFixedTime(state, blendDuration, 0);
            else animator.Play(state, 0, 0f);
        }

        // ---- render texture --------------------------------------------------

        /// <summary>
        /// Builds a render texture shaped like the walker slot, so the rig keeps its
        /// proportions whatever size the slot is set to. Re-runs when the slot is
        /// resized, which is why the size is read from the rect rather than authored.
        /// </summary>
        private void EnsureTexture()
        {
            if (rigCamera == null || target == null)
            {
                Debug.LogWarning("[BridgeQuest] BridgeQuestPlayerView needs both a rigCamera and a target RawImage.", this);
                return;
            }

            Rect r = target.rectTransform.rect;
            int ss = Mathf.Max(1, supersample);
            int w = Mathf.Max(16, Mathf.RoundToInt(r.width * ss));
            int h = Mathf.Max(16, Mathf.RoundToInt(r.height * ss));

            if (owned != null && owned.width == w && owned.height == h) return;

            if (owned != null)
            {
                if (rigCamera.targetTexture == owned) rigCamera.targetTexture = null;
                if (target.texture == owned) target.texture = null;
                owned.Release();
                Destroy(owned);
            }

            owned = new RenderTexture(w, h, 16, RenderTextureFormat.ARGB32);
            owned.name = "TappuWalker_RT";
            owned.antiAliasing = 2;
            owned.Create();

            rigCamera.targetTexture = owned;
            target.texture = owned;
        }

        /// <summary>
        /// Points the camera at the rig and zooms it to fit. Measured from the
        /// renderers rather than authored, so a taller character (Bhide, Gogi) drops
        /// in without re-tuning the camera by hand.
        /// </summary>
        private void FrameRig()
        {
            if (rigCamera == null) return;

            // measure the animated rig only, and only what is actually drawn. Measuring
            // the whole object would fold in anything else parked under it -- leftover
            // art from another game, a spare rig -- and zoom the camera out to nothing.
            Transform rigRoot = animator != null ? animator.transform : transform;
            SpriteRenderer[] parts = rigRoot.GetComponentsInChildren<SpriteRenderer>(false);
            if (parts == null || parts.Length == 0) return;

            bool any = false;
            Bounds b = new Bounds();
            for (int i = 0; i < parts.Length; i++)
            {
                if (parts[i] == null || !parts[i].enabled) continue;
                if (!any) { b = parts[i].bounds; any = true; }
                else b.Encapsulate(parts[i].bounds);
            }
            if (!any) return;

            rigCamera.orthographic = true;
            rigCamera.orthographicSize = Mathf.Max(0.01f, b.size.y * 0.5f * Mathf.Max(1f, verticalMargin));

            Vector3 p = rigCamera.transform.position;
            rigCamera.transform.position = new Vector3(b.center.x, b.center.y, p.z);
        }

#if UNITY_EDITOR
        /// <summary>Keeps the editor preview honest while the slot is being laid out.</summary>
        private void OnValidate()
        {
            if (!Application.isPlaying) return;
            EnsureTexture();
            FrameRig();
        }
#endif
    }
}
