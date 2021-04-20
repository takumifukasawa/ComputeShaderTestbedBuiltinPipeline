using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBlurController : MonoBehaviour
{
    [SerializeField]
    private Texture2D _srcTexture;

    [SerializeField]
    private ComputeShader _boxBlurComputeShader;

    [SerializeField, Range(1, 10)]
    private int _blurSize = 3;

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
        // init compute shader
        // ---------------------------------------------------------------------------

        kernelID = _boxBlurComputeShader.FindKernel("CSMain");

        _boxBlurComputeShader.SetTexture(kernelID, "destTexture", _destTexture);
        _boxBlurComputeShader.SetTexture(kernelID, "srcTexture", _srcTexture);

        _boxBlurComputeShader.SetInt("TextureWidth", _srcTexture.width);
        _boxBlurComputeShader.SetInt("TextureHeight", _srcTexture.height);

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

        _boxBlurComputeShader.SetInt("BlurSize", _blurSize);

        _boxBlurComputeShader.Dispatch(
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
