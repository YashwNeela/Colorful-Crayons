using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;

namespace TMKOC.Sorting.FruitSorting
{
    [Flags]
    public enum BasketType
    {
        None = 0,
        Red = 1 << 0,
        Yellow = 1 << 1,
        Green = 1 << 2,
        Orange = 1 << 3,

        Chery = 1 << 4, Tomato = 1 << 5, BeetRoot = 1 << 6, Apple = 1 << 7, Banana = 1 << 8, StarFruit = 1 << 9, Mango = 1 << 10, WaterMelon = 1 << 11, Pear = 1 << 12,
        Papaya = 1 << 13, Apricot = 1 << 14, GrapeFruit = 1 << 15, Guava = 1 << 16, OrangeFruit = 1 << 17, Avacado = 1 << 18, GreenApple = 1 << 19


    }


    public class FruitBasket : Collector
    {
        [SerializeField] private BasketType m_BasketType;

        public BasketType BasketType => m_BasketType;

        private Renderer m_Renderer;

        private StarCollectorParticleImage m_StartCollector;

        private DOTweenAnimation m_OnBasketEnteredAnimation;

        [SerializeField] private FruitBasketLableSO m_FruitBasketLableSO;
        //public List<Sprite> m_LableTextures;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            m_OnBasketEnteredAnimation = GetComponent<DOTweenAnimation>();

            // Set the colors of the box based on the selected CrayonColor flags
            SetBoxColorsBasedOnEnum(m_BasketType);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Gamemanager.OnRightAnswerAction += OnRightAnswerAction;
            Gamemanager.OnWrongAnswerAction += OnWrongAnswer;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Gamemanager.OnRightAnswerAction -= OnRightAnswerAction;
            Gamemanager.OnWrongAnswerAction -= OnWrongAnswer;
        }

        private void OnRightAnswerAction()
        {
            m_OnBasketEnteredAnimation.DOComplete();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        private void OnWrongAnswer()
        {
            m_OnBasketEnteredAnimation.DOComplete();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }



        private void SetBoxColorsBasedOnEnum(BasketType basketType)
        {
            // List<Color> selectedColors = new List<Color>();

            // // Check each flag and add corresponding colors from ColorCodes
            // if (basketType.HasFlag(BasketType.Red))
            // {
            //     selectedColors.Add(ColorCodes.red);
            // }
            // if (basketType.HasFlag(BasketType.Yellow))
            // {
            //     selectedColors.Add(ColorCodes.yellow);
            // }
            // if (basketType.HasFlag(BasketType.Green))
            // {
            //     selectedColors.Add(ColorCodes.green);
            // }
            // if (basketType.HasFlag(BasketType.Orange))
            // {
            //     selectedColors.Add(ColorCodes.orange);
            // }

            Sprite sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Chery];
            m_Renderer.materials[1].mainTexture = null;
            // if (basketType.HasFlag(BasketType.Apple) && basketType.HasFlag(BasketType.Banana))
            // {
            //     sprite = m_FruitBasketLableSO.BasketLableTextures[basketType];
            //     m_Renderer.materials[1].mainTexture = sprite.texture;
            // }

           // m_Renderer.materials[1].color = selectedColors[0];
            
            switch (basketType)
            {
                #region Color
                case BasketType.None:
                    Debug.Log("No basket selected.");
                    break;

                case BasketType.Red:
                    m_Renderer.materials[1].color = ColorCodes.red;
                    break;

                case BasketType.Yellow:
                    m_Renderer.materials[1].color = ColorCodes.yellow;
                    break;

                case BasketType.Green:
                    m_Renderer.materials[1].color = ColorCodes.green;
                    break;

                case BasketType.Orange:
                    m_Renderer.materials[1].color = ColorCodes.orange;
                    break;

                #endregion

                #region SingleTexture
                // Add cases for each fruit basket
                case BasketType.Chery:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Chery];
                    m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Tomato:
                     sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Tomato];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.BeetRoot:
                   sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.BeetRoot];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Apple:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Apple];
                     m_Renderer.materials[1].mainTexture = sprite.texture;

                    break;

                case BasketType.Banana:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Banana];
                     m_Renderer.materials[1].mainTexture = sprite.texture;

                    break;

                case BasketType.StarFruit:
                   sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.StarFruit];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Mango:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Mango];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.WaterMelon:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.WaterMelon];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Pear:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Pear];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Papaya:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Papaya];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Apricot:
                   sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Apricot];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.GrapeFruit:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.GrapeFruit];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Guava:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Guava];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.OrangeFruit:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.OrangeFruit];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.Avacado:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.Avacado];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;

                case BasketType.GreenApple:
                    sprite = m_FruitBasketLableSO.BasketLableTextures[BasketType.GreenApple];
                     m_Renderer.materials[1].mainTexture = sprite.texture;
                    break;
                #endregion

                // Handle combinations of basket types using `HasFlag`
                default:

                #region MultipleTexture
                    if (basketType.HasFlag(BasketType.Orange) && basketType.HasFlag(BasketType.Banana))
                    {
                         sprite = m_FruitBasketLableSO.BasketLableTextures[m_BasketType];
                        m_Renderer.materials[1].mainTexture = sprite.texture;
                    }
                    if (basketType.HasFlag(BasketType.Apple) && basketType.HasFlag(BasketType.Banana))
                    {
                         sprite = m_FruitBasketLableSO.BasketLableTextures[m_BasketType];
                        m_Renderer.materials[1].mainTexture = sprite.texture;
                    }
                    if (basketType.HasFlag(BasketType.Apple) && basketType.HasFlag(BasketType.Guava))
                    {
                         sprite = m_FruitBasketLableSO.BasketLableTextures[m_BasketType];
                        m_Renderer.materials[1].mainTexture = sprite.texture;
                    }
                    if (basketType.HasFlag(BasketType.Banana) && basketType.HasFlag(BasketType.Guava))
                    {
                         sprite = m_FruitBasketLableSO.BasketLableTextures[m_BasketType];
                        m_Renderer.materials[1].mainTexture = sprite.texture;
                    }
                    if (basketType.HasFlag(BasketType.Apple) && basketType.HasFlag(BasketType.OrangeFruit))
                    {
                         sprite = m_FruitBasketLableSO.BasketLableTextures[m_BasketType];
                        m_Renderer.materials[1].mainTexture = sprite.texture;
                    }
                    if (basketType.HasFlag(BasketType.Apple) && basketType.HasFlag(BasketType.Avacado))
                    {
                         sprite = m_FruitBasketLableSO.BasketLableTextures[m_BasketType];
                        m_Renderer.materials[1].mainTexture = sprite.texture;
                    }
                    break;
                #endregion
            }
        }

        protected override void OnItemCollected(SnapPoint snapPoint)
        {
            base.OnItemCollected(snapPoint);
            m_StartCollector.SetEmitter(snapPoint.transform);
            m_StartCollector.PlayParticle();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        public override void OnWrongItemTriedToCollect()
        {
            base.OnWrongItemTriedToCollect();
            Debug.Log("wrong Item");
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        public override void OnCollectibleEntered(Collectible collectible)
        {
            base.OnCollectibleEntered(collectible);
            Debug.Log("Collectible entered");
            m_OnBasketEnteredAnimation.DOPlayForward();
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            base.OnCollectibleExited(collectible);
            Debug.Log("Collectible Exited");
            m_OnBasketEnteredAnimation.DOComplete();
            m_OnBasketEnteredAnimation.DOPlayBackwards();
        }

        public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                                                                        //  collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();
                    break;
                }
            }
        }

    }
}
