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

    [System.Serializable]
    public struct SnapPointData
    {
        public GameObject m_SmallBox;
        public List<SnapPoint> snapPoints;
    }


    public class FruitBasket : Collector
    {
        [SerializeField] private BasketType m_BasketType;

        public BasketType BasketType => m_BasketType;

        private Renderer m_Renderer;

        private StarCollectorParticleImage m_StartCollector;

        [SerializeField] private FruitBasketLableSO m_FruitBasketLableSO;

        [SerializeField] private List<SnapPointData> m_SnapPointData;
        //public List<Sprite> m_LableTextures;

        protected override void Awake()
        {
            base.Awake();
            m_Renderer = GetComponent<Renderer>();
            m_StartCollector = FindAnyObjectByType<StarCollectorParticleImage>();
            

            // Set the colors of the box based on the selected CrayonColor flags
            SetBoxColorsBasedOnEnum(m_BasketType);
            SetSnapPoints();
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
            currentSelectedSmallBoxAnimation.DOComplete();
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
        }

        private void OnWrongAnswer()
        {
            currentSelectedSmallBoxAnimation.DOComplete();
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
        }

        private void SetSmallBoxColor(int count, Color color, BasketType basketType)
        {
            if (count > m_SnapPointData.Count)
            {
                Debug.LogError("Count is greater than Small box list");
                return;
            }
            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < m_SnapPointData[i].snapPoints.Count; j++)
                {
                    SnapPoint s = m_SnapPointData[i].snapPoints[j];
                    (s as FruitSnapPoint).SetBasketType(basketType);
                    m_SnapPointData[i].m_SmallBox.gameObject.GetComponent<Renderer>().materials[1].color = ColorCodes.red;
                }
            }
        }

        /// <summary>
        /// This is kind of overriding the snappoint vairable list in parent class
        /// </summary>

        private void SetSnapPoints()
        {
            List<SnapPoint> tempSnapPoint = new List<SnapPoint>();
            for (int i = 0; i < m_SnapPointData.Count; i++)
            {
                for (int j = 0; j < m_SnapPointData[i].snapPoints.Count; j++)
                {
                    tempSnapPoint.Add(m_SnapPointData[i].snapPoints[j]);
                }
            }

            //Parent variable
            snapPoints = new SnapPoint[tempSnapPoint.Count];

            for (int i = 0; i < tempSnapPoint.Count; i++)
            {
                snapPoints[i] = tempSnapPoint[i];
            }
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
                    SetSmallBoxColor(m_SnapPointData.Count, ColorCodes.red,basketType);
                    break;

                case BasketType.Yellow:
                    SetSmallBoxColor(m_SnapPointData.Count, ColorCodes.yellow, basketType);

                    break;

                case BasketType.Green:
                    SetSmallBoxColor(m_SnapPointData.Count, ColorCodes.green, basketType);
                    break;

                case BasketType.Orange:
                    SetSmallBoxColor(m_SnapPointData.Count, ColorCodes.orange, basketType);
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
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
        }

        public override void OnWrongItemTriedToCollect()
        {
            base.OnWrongItemTriedToCollect();
            Debug.Log("wrong Item");
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
        }

        DOTweenAnimation currentSelectedSmallBoxAnimation;

        public override void OnCollectibleEntered(Collectible collectible)
        {
            base.OnCollectibleEntered(collectible);
            Debug.Log("Collectible entered");
            bool hasToEnd = false;
            for (int i = 0; i < m_SnapPointData.Count; i++)
            {
                if (hasToEnd)
                    break;
                for (int j = 0; j < m_SnapPointData[i].snapPoints.Count; j++)
                {
                    FruitSnapPoint fruitSnapPoint = m_SnapPointData[i].snapPoints[j] as FruitSnapPoint;
                    BasketType f = (collectible as Fruit).BasketType;

                    if (CanFruitBePutInBasket(fruitSnapPoint.BasketType, f) &&
                    !fruitSnapPoint.IsOccupied)
                    {
                        currentSelectedSmallBoxAnimation = m_SnapPointData[i].m_SmallBox.GetComponent<DOTweenAnimation>();
                        currentSelectedSmallBoxAnimation.DOComplete();
                        currentSelectedSmallBoxAnimation.DOPlayForward();
                        hasToEnd = true;
                        break;

                    }


                }
            }
     
           // m_OnBasketEnteredAnimation.DOPlayForward();
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            base.OnCollectibleExited(collectible);
            Debug.Log("Collectible Exited");
            currentSelectedSmallBoxAnimation.DOComplete();
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
        }

        public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            
            foreach (var snapPoint in snapPoints)
            {
                FruitSnapPoint fruitSnapPoint = snapPoint as FruitSnapPoint;
                BasketType f = (collectible as Fruit).BasketType;
                //  basketType & fruitType
                // bool y = fruitSnapPoint.BasketType & (collectible as Fruit).BasketType;

                if (CanFruitBePutInBasket(fruitSnapPoint.BasketType, f) &&
                    !fruitSnapPoint.IsOccupied)
                {
                    collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform;
                    collectible.transform.localPosition = Vector3.zero;
                    collectible.transform.localRotation = Quaternion.identity;
                    snapPoint.IsOccupied = true;
                    OnItemCollected(snapPoint);
                    PlacedCorrectly?.Invoke();
                    break;
                }
                else
                {
                    currentSelectedSmallBoxAnimation.DOComplete();
                    currentSelectedSmallBoxAnimation.DOPlayBackwards();
                }
            }
        }

        private bool CanFruitBePutInBasket(BasketType basketType, BasketType fruitType)
        {
            // Check if the basket type has all the required flags of the fruit type
            return (basketType & fruitType) != 0;
        }


    }
}
