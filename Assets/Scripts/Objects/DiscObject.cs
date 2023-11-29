using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Objects;
using UnityEngine;
using UnityEngine.VFX;
using Random = UnityEngine.Random;
namespace Disc
{
    public class DiscObject : MonoBehaviour
    {
        public bool discOnMotion;
        private new Camera camera;
        [SerializeField] private float verticalMoveDistance = 0.5f; // The distance the disc moves up and down
        [SerializeField] private float verticalMoveDuration = 2.0f; // Duration for one complete up and down cycle
        [SerializeField] private int minPoints = 10; // Minimum number of points in the Bezier path
        [SerializeField] private int maxPoints = 30; // Maximum number of points in the Bezier path
        [SerializeField] private float distanceFactor = 0.5f; // Factor to control curve smoothness
        [SerializeField] private float constantSpeed = 5f; // Speed of the disc along the path
        [SerializeField] private Transform discParentTransform; // Reference to the player's transform
        [SerializeField] private float returnSpeed = 5f; // Speed at which the disc returns to the player
        [SerializeField] private float rotationSmoothing = 0.1f; // Smoothing factor for rotation
        [SerializeField] private VisualEffect discImpactEffect;

        private Vector3 controlPoint1;
        private Vector3 controlPoint2;
        private Tween verticalTween;
        private List<Vector3> bezierPathPoints;
        private Quaternion initialLocalRotation;
        [SerializeField]private LayerMask discLayerMask;
      
        
        private void Start()
        {
            camera=Camera.main;
            Vector3 currentRotation = transform.localEulerAngles;
            Vector3 targetRotation = new Vector3(currentRotation.x, currentRotation.y, currentRotation.z + 360);
            transform.DOLocalRotate(targetRotation, 0.1f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
            
            Vector3 startPosition = transform.localPosition;
            Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + verticalMoveDistance, startPosition.z);
            verticalTween= transform.DOLocalMove(endPosition, verticalMoveDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo); 

            StartCoroutine(CacheLocal());
        }

        private IEnumerator CacheLocal()
        {
            yield return new WaitForSeconds(1f);
            initialLocalRotation = transform.rotation;
        }
        
         private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                verticalTween?.Pause();
                Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
                if (Physics.Raycast(ray, out var hit,150f,discLayerMask))
                {
                    List<Vector3> pathPoints = GenerateBezierPath(transform.position, hit.point, Random.Range(15f,30f), 20);
                    transform.SetParent(null);
                    StartCoroutine(MoveAlongPath(pathPoints));
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                ReturnToStart();
            }
        }
         public void ReturnToStart()
         {
             StartCoroutine(ReturnToStartPosition());
         }
         private IEnumerator ReturnToStartPosition()
         {
             Vector3 startPosition = transform.position;
             Quaternion startRotation = transform.rotation; // Use the current rotation as the start

             while (Vector3.Distance(transform.position, discParentTransform.position) > 1.10f)
             {
                 Vector3 targetPosition = discParentTransform.position + discParentTransform.forward; // Adjust as needed
                 transform.position = Vector3.MoveTowards(transform.position, targetPosition, returnSpeed * Time.deltaTime);
                 Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
                 transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothing);
                 yield return null;
             }
             transform.position = discParentTransform.position;
             transform.rotation = initialLocalRotation;
             transform.SetParent(discParentTransform);
             verticalTween.Play();
         }

         private void OnTriggerEnter(Collider other)
         {
             if (other.TryGetComponent<Dummy>(out var dummy))
             {
                 Vector3 hitDirection = (dummy.transform.position - discParentTransform.position).normalized;
                 Debug.Log(hitDirection);
                 dummy.DeathVfx(hitDirection);
                 dummy.Dissolve();
                 Debug.DrawRay(transform.position, hitDirection * 5f, Color.red, 2f);
             }
         }


         #region Bezier
        
         private IEnumerator MoveAlongPath(List<Vector3> pathPoints )
        {
            
            Vector3 startPosition = transform.position;
            Vector3 endPosition = pathPoints[pathPoints.Count - 1];
            float distance = Vector3.Distance(startPosition, endPosition);
            float duration = distance / constantSpeed;  //constant speed here*****

            float time = 0;
            int currentPointIndex = 0;
            Vector3 globalUp = Vector3.up;

            while (currentPointIndex < pathPoints.Count - 1)
            {
                Vector3 startPoint = pathPoints[currentPointIndex];
                Vector3 endPoint = pathPoints[currentPointIndex + 1];

                while (time < duration)
                {
                    float t = time / duration;
                    transform.position = Vector3.Lerp(startPoint, endPoint, t);
                    
                    Vector3 tangent = CalculateBezierTangent(t, startPoint, controlPoint1, controlPoint2, endPoint).normalized;
                    Vector3 binormal = Vector3.Cross(tangent, globalUp).normalized;
                    Vector3 normal = Vector3.Cross(binormal, tangent).normalized;
                    
                    Quaternion targetRotation = Quaternion.FromToRotation(Vector3.forward, normal);
                    transform.rotation = targetRotation;

                    time += Time.deltaTime;
                    yield return null;
                }

                time = 0;
                currentPointIndex++;
            }
            
            transform.position = pathPoints[pathPoints.Count - 1];
            Vector3 finalTangent = CalculateBezierTangent(1, pathPoints[pathPoints.Count - 2], controlPoint1, controlPoint2, pathPoints[pathPoints.Count - 1]).normalized;
            Vector3 finalBinormal = Vector3.Cross(finalTangent, globalUp).normalized;
            Vector3 finalNormal = Vector3.Cross(finalBinormal, finalTangent).normalized;
            Quaternion finalRotation = Quaternion.FromToRotation(Vector3.forward, finalNormal);
            transform.rotation = finalRotation;
            ReturnToStart();
        }
        
        private Vector3 CalculateBezierTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float uu = u * u;
            float tt = t * t;
            Vector3 tangent = -3 * uu * p0 + 3 * uu * p1 - 6 * u * t * p1 - 3 * tt * p2+ 6 * u * t * p2+ 3 * tt * p3;
            return tangent.normalized;
        }

        public Vector3 GenerateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            Vector3 p = uuu * p0; 
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2; 
            p += ttt * p3; 
            return p;
        }
        public List<Vector3> GenerateBezierPath(Vector3 startPosition, Vector3 endPosition, float angle, float distance)
        {
            int numPoints = Mathf.Clamp(Mathf.RoundToInt(distance / distanceFactor), minPoints, maxPoints);
            List<Vector3> pathPoints = new List<Vector3>();
            Vector3 direction = (endPosition - startPosition).normalized;
            Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 randomDirection = rotation * direction;
            controlPoint1 = startPosition + randomDirection * (Random.Range(0.25f, 0.75f) * (endPosition - startPosition).magnitude);
            controlPoint2 = endPosition - randomDirection * (Random.Range(0.25f, 0.75f) * (endPosition - startPosition).magnitude);
            for (int i = 0; i <= numPoints; i++)
            {
                float t = i / (float)numPoints;
                pathPoints.Add(GenerateBezierPoint(t, startPosition, controlPoint1, controlPoint2, endPosition));
            }
            bezierPathPoints = pathPoints;
            return pathPoints;
        }
        private void OnDrawGizmos()
        {
            if (bezierPathPoints != null && bezierPathPoints.Count > 1)
            {
                Gizmos.color = Color.red; 
                for (int i = 0; i < bezierPathPoints.Count - 1; i++)
                {
                    Gizmos.DrawLine(bezierPathPoints[i], bezierPathPoints[i + 1]);
                }
                Gizmos.color = Color.green;
                Vector3 globalUp = Vector3.up;
                for (int i = 0; i < bezierPathPoints.Count - 1; i++)
                {
                    float t = i / (float)(bezierPathPoints.Count - 1);
                    Vector3 tangent = CalculateBezierTangent(t, bezierPathPoints[0], controlPoint1, controlPoint2,bezierPathPoints[bezierPathPoints.Count - 1]).normalized;
                    Vector3 binormal =Vector3.Cross(tangent, globalUp).normalized; 
                    Vector3 normal =Vector3.Cross(binormal, tangent).normalized; 

                    Vector3 midPoint = Vector3.Lerp(bezierPathPoints[i], bezierPathPoints[i + 1], 0.5f);

                    Gizmos.DrawLine(midPoint,midPoint + normal * 0.5f); 
                }
            }
        }
        #endregion
        
    }
}