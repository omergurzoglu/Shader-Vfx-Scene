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

        private void Awake()
        {
            forceField.gameObject.SetActive(false);
        }

        public void PlayEffect(Vector3 pos)
        {
            StartCoroutine(Effect(pos));
        }

        private IEnumerator Effect(Vector3 pos)
        {
            transform.position = pos;
            
            forceField.gameObject.SetActive(true);
            pointLight.gameObject.SetActive(true);
            forceField.transform.localScale = Vector3.zero;

            // Play initial scale-up and light tweens
            yield return PlayTweens(new Vector3(10f, 10f, 10f), 80f, 100f);

            // Play scale-down and light tweens
            yield return PlayTweens(Vector3.zero, 5f, 0f);

            // Play the VFX
            vfx.Play();

            // Only play the light tweens after the VFX
            yield return PlayLightTweens(80f, 100f);

            // Finally, decrease light range and intensity
            yield return PlayLightTweens(2f, 0f);

            // Disable the force field GameObject
            forceField.gameObject.SetActive(false);
            pointLight.gameObject.SetActive(false);
        }

        private IEnumerator PlayTweens(Vector3 scale, float lightRange, float lightIntensity)
        {
            Tween scaleTween = forceField.transform.DOScale(scale, 0.2f).SetEase(Ease.InOutSine);
            Tween lightRangeTween = DOTween.To(() => pointLight.range, x => pointLight.range = x, lightRange, 0.2f).SetEase(Ease.InOutSine);
            Tween lightIntensityTween = DOTween.To(() => pointLight.intensity, x => pointLight.intensity = x, lightIntensity, 0.2f).SetEase(Ease.InOutSine);

            yield return DOTween.Sequence().Join(scaleTween).Join(lightRangeTween).Join(lightIntensityTween).Play().WaitForCompletion();
        }

        private IEnumerator PlayLightTweens(float lightRange, float lightIntensity)
        {
            Tween lightRangeTween = DOTween.To(() => pointLight.range, x => pointLight.range = x, lightRange, 0.2f).SetEase(Ease.InOutSine);
            Tween lightIntensityTween = DOTween.To(() => pointLight.intensity, x => pointLight.intensity = x, lightIntensity, 0.2f).SetEase(Ease.InOutSine);

            yield return DOTween.Sequence().Join(lightRangeTween).Join(lightIntensityTween).Play().WaitForCompletion();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                PlayEffect(transform.position);
            }
        }
    }
}
