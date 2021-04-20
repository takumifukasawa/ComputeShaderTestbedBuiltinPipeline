using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyTextureController : MonoBehaviour
{
    [SerializeField]
    private Texture2D _srcTexture;

    [SerializeField]
    private ComputeShader _copyTextureComputeShader;

    private RenderTexture _destTexture;

    private MaterialPropertyBlock _materialPropertyBlock;

    private MeshRenderer _meshRenderer;

    private int kernelID;

    void Start()
    {
        // ---------------------------------------------------------------------------
        // create texture
        // ---------------------------------------------------------------------------

        _destTexture = new RenderTexture(
            _srcTexture.width,
            _srcTexture.height,
            0,
            RenderTextureFormat.ARGB32
        );
        _destTexture.enableRandomWrite = true;
        _destTexture.Create();

        // ---------------------------------------------------------------------------
        // compute shader
        // ---------------------------------------------------------------------------

        kernelID = _copyTextureComputeShader.FindKernel("CSMain");

        _copyTextureComputeShader.SetTexture(kernelID, "destTexture", _destTexture);
        _copyTextureComputeShader.SetTexture(kernelID, "srcTexture", _srcTexture);

        // ---------------------------------------------------------------------------
        // init material property block
        // ---------------------------------------------------------------------------

        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        // ---------------------------------------------------------------------------
        // dispatch compute shader
        // ---------------------------------------------------------------------------

        _copyTextureComputeShader.Dispatch(
            kernelID,
            _srcTexture.width,
            _srcTexture.height,
            1
        );

        // ---------------------------------------------------------------------------
        // set texture to material
        // ---------------------------------------------------------------------------

        _meshRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetTexture("_MainTex", _destTexture);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
