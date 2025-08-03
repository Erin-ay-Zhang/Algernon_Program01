using UnityEngine;

[ExecuteInEditMode]
public class EdgeBlurController : MonoBehaviour
{
    [SerializeField] private Material blurMaterial;

    [Header("Ä£ºýÉèÖÃ")]
    [Range(0, 0.1f)] public float blurRadius = 0.05f;
    [Range(0, 1)] public float edgeStart = 0.7f;
    [Range(0, 5)] public float blurIntensity = 1.5f;
    [Range(4, 16)] public int blurSamples = 8;

    void OnValidate()
    {
        UpdateMaterialProperties();
    }

    void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdateMaterialProperties();
        }
#endif
    }

    private void UpdateMaterialProperties()
    {
        if (blurMaterial != null)
        {
            blurMaterial.SetFloat("_BlurRadius", blurRadius);
            blurMaterial.SetFloat("_EdgeStart", edgeStart);
            blurMaterial.SetFloat("_BlurIntensity", blurIntensity);
            blurMaterial.SetInt("_BlurSamples", blurSamples);
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (blurMaterial != null)
        {
            Graphics.Blit(src, dest, blurMaterial);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}