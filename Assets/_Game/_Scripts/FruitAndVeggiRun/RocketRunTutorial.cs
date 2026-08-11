using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TMKOC.FruitAndVeggiRun
{

    /// <summary>
    /// Guided opening tutorial. Every instruction is shown with the world frozen and
    /// waits for a tap; dismissing it unfreezes the game for a short hands-on beat
    /// before the next instruction interrupts. Sequence:
    ///   1. "hold to fly"       -> tap -> 3s of free flying, no pickups in the level
    ///   2. "collect the fruit" -> tap -> a guaranteed correct fruit spawns ahead
    ///   3. water approaches,  2s later  -> "water costs a life"
    /// then normal play resumes and the level starts spawning produce again.
    /// </summary>
    public class RocketRunTutorial : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private GameFlow flow;
        [SerializeField] private TargetWordUI targetUI;
        [SerializeField] private LevelBuilder level;
        [SerializeField] private RocketPlayer player;

        [Header("UI")]
        [SerializeField] private GameObject root;
        [SerializeField] private RectTransform bubble;
        [SerializeField] private Image primaryIcon;
        [SerializeField] private Image secondaryIcon;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private TextMeshProUGUI tapHintText;

        [Header("Step Extras")]
        [Tooltip("Animated hand over the middle of the screen. Only shown on the 'hold to fly' step.")]
        [SerializeField] private GameObject tapHand;
        [Tooltip("Ring that pulses on the target-item icon. Only shown on the 'collect the fruit' step.")]
        [SerializeField] private GameObject targetHighlight;

        [Header("Step Icons")]
        [Tooltip("Pointing-finger tap icon, used on the 'hold to fly' step.")]
        [SerializeField] private Sprite handIcon;
        [Tooltip("Same heart sprite as the lives UI, used as a secondary icon on the life-cost steps.")]
        [SerializeField] private Sprite heartIcon;

        [Tooltip("Bird art for the 'birds cost a life' step. Left empty, the bird prefab's own sprite is used.")]
        [SerializeField] private Sprite birdIcon;

        [Header("Tuning")]
        [SerializeField] private float popInDuration = 0.3f;
        [Tooltip("Seconds of free flying after the first instruction, before the collect step.")]
        [SerializeField] private float freeFlyDuration = 3f;
        [Tooltip("Seconds after water comes into view before the warning interrupts.")]
        [SerializeField] private float waterWarnDelay = 2f;
        [Tooltip("Seconds of flight between the rocket and a freshly spawned target fruit.")]
        [SerializeField] private float targetFruitLeadTime = 1.5f;
        [Tooltip("Seconds of flight the water should still be ahead by when the warning appears.")]
        [SerializeField] private float waterSpareTime = 1.5f;

        [Header("Safety Timeouts")]
        [Tooltip("How far behind the rocket a tutorial fruit must fall before it counts as missed and respawns.")]
        [SerializeField] private float missedMargin = 2f;
        [Tooltip("Safety net: stop waiting for water after this long, so a dry setup cannot stall the steps that follow. If water never appears the warning is simply skipped.")]
        [SerializeField] private float waterWaitTimeout = 120f;

        // set by WaitForWaterAhead, so the warning is only given if water really turned up
        private bool waterSeen;

        private class Step
        {
            public string message;
            public Sprite primary;
            public Sprite secondary;
            public string voiceKey;
            /// <summary>Show the animated hand over the screen for this step.</summary>
            public bool showHand;
            /// <summary>Pulse a ring on the target-item icon for this step.</summary>
            public bool highlightTarget;
        }

        private void Awake()
        {
            if (flow == null) flow = FindObjectOfType<GameFlow>();
            if (targetUI == null) targetUI = FindObjectOfType<TargetWordUI>();
            if (level == null) level = FindObjectOfType<LevelBuilder>();
            if (player == null) player = FindObjectOfType<RocketPlayer>();
        }

        private void Start()
        {
            // freeze before anything can move, so the rocket is stationary on the
            // very first frame the player sees
            Time.timeScale = 0f;
            if (targetUI != null) targetUI.SetFillVisible(false);
            if (root != null) root.SetActive(false);
            // these two live outside the bubble, so they need hiding by hand
            if (tapHand != null) tapHand.SetActive(false);
            if (targetHighlight != null) targetHighlight.SetActive(false);
            StartCoroutine(RunTutorial());
        }

        private IEnumerator RunTutorial()
        {
            // wait a frame so every other script's Start() (GameFlow and LevelBuilder
            // included) has run before we read CurrentTarget or touch the level
            yield return null;

            // the opening story cut-scene owns the screen first -- sit tight (still
            // frozen) until it has played out or the player skipped it
            while (StoryCutsceneUI.IsPlaying)
            {
                yield return null;
            }

            // the cut-scene may have unfrozen on its way out; take the freeze back
            Time.timeScale = 0f;

            // Fruit keeps falling all through the lesson so the sky is never empty, but
            // none of it counts yet -- a lucky first flight should not finish the whole
            // item before the player has been told what they are doing.
            if (flow != null) flow.PracticeMode = true;

            // ---- 1. hold to fly ----
            yield return ShowInstruction(new Step
            {
                message = "Hold anywhere on the screen to fly up. \n Let go to fall!",
                primary = handIcon,
                secondary = null,
                showHand = true,
                voiceKey = VoiceKeys != null ? VoiceKeys.TutorialFly : null
            });

            // let them actually fly for a few seconds, grabbing practice fruit
            yield return new WaitForSeconds(freeFlyDuration);

            // ---- 2. collect the fruit ----
            string targetName = flow != null ? flow.CurrentTarget : null;
            Sprite targetIcon = (level != null && !string.IsNullOrEmpty(targetName))
                ? level.GetProduceSprite(targetName)
                : null;

            yield return ShowInstruction(new Step
            {
                message = "Now collect the " + (string.IsNullOrEmpty(targetName) ? "fruit" : targetName) + "!",
                primary = targetIcon,
                secondary = null,
                highlightTarget = true,
                voiceKey = VoiceKeys != null ? VoiceKeys.TutorialCollect : null
            });

            // keep dropping a fresh apple every time one is missed, so the step can
            // only be cleared by actually collecting one
            Collectible targetFruit = null;
            while (true)
            {
                if (targetFruit == null)
                {
                    targetFruit = SpawnAhead(targetName, true, targetFruitLeadTime);
                    // refs missing / nothing to spawn - don't hang the tutorial
                    if (targetFruit == null) break;
                }

                // wait until it's picked up (Collectible destroys itself) or flown past
                while (targetFruit != null && !HasFlownPast(targetFruit))
                {
                    yield return null;
                }

                if (targetFruit == null) break; // collected it

                Destroy(targetFruit.gameObject);
                targetFruit = null;
            }

            // ---- done: hand the level back to normal ----
            // Counting starts HERE, before the hazard warnings below. Those wait on
            // things that only exist deeper into the run, so holding practice mode open
            // until then would stop the player ever finishing an item -- and so stop
            // them ever reaching the water or the birds in the first place.
            if (flow != null) flow.PracticeMode = false;
            if (level != null) level.SuppressProduce = false;
            if (targetUI != null) targetUI.SetFillVisible(true);
            if (root != null) root.SetActive(false);
            if (tapHand != null) tapHand.SetActive(false);
            if (targetHighlight != null) targetHighlight.SetActive(false);
            Time.timeScale = 1f;

            // nothing else is speaking now -- good moment for the "ready, set, fly" line
            if (flow != null) flow.AnnounceRunStart();

            // ---- 3. water costs a life ----
            // Deferred. The opening stretch is dry by design, so this sits quiet in the
            // background and interrupts the moment real water first comes into view.
            yield return WaitForWaterAhead();

            if (waterSeen && level != null)
            {
                yield return new WaitForSeconds(waterWarnDelay);

                yield return ShowInstruction(new Step
                {
                    message = "Splashing into the water costs a life!",
                    primary = level.WaterSprite,
                    secondary = heartIcon,
                    voiceKey = VoiceKeys != null ? VoiceKeys.TutorialWater : null
                });
            }

            // ---- 4. birds cost a life ----
            // Same idea, much later. Birds only turn up once a few items are in the
            // basket, and a warning given back at the start would be long forgotten.
            yield return WaitForBirdsAhead();

            if (level != null)
            {
                yield return ShowInstruction(new Step
                {
                    message = "Watch out for the birds! \n Bumping into one costs a life too!",
                    primary = birdIcon != null ? birdIcon : level.BirdIcon,
                    secondary = heartIcon,
                    voiceKey = VoiceKeys != null ? VoiceKeys.TutorialBird : null
                });

                // only now are birds allowed into the world
                level.ConfirmBirdBriefing();
            }
        }

        /// <summary>
        /// Freezes the world, pops the bubble in, waits for a tap, then hides the
        /// bubble and unfreezes so the player can act on what they just read.
        /// </summary>
        private IEnumerator ShowInstruction(Step step)
        {
            Time.timeScale = 0f;
            if (root != null) root.SetActive(true);

            yield return PresentStep(step);

            // eat one frame so the tap that dismissed the previous bubble can't
            // also immediately dismiss this one
            yield return null;

            while (!TapDetected())
            {
                yield return null;
            }

            if (root != null) root.SetActive(false);
            // the target-icon ring is parented to the HUD, not the bubble, so it does
            // not go away with the popup
            if (targetHighlight != null) targetHighlight.SetActive(false);
            Time.timeScale = 1f;
        }

        /// <summary>Fills the bubble in with this step's text/icons and plays the pop-in.</summary>
        private IEnumerator PresentStep(Step step)
        {
            if (messageText != null) messageText.text = step.message;

            RocketRunVoice.Play(step.voiceKey);

            if (primaryIcon != null)
            {
                primaryIcon.sprite = step.primary;
                primaryIcon.enabled = step.primary != null;
            }
            if (secondaryIcon != null)
            {
                secondaryIcon.sprite = step.secondary;
                secondaryIcon.gameObject.SetActive(step.secondary != null);
            }
            if (tapHintText != null) tapHintText.gameObject.SetActive(true);

            // the hand only belongs on the step that asks for a screen press; the
            // target ring only on the step that asks for a pickup
            if (tapHand != null) tapHand.SetActive(step.showHand);
            if (targetHighlight != null) targetHighlight.SetActive(step.highlightTarget);

            if (bubble != null)
            {
                float t = 0f;
                bubble.localScale = Vector3.zero;
                while (t < popInDuration)
                {
                    t += Time.unscaledDeltaTime;
                    float p = Mathf.Clamp01(t / popInDuration);
                    float s = Mathf.Sin(p * Mathf.PI * 0.5f); // quick ease-out pop
                    bubble.localScale = Vector3.one * s;
                    yield return null;
                }
                bubble.localScale = Vector3.one;
            }
        }

        /// <summary>Drops a tutorial pickup a little way in front of the rocket.</summary>
    private Collectible SpawnAhead(string itemName, bool isTarget, float leadSeconds)
        {
            if (level == null || player == null || string.IsNullOrEmpty(itemName)) return null;

            Vector3 pos = player.transform.position + new Vector3(LeadDistance(leadSeconds), 0f, 0f);
            return level.SpawnTutorialCollectible(pos, itemName, isTarget);
        }

    /// <summary>True once the rocket has clearly passed this pickup without grabbing it.</summary>
        private bool HasFlownPast(Collectible c)
        {
            if (c == null || player == null) return false;
            return c.transform.position.x < player.transform.position.x - missedMargin;
        }


        /// <summary>How far the rocket travels in the given number of seconds.</summary>
        private float LeadDistance(float seconds)
        {
            float speed = player != null ? player.ForwardSpeed : 7.5f;
            return speed * seconds;
        }

        /// <summary>Polls until a water stretch is coming up in front of the player.</summary>
    private IEnumerator WaitForWaterAhead()
        {
            waterSeen = false;
            if (level == null || player == null) yield break;

            // look far enough ahead that the water is still in front of the rocket
            // once the warning delay has elapsed
            float lookahead = LeadDistance(waterWarnDelay + waterSpareTime);

            float waited = 0f;
            while (waterWaitTimeout <= 0f || waited < waterWaitTimeout)
            {
                if (level.IsWaterAt(player.transform.position.x + lookahead))
                {
                    waterSeen = true;
                    yield break;
                }
                waited += Time.deltaTime;
                yield return null;
            }
        }

        /// <summary>
        /// Polls until the level opens its bird stretch. That flag flips the instant the
        /// band changes, a second or two before the first bird is actually within reach,
        /// which is exactly when the warning is worth giving.
        /// </summary>
        /// <summary>
        /// Polls until the level says birds are ready but the player has not been warned.
        /// The level holds them at the gate until ConfirmBirdBriefing() is called, so
        /// nothing can be hit by a bird it has not been told about.
        /// </summary>
        private IEnumerator WaitForBirdsAhead()
        {
            if (level == null) yield break;
            while (!level.BirdsAwaitingBriefing) yield return null;
        }

        private bool TapDetected()
        {
            if (Input.GetMouseButtonDown(0)) return true;
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (Input.GetTouch(i).phase == TouchPhase.Began) return true;
            }
            return false;
        }


    /// <summary>Voice-over keys for the four instruction steps, or null if no mapper is in the scene.</summary>
        private RocketRunAudioMapper VoiceKeys
        {
            get { return RocketRunVoice.Mapper; }
        }
    }
}
