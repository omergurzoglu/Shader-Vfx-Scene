using System.Collections;
using UnityEngine;

namespace Utility
{
    public class Util : MonoBehaviour
    {
        private IEnumerator MoveAndRotateArc(Vector3 startPosition, Vector3 endPosition, float duration)
        {
            Debug.Log("MoveAndRotateArc started.");
            float time = 0;
            Quaternion startRotation = Quaternion.LookRotation(startPosition - transform.position);
            Quaternion endRotation = Quaternion.LookRotation(endPosition - transform.position);

            Vector3 directionToEnd = endPosition - startPosition;
            float distance = directionToEnd.magnitude;

            while (time < duration)
            {
                float t = time / duration;

                // Slerp rotation
                Quaternion rotation = Quaternion.Slerp(startRotation, endRotation, t);
                transform.rotation = rotation;

                // Move along the arc
                transform.position = startPosition + rotation * Vector3.forward * (distance * t);

                time += Time.deltaTime;
                yield return null;
            }

            // Ensure the disc is exactly at the end position and rotation after the movement
            transform.position = endPosition;
            transform.rotation = endRotation;
            Debug.Log("MoveAndRotateArc ended.");
        }


        public void StartMoveAndRotateArc(Vector3 endPosition)
        {
            Debug.Log("call one ");
            Vector3 startPosition = transform.position;
            StartCoroutine(MoveAndRotateArc(startPosition, endPosition, 1.5f)); // Adjust duration as needed
        }
    }
}