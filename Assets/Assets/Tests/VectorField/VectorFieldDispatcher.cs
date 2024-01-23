using System;
using UnityEngine;
using UnityEngine.Serialization;

public class VectorFieldDispatcher : MonoBehaviour
{
    public RenderTexture RenderTextureResult;
    public ComputeShader DrawShader;
    public ComputeShader VectorFieldShader;
    

    public Color Color = UnityEngine.Color.red;
    public int BrushSize = 3;
    public float SpreadRadius = 1.0f;
    
    int vectorShader;
    int drawShader;
    
    private static readonly int Result = Shader.PropertyToID("Result");
    private static readonly int ColorID = Shader.PropertyToID("color");
    private static readonly int Size = Shader.PropertyToID("brushSize");
    private static readonly int Pixel = Shader.PropertyToID("pixel");
    private static readonly int Radius = Shader.PropertyToID("Radius");
    private static readonly int TextureSize = Shader.PropertyToID("TextureSize");

    private void Awake()
    {
        string kernelName = "CSMain";
        drawShader = DrawShader.FindKernel(kernelName);
        vectorShader = VectorFieldShader.FindKernel(kernelName);

        DrawShader.SetTexture(drawShader, Result, RenderTextureResult);
        VectorFieldShader.SetTexture(vectorShader, Result, RenderTextureResult);
        VectorFieldShader.SetInts(TextureSize, new []{RenderTextureResult.width, RenderTextureResult.height});
    }

    private void Update()
    {
        var threadGroupsX = Mathf.CeilToInt((float)RenderTextureResult.width / 8);
        var threadGroupsY = Mathf.CeilToInt((float)RenderTextureResult.height / 8);

        
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, -10);
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 10000))
        {
            if (Input.GetMouseButton(0))
            {
                Vector2 pixelCoord = new Vector2(hit.textureCoord.x * RenderTextureResult.width, hit.textureCoord.y * RenderTextureResult.height);
                int[] pixel = { (int)pixelCoord.x, (int)pixelCoord.y };
                DrawShader.SetVector(ColorID, Color);
                DrawShader.SetInt(Size, BrushSize);
                DrawShader.SetInts(Pixel, pixel); 
                DrawShader.Dispatch(drawShader, threadGroupsX, threadGroupsY, 1);
            }
        }
    
        VectorFieldShader.SetInt(Radius, (int)SpreadRadius); 
        VectorFieldShader.Dispatch(vectorShader, threadGroupsX, threadGroupsY, 1);
    }
    
}
