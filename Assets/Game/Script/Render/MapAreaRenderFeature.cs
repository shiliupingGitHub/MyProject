using System.Collections.Generic;
using Game.Script.Map;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MapAreaRenderFeature : ScriptableRendererFeature
{
    
    class CustomRenderPass : ScriptableRenderPass
    {
        // This method is called before executing the render pass.
        // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
        // When empty this render pass will render to the active camera render target.
        // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
        // The render pipeline will ensure target setup and clearing happens in a performant manner.
        
        private Mesh _gridMesh;
        public Material drawMaterial = null;
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        // Here you can implement the rendering logic.
        // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
        // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
        // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            MapScript mapScript = GameObject.FindObjectOfType<MapScript>();

            if (mapScript == null)
            {
                return;
            }

            if (!mapScript.showGrid)
            {
                return;
            }

            if (renderingData.cameraData.camera.cullingMask == 32)
            {
                return;
            }

            if (null != drawMaterial)
            {
                CreateMesh(mapScript);
                CommandBuffer cmd = CommandBufferPool.Get();
                
                cmd.DrawMesh(_gridMesh, Matrix4x4.identity, drawMaterial, 0);
                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
           
            
        }

        void CreateMesh(MapScript script)
        {
            if (null == _gridMesh)
            {
                _gridMesh = new Mesh();
            }
            _gridMesh.Clear();

            Grid grid = script.MyGrid;

            if (null == grid)
            {
                return;
            }
            
            int index = 0;
            Vector3 start = script.transform.position;
            List<int> indices = new();
            List<Vector3> vertices = new();
            for (int x = 0; x <= script.xGridNum; x++)
            {
                var v0 = start + new Vector3(x * grid.cellSize.x, 0, 0);
                vertices.Add(v0);
                indices.Add(index);
                index++;
                
                var v1 = start + new Vector3(x * grid.cellSize.x, script.yGridNum * grid.cellSize.y, 0);
                vertices.Add(v1);
                indices.Add(index);
                index++;
          
            }
            
            for (int y = 0; y <= script.yGridNum; y++)
            {
                var v0 = start + new Vector3(0,  y * grid.cellSize.y, 0);
                vertices.Add(v0);
                indices.Add(index);
                index++;
                
                var v1 = start + new Vector3(script.xGridNum * grid.cellSize.x , y * grid.cellSize.y , 0);
                vertices.Add(v1);
                indices.Add(index);
                index++;
          
            }

            _gridMesh.SetVertices(vertices);
            _gridMesh.SetIndices(indices, MeshTopology.Lines, 0);


        }
        

        // Cleanup any allocated resources that were created during the execution of this render pass.
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }

    CustomRenderPass m_ScriptablePass;

    public Material drawMaterial = null;
    /// <inheritdoc/>
    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass();
        m_ScriptablePass.drawMaterial = drawMaterial;
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRendering;
        
    }



    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


