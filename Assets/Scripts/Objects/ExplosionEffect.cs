using System;
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
            GetComponent<Light>();
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

            // Start scale up and light range increase animations
            Tween scaleUp = forceField.transform.DOScale(10f, 0.2f).SetEase(Ease.InOutSine);
            Tween lightIncrease = DOTween.To(() => pointLight.range, x => pointLight.range = x, 40f, 0.2f).SetEase(Ease.InOutSine);
            yield return DOTween.Sequence().Join(scaleUp).Join(lightIncrease).Play().WaitForCompletion();

            // Start scale down and light range decrease animations
            Tween scaleDown = forceField.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InOutSine);
            Tween lightDecrease = DOTween.To(() => pointLight.range, x => pointLight.range = x, 5f, 0.2f).SetEase(Ease.InOutSine);
            yield return DOTween.Sequence().Join(scaleDown).Join(lightDecrease).Play().WaitForCompletion();

            // Play the VFX and increase light range again
            vfx.Play();
            Tween lightIncreaseAgain = DOTween.To(() => pointLight.range, x => pointLight.range = x, 40f, 0.2f).SetEase(Ease.InOutSine);
            yield return lightIncreaseAgain.WaitForCompletion();

            // Decrease light range again
            Tween lightDecreaseFinal = DOTween.To(() => pointLight.range, x => pointLight.range = x, 2f, 0.2f).SetEase(Ease.InOutSine);
            yield return lightDecreaseFinal.WaitForCompletion();

            // Disable the force field GameObject
            forceField.gameObject.SetActive(false);
            pointLight.gameObject.SetActive(false);
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