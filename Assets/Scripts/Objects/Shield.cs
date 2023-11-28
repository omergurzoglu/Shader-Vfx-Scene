using System;
using UnityEngine;

namespace Objects
{
    public class Shield : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                gameObject.SetActive(!gameObject.activeSelf);
            }
        }
    }
}