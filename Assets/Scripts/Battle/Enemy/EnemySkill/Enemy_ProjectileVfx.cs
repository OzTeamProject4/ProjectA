using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixPlays.ElementalVFX
{
    public class Enemy_ProjectileVfx : Enemy_BaseVfx
    {
        [SerializeField] ParticleSystem _CastEffect;
        [SerializeField] ParticleSystem _HitEffect;
        [SerializeField] ParticleSystem _ProjectileEffect;
        [SerializeField] float _ProjectileFlyDelay;
        [SerializeField] float _ProjectileDeactivateDelay;

        private Coroutine _castCoroutine;

        public override void Play(VfxData data)
        {
            base.Play(data);

            if (_castCoroutine != null) StopCoroutine(_castCoroutine);
            _castCoroutine = StartCoroutine(Coroutine_PlayEffects());
        }

        private IEnumerator Coroutine_PlayEffects()
        {
            // 1. Cast 이펙트 재생
            if (_CastEffect != null)
            {
                _CastEffect.gameObject.SetActive(true);
                _CastEffect.transform.position = _data.Source;
                _CastEffect.transform.forward = (_data.Target - _data.Source).normalized;
                _CastEffect.Play();
            }

            // Fly Delay 대기 후 투사체 파티클 활성화
            if (_ProjectileFlyDelay > 0)
                yield return new WaitForSeconds(_ProjectileFlyDelay);

            // 2. Projectile(투사체) 이펙트 재생
            if (_ProjectileEffect != null)
            {
                _ProjectileEffect.gameObject.SetActive(true);
                _ProjectileEffect.transform.position = transform.position;
                _ProjectileEffect.Play();
            }
        }

        /// <summary>
        /// 물리 충돌 시 호출되어 투사체를 끄고 적중(Hit) 이펙트를 재생합니다.
        /// </summary>
        public void PlayHitEffect(Vector3 hitPoint, Vector3 hitNormal)
        {
            if (_castCoroutine != null) StopCoroutine(_castCoroutine);

            // 투사체 이펙트 중지
            if (_ProjectileEffect != null)
            {
                _ProjectileEffect.Stop();
            }

            // Hit 이펙트 위치 및 방향 설정 후 재생
            if (_HitEffect != null)
            {
                _HitEffect.transform.position = hitPoint;
                _HitEffect.transform.forward = hitNormal;
                _HitEffect.gameObject.SetActive(true);
                _HitEffect.Play();
            }

            StartCoroutine(Coroutine_DeactivateProjectile());
        }

        private IEnumerator Coroutine_DeactivateProjectile()
        {
            yield return new WaitForSeconds(_ProjectileDeactivateDelay);
            if (_ProjectileEffect != null)
            {
                _ProjectileEffect.gameObject.SetActive(false);
            }
        }

        public override void Stop()
        {
            base.Stop();
            if (_HitEffect != null) _HitEffect.Stop();
            if (_ProjectileEffect != null) _ProjectileEffect.Stop();
            if (_CastEffect != null) _CastEffect.Stop();
        }
    }
}