using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PixPlays.ElementalVFX
{
    public class Enemy_BaseVfx : MonoBehaviour
    {
        [SerializeField] float _SafetyDestroy; //Destroy the object after a certan time in case user error keeps it active.
        [SerializeField] float _DestoyDelay; //Wait for effect to finish stopping before destroying the GameObject
        protected VfxData _data;
        public virtual void Play(VfxData data)
        {
            _data = data;
            StopAllCoroutines();
        }

        public virtual void Stop()
        {
            StopAllCoroutines();
        }
    }
}