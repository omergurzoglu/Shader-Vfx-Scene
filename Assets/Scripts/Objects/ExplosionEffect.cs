using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.VFX;

namespace Objects
{
    public class ExplosionEffect : MonoBehaviour
    {
        [SerializeField] private Light pointLight;
        [SerializeField] private GameObject forceField;
        [SerializeField] private VisualEffect vfx;
        public bool isPlaying = false;
        private new Camera camera;

        private void Awake()
        {
            camera= Camera.main;
            forceField.gameObject.SetActive(false);
        }

        public void PlayEffect(Vector3 pos)
        {
            StartCoroutine(Effect(pos));
        }

        private IEnumerator Effect(Vector3 pos)
        {
            if (isPlaying)
            {
                yield break;
            }
            isPlaying = true;
            transform.position = pos;
            forceField.gameObject.SetActive(true);
            pointLight.gameObject.SetActive(true);
            forceField.transform.localScale = Vector3.zero;
            
            yield return PlayTweens(new Vector3(20f, 20f, 20f), 2000f, 350f);
            yield return PlayTweens(Vector3.zero, 200f, 0f);
            
            vfx.Play();
            ShakeCamera(0.3f, 0.9f);
            
            yield return PlayLightTweens(2000f, 300f);
            yield return PlayLightTweens(0f, 0f);
            yield return new WaitForSeconds(2f);
            
            isPlaying = false;
            forceField.gameObject.SetActive(false);
            pointLight.gameObject.SetActive(false);
        }

        private IEnumerator PlayTweens(Vector3 scale, float lightRange, float lightIntensity)
        {
            Tween scaleTween = forceField.transform.DOScale(scale, 0.3f).SetEase(Ease.OutExpo);
            Tween lightRangeTween = DOTween.To(() => pointLight.range, x => pointLight.range = x, lightRange, 0.15f).SetEase(Ease.Flash);
            Tween lightIntensityTween = DOTween.To(() => pointLight.intensity, x => pointLight.intensity = x, lightIntensity, 0.15f).SetEase(Ease.Flash);

            yield return DOTween.Sequence().Join(scaleTween).Join(lightRangeTween).Join(lightIntensityTween).Play().WaitForCompletion();
        }

        private IEnumerator PlayLightTweens(float lightRange, float lightIntensity)
        {
            Tween lightRangeTween = DOTween.To(() => pointLight.range, x => pointLight.range = x, lightRange, 0.2f).SetEase(Ease.InOutSine);
            Tween lightIntensityTween = DOTween.To(() => pointLight.intensity, x => pointLight.intensity = x, lightIntensity, 0.2f).SetEase(Ease.InOutSine);

            yield return DOTween.Sequence().Join(lightRangeTween).Join(lightIntensityTween).Play().WaitForCompletion();
        }
        private void ShakeCamera(float duration, float strength)
        {
            camera.DOShakePosition(duration, strength,50,90f,true,ShakeRandomnessMode.Harmonic);
        }
        
    }
}
