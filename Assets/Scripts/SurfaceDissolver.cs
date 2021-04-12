using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SurfaceDissolver : MonoBehaviour
{
    [SerializeField]
    private Texture2D _srcTexture;

    [SerializeField]
    private ComputeShader _maskColorComputeShader;

    [SerializeField]
    private bool _R = true;

    [SerializeField]
    private bool _G = true;

    [SerializeField]
    private bool _B = true;

    [SerializeField]
    private bool _A = true;

    // private ComputeShader _maskColorComputeShader;

    private RenderTexture _destTexture;

    private MaterialPropertyBlock _materialPropertyBlock;

    private MeshRenderer _meshRenderer;

    private int kernelID;

    // Start is called before the first frame update
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

        // _maskColorComputeShader = ComputeShader.Instantiate(Resources.Load<ComputeShader>("Shaders/MaskColor"));

        kernelID = _maskColorComputeShader.FindKernel("CSMain");

        _maskColorComputeShader.SetTexture(kernelID, "destTexture", _destTexture);
        _maskColorComputeShader.SetTexture(kernelID, "srcTexture", _srcTexture);

        _meshRenderer = GetComponent<MeshRenderer>();

        _materialPropertyBlock = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        _maskColorComputeShader.SetInt("MaskR", _R ? 1 : 0);
        _maskColorComputeShader.SetInt("MaskG", _G ? 1 : 0);
        _maskColorComputeShader.SetInt("MaskB", _B ? 1 : 0);
        _maskColorComputeShader.SetInt("MaskA", _A ? 1 : 0);

        _maskColorComputeShader.Dispatch(
            kernelID,
            _srcTexture.width,
            _srcTexture.height,
            1
        );
        // _maskColorComputeShader.Dispatch(kernelID, (int)(_srcTexture.width * 0.5), (int)(_srcTexture.height * 0.5), 1);
        // _maskColorComputeShader.Dispatch(kernelID, 10, 10, 1);

        _meshRenderer.GetPropertyBlock(_materialPropertyBlock);

        _materialPropertyBlock.SetTexture("_MainTex", _destTexture);

        _meshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }
}
