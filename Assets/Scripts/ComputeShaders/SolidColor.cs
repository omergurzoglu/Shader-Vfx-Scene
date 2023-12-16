
using UnityEngine;

namespace ComputeShaders
{
    public class SolidColor : MonoBehaviour
    {
        public ComputeShader computeShader;
        public string kernelName = "SolidRed";
        public int textResolution = 256;
        private new Renderer renderer;
        private RenderTexture outputTexture;
        private int kernelHandle;
        

        private void Start()
        {
            outputTexture = new RenderTexture(textResolution, textResolution, 0);
            outputTexture.enableRandomWrite = true;
            outputTexture.Create();
            renderer = GetComponent<Renderer>();
            renderer.enabled = true;
            InitShader();
        }

        private void InitShader()
        {
            kernelHandle = computeShader.FindKernel(kernelName);
            computeShader.SetInt("textResolution",textResolution);
            computeShader.SetTexture(kernelHandle,"Result",outputTexture);
            renderer.material.SetTexture("_BaseMap",outputTexture);
            DispatchShader(textResolution/16,textResolution/16);  
        }

        private void DispatchShader(int x,int y)
        {
            computeShader.Dispatch(kernelHandle,x,y,1);
        }

        private void Update()
        {
            computeShader.SetFloat("time", Time.time);
            DispatchShader(textResolution / 8, textResolution / 8);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                DispatchShader(textResolution/8,textResolution/8);
            }
        }
    }
}