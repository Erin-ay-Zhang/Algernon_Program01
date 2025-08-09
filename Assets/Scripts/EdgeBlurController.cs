using UnityEngine;

[ExecuteInEditMode]
public class EdgeBlurController : MonoBehaviour
{
    public Shader edgeBlurShader;
    private Material edgeBlurMaterial;

    [Range(0, 10)]
    public float blurSize = 3f;

    [Range(0, 1)]
    public float edgeStart = 0.3f;

    [Range(0, 1)]
    public float edgeEnd = 0.7f;

    private void Start()
    {
        if (edgeBlurShader == null)
        {
            Debug.LogError("请在 Inspector 里指定 EdgeBlur Shader");
            enabled = false;
            return;
        }

        edgeBlurMaterial = new Material(edgeBlurShader);
    }

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (edgeBlurMaterial != null)
        {
            edgeBlurMaterial.SetFloat("_BlurSize", blurSize);
            edgeBlurMaterial.SetFloat("_EdgeStart", edgeStart);
            edgeBlurMaterial.SetFloat("_EdgeEnd", edgeEnd);

            Graphics.Blit(src, dest, edgeBlurMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
