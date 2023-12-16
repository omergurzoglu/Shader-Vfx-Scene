// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal;
//
// public class RingHighlightFeature : ScriptableRendererFeature
// {
//     class RingHighlightPass : ScriptableRenderPass
//     {
//         private Material ringHighlightMaterial;
//         private RenderTargetIdentifier source;
//         private RenderTargetHandle temporaryTextureHandle;
//
//         public RingHighlightPass(Material material)
//         {
//             ringHighlightMaterial = material;
//             temporaryTextureHandle.Init("_TemporaryColorTexture");
//         }
//
//         public void Setup(RenderTargetIdentifier sourceIdentifier)
//         {
//             this.source = sourceIdentifier;
//         }
//
//         public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
//         {
//             CommandBuffer cmd = CommandBufferPool.Get("RingHighlightPass");
//
//             RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
//             opaqueDesc.depthBufferBits = 0;
//             
//             cmd.GetTemporaryRT(temporaryTextureHandle.id, opaqueDesc, FilterMode.Bilinear);
//             
//             Blit(cmd, source, temporaryTextureHandle.Identifier(), ringHighlightMaterial, 0);
//             Blit(cmd, temporaryTextureHandle.Identifier(), source);
//
//             context.ExecuteCommandBuffer(cmd);
//             CommandBufferPool.Release(cmd);
//         }
//
//         public override void FrameCleanup(CommandBuffer cmd)
//         {
//             cmd.ReleaseTemporaryRT(temporaryTextureHandle.id);
//         }
//     }
//
//     public Material ringHighlightMaterial;
//     private RingHighlightPass ringHighlightPass;
//
//     public override void Create()
//     {
//         ringHighlightPass = new RingHighlightPass(ringHighlightMaterial)
//         {
//             renderPassEvent = RenderPassEvent.AfterRenderingOpaques
//         };
//     }
//         
//
//     public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
//     {
//         ringHighlightPass.Setup(renderer.cameraColorTarget);
//         renderer.EnqueuePass(ringHighlightPass);
//     }
// }
