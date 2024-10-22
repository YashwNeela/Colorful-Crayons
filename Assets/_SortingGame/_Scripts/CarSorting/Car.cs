using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TMKOC.Sorting.CarSorting
{
    [Flags]
    public enum CarType
    {
        None = 0,
        RedCar = 1 << 0,
        BlueCar = 1 << 1,
        GreenCar = 1 << 2,
        YellowCar = 1 << 3,

        BlueTire = 1 << 4,
        GreenTire = 1 << 5,
        YellowTire = 1 << 6,
        RedTire = 1 << 7,

        RedPetrolPump = 1<<8, BluePetrolPump = 1<<9, YellowPetrolPump = 1<<10, GreenPetrolPump = 1<<11, RedGarage = 1<<12,
        BlueGarage = 1<<13, YellowGarage = 1<<14, GreenGarage = 1<<15

    }
    public class Car : Collector
    {
        [SerializeField] private CarType m_CarType;

        public CarType CarType => m_CarType;

        public override void SnapCollectibleToCollector(Collectible collectible, Action PlacedCorrectly)
        {
            foreach (var snapPoint in snapPoints)
            {
                if (!snapPoint.IsOccupied)
                {
                    // collectible.GetComponent<Draggable>().HandleRigidbodyKinematic(true);
                    collectible.transform.parent = snapPoint.transform; // Change parent first
                    collectible.transform.localPosition = Vector3.zero; // Reset position relative to the new parent
                   // collectible.transform.localRotation = Quaternion.identity; // Reset rotation relative to the new parent
                    snapPoint.IsOccupied = true;
                    collectible.SetSnapPoint(snapPoint);

                    if (m_CarType.HasFlag((collectible as CarTire).CarType))
                    {
                        OnItemCollected(snapPoint);
                        PlacedCorrectly?.Invoke();
                    }

                    break;
                }
            }
        }

        public override void OnCollectibleExited(Collectible collectible)
        {
            if(!collectible.IsPlacedInsideCollector)
                return;
                
            collectible.RemoveFromSnapPoint();
            if (m_CarType.HasFlag((collectible as CarTire).CarType))
            {
                if (collectedItems > 0)
                    OnItemRemoved();
            }
        }

        public override SnapPoint GetValidSnapPoint(Collectible collectible)
        {
            foreach(SnapPoint snapPoint in snapPoints)
            {
                if(!snapPoint.IsOccupied)
                {
                    if((snapPoint as CarSnapPoint).CarType.HasFlag((collectible as CarTire).CarType))
                    {
                        return snapPoint;
                    }
                }
            }
            return null;
        }

        public void PlayCarAudio()
        {
            if(m_CarType.HasFlag(CarType.BlueCar))
            {
                CarSortingAudioManager.Instance.PlayAudio((CarSortingAudioManager.Instance.CurrentLocalizedAudio as CarSortingAudio).normalCarAudio,CarSortingAudioManager.Instance.SFXAudioSource,isPlayOneShot: true);

            }
            else if(m_CarType.HasFlag(CarType.RedCar))
                CarSortingAudioManager.Instance.PlayAudio((CarSortingAudioManager.Instance.CurrentLocalizedAudio as CarSortingAudio).muscleCarAudio,CarSortingAudioManager.Instance.SFXAudioSource,isPlayOneShot: true);
             else if(m_CarType.HasFlag(CarType.YellowCar))
                CarSortingAudioManager.Instance.PlayAudio((CarSortingAudioManager.Instance.CurrentLocalizedAudio as CarSortingAudio).sportsCarAudio,CarSortingAudioManager.Instance.SFXAudioSource,isPlayOneShot: true);
                 else if(m_CarType.HasFlag(CarType.GreenCar))
                CarSortingAudioManager.Instance.PlayAudio((CarSortingAudioManager.Instance.CurrentLocalizedAudio as CarSortingAudio).normalCarAudio,CarSortingAudioManager.Instance.SFXAudioSource,isPlayOneShot: true);

        }
      

        
    }
}
