using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxBlurController : MonoBehaviour
{
    [SerializeField]
    private Texture2D _srcTexture;

    [SerializeField]
    private ComputeShader _boxBlurComputeShader;

    [SerializeField, Range(1, 7)]
    private int _blurSize = 3;

    private RenderTexture _destTexture;

    private MaterialPropertyBlock _materialPropertyBlock;

    private MeshRenderer _meshRenderer;

    private int kernelID;

    void Start()
    {
        _destTexture = new RenderTexture(
            _srcTexture.width,
            _srcTexture.height,
            0,
            RenderTextureFormat.ARGB32
        );
        _destTexture.enableRandomWrite = true;
        _destTexture.Create();

        kernelID = _boxBlurComputeShader.FindKernel("CSMain");

        _boxBlurComputeShader.SetTexture(kernelID, "destTexture", _destTexture);
        _boxBlurComputeShader.SetTexture(kernelID, "srcTexture", _srcTexture);

        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    void Update()
    {
        _boxBlurComputeShader.SetInt("BlurSize", _blurSize);
        _boxBlurComputeShader.SetInt("TextureWidth", _srcTexture.width);
        _boxBlurComputeShader.SetInt("TextureHeight", _srcTexture.height);

        _boxBlurComputeShader.Dispatch(
            kernelID,
            _srcTexture.width,
            _srcTexture.height,
            1
        );

        _meshRenderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetTexture("_MainTex", _destTexture);
        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
