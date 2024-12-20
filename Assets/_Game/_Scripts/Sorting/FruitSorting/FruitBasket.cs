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
        public GameObject SmallBox;
        public BasketType basketType;
        public List<SnapPoint> snapPoints;
    }


    public class FruitBasket : Collector
    {
        [SerializeField] private BasketType m_BasketType;

        public BasketType BasketType => m_BasketType;

        //Only for Editor script to generate level

        public void SetBasketType(BasketType basketType)
        {
            m_BasketType = basketType;
            SnapPointData snapPointData = m_SnapPointData[0];
            snapPointData.basketType = basketType;
            m_SnapPointData[0] = snapPointData;
           
        }

        private Renderer m_Renderer;

        private StarCollectorParticleImage m_StartCollector;

        [SerializeField] private FruitBasketLableSO m_FruitBasketLableSO;

        [SerializeField] private List<SnapPointData> m_SnapPointData;

        public List<SnapPointData> SnapPointData => m_SnapPointData;

        [SerializeField] public ParticleSystem m_FruitCollectParticleEffect;

        
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
           // Gamemanager.OnRightAnswerAction += OnRightAnswerAction;
           // Gamemanager.OnWrongAnswerAction += OnWrongAnswer;
            SortingGameManager.OnGameWin += OnGameWin;
           
        }
        

        private void OnGameWin()
        {
            //Invoke(nameof(PlayLevelCompletedBlast),3);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            //Gamemanager.OnRightAnswerAction -= OnRightAnswerAction;
          //  Gamemanager.OnWrongAnswerAction -= OnWrongAnswer;
            SortingGameManager.OnGameWin -= OnGameWin;

            

        }

        private void OnRightAnswerAction()
        {
            if(currentSelectedSmallBoxAnimation != null){

            currentSelectedSmallBoxAnimation.DOComplete();
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
            }
        }

        private void OnWrongAnswer()
        {
            if(currentSelectedSmallBoxAnimation != null){

            currentSelectedSmallBoxAnimation.DOComplete();
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
            }
        }

        private void SetSmallBoxColor(int count, Color color, BasketType basketType)
        {
            List<BasketType> individualFlags = GetIndividualFlags(basketType);

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
                    if(individualFlags.Count == 1){
                    
                    (s as FruitSnapPoint).SetBasketType(basketType);
                    }else
                    {
                        (s as FruitSnapPoint).SetBasketType(individualFlags[j]);
                        
                    }
                    m_SnapPointData[i].SmallBox.gameObject.GetComponent<Renderer>().materials[1].color = color;
                }
            }
        }

        private void SetSmallBoxTexture(int count, Sprite texture, BasketType basketType)
        {
            List<BasketType> individualFlags = GetIndividualFlags(basketType);
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
                    if(individualFlags.Count == 1){
                    
                    (s as FruitSnapPoint).SetBasketType(basketType);
                    }else
                    {
                        (s as FruitSnapPoint).SetBasketType(individualFlags[j]);
                        
                    }
                    m_SnapPointData[i].SmallBox.gameObject.GetComponent<Renderer>().materials[1].color = Color.white;
                    m_SnapPointData[i].SmallBox.gameObject.GetComponent<Renderer>().materials[1].mainTexture = texture.texture;

                }
            }
        }

        // Method to extract individual flags from a combined enum value
    public static List<BasketType> GetIndividualFlags(BasketType combinedFlags)
    {
        List<BasketType> individualFlags = new List<BasketType>();

        foreach (BasketType flag in Enum.GetValues(typeof(BasketType)))
        {
            // Skip the 'None' flag or any combined flags
            if (flag != BasketType.None && combinedFlags.HasFlag(flag))
            {
                individualFlags.Add(flag);
            }
        }
        for(int i = 0;i<individualFlags.Count;i++)
        {
            Debug.Log("Falg is " + individualFlags[i].ToString());
        }

        return individualFlags;
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
                     //   m_Renderer.materials[1].mainTexture = m_FruitBasketLableSO.BasketLableTextures[BasketType.StarFruit].texture;
            
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
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Chery], basketType);
                    break;

                case BasketType.Tomato:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Tomato], basketType);

                    break;

                case BasketType.BeetRoot:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.BeetRoot], basketType);

                    break;

                case BasketType.Apple:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Apple], basketType);


                    break;

                case BasketType.Banana:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Banana], basketType);


                    break;

                case BasketType.StarFruit:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.StarFruit], basketType);

                    break;

                case BasketType.Mango:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Mango], basketType);

                    break;

                case BasketType.WaterMelon:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.WaterMelon], basketType);

                    break;

                case BasketType.Pear:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Pear], basketType);

                    break;

                case BasketType.Papaya:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Papaya], basketType);

                    break;

                case BasketType.Apricot:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Apricot], basketType);

                    break;

                case BasketType.GrapeFruit:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.GrapeFruit], basketType);

                    break;

                case BasketType.Guava:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Guava], basketType);

                    break;

                case BasketType.OrangeFruit:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.OrangeFruit], basketType);

                    break;

                case BasketType.Avacado:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.Avacado], basketType);

                    break;

                case BasketType.GreenApple:
                    SetSmallBoxTexture(m_SnapPointData.Count, m_FruitBasketLableSO.BasketLableTextures[BasketType.GreenApple], basketType);
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
                    if (basketType.HasFlag(BasketType.OrangeFruit) && basketType.HasFlag(BasketType.Apple))
                    {
                        //m_Renderer.materials[1].mainTexture = m_FruitBasketLableSO.appleAndOrange.texture;
                         var combinedKey = BasketType.OrangeFruit | BasketType.Apple;
                        if (m_FruitBasketLableSO.BasketLableTextures.TryGetValue(combinedKey, out Sprite sprite1)){

                         sprite = sprite1;
                        SetSmallBoxTexture(m_SnapPointData.Count, sprite, basketType);

                        }
                    }

                    if (basketType.HasFlag(BasketType.Apple) && basketType.HasFlag(BasketType.Avacado))
                    {
                        //m_Renderer.materials[1].mainTexture = m_FruitBasketLableSO.appleAndOrange.texture;
                         var combinedKey = BasketType.Apple | BasketType.Avacado;
                        if (m_FruitBasketLableSO.BasketLableTextures.TryGetValue(combinedKey, out Sprite sprite1)){

                         sprite = sprite1;
                        SetSmallBoxTexture(m_SnapPointData.Count, sprite, basketType);

                        }
                    }
                    if (basketType.HasFlag(BasketType.Mango) && basketType.HasFlag(BasketType.WaterMelon))
                    {
                        //m_Renderer.materials[1].mainTexture = m_FruitBasketLableSO.appleAndOrange.texture;
                         var combinedKey = BasketType.Mango | BasketType.WaterMelon;
                        if (m_FruitBasketLableSO.BasketLableTextures.TryGetValue(combinedKey, out Sprite sprite1)){

                         sprite = sprite1;
                        SetSmallBoxTexture(m_SnapPointData.Count, sprite, basketType);

                        }
                    }

                    if (basketType.HasFlag(BasketType.WaterMelon) && basketType.HasFlag(BasketType.Papaya))
                    {
                        //m_Renderer.materials[1].mainTexture = m_FruitBasketLableSO.appleAndOrange.texture;
                         var combinedKey = BasketType.WaterMelon | BasketType.Papaya;
                        if (m_FruitBasketLableSO.BasketLableTextures.TryGetValue(combinedKey, out Sprite sprite1)){

                         sprite = sprite1;
                        SetSmallBoxTexture(m_SnapPointData.Count, sprite, basketType);

                        }
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
            if(currentSelectedSmallBoxAnimation != null){

            currentSelectedSmallBoxAnimation.DOPlayBackwards();
            }
        }

        public override void OnWrongItemTriedToCollect()
        {
            base.OnWrongItemTriedToCollect();
            Debug.Log("wrong Item");
            if(currentSelectedSmallBoxAnimation != null){

            currentSelectedSmallBoxAnimation.DOPlayBackwards();
            }
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
                        currentSelectedSmallBoxAnimation = m_SnapPointData[i].SmallBox.GetComponent<DOTweenAnimation>();
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
            if(currentSelectedSmallBoxAnimation != null){
            currentSelectedSmallBoxAnimation.DOComplete();
            currentSelectedSmallBoxAnimation.DOPlayBackwards();
            }

            currentSelectedSmallBoxAnimation = null;
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

                    m_FruitCollectParticleEffect.transform.position = snapPoint.transform.position;
                    m_FruitCollectParticleEffect.Play();
                    break;
                }
                else
                {
                    if(currentSelectedSmallBoxAnimation != null){
                    currentSelectedSmallBoxAnimation.DOComplete();
                    currentSelectedSmallBoxAnimation.DOPlayBackwards();
                    }
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
