using System.Collections;
using UnityEngine;

public class ShaderTest : MonoBehaviour
{
    public ComputeShader computeShader;
    private int width = 256, height = 256;
    public RenderTexture outputTexture;
    public Color targetColor = Color.blue;

    [ContextMenu("Run Calculate Shader")]
    public void RunCalculateShader()
    {
        string kernelName = "CSMain";

        int kernelHandle = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kernelHandle, "Result", outputTexture);
        computeShader.SetVector("color", targetColor);
        computeShader.Dispatch(kernelHandle, 
            outputTexture.width / 8,
            outputTexture.height / 8,
            1);
    }

    [ContextMenu("TEst")]
    void RunTest()
    {
        string kernelName = "CSMain";
        int kernelHandle = computeShader.FindKernel(kernelName);
        computeShader.SetTexture(kernelHandle, "Result", outputTexture);
        computeShader.SetVector("color", targetColor);
        var threadGroupsX = Mathf.CeilToInt((float)outputTexture.width / 8);
        var threadGroupsY = Mathf.CeilToInt((float)outputTexture.height / 8);

        computeShader.Dispatch(kernelHandle, threadGroupsX, threadGroupsY, 1);
    }

}