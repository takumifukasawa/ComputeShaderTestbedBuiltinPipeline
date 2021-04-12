using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddFloatController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _computeShader;

    [SerializeField]
    private float _a = 1;

    [SerializeField]
    private float _b = 2;

    private ComputeBuffer _buffer;

    void Start()
    {
        int kernelIndex = _computeShader.FindKernel("CSAddFloat");
        // int kernelIndex = 0;

        _buffer = new ComputeBuffer(1, sizeof(float));

        _computeShader.SetBuffer(kernelIndex, "buffer", _buffer);

        _computeShader.SetFloat("a", _a);
        _computeShader.SetFloat("b", _a);

        _computeShader.Dispatch(kernelIndex, 1, 1, 1);

        float[] result = new float[1];

        _buffer.GetData(result);

        Debug.Log("--- add ---");
        Debug.Log("a: " + _a + ", b: " + _b + ", result: " + result[0]);
    }

    void OnDisable()
    {
        Dispose();
    }

    void OnDestroy()
    {
        Dispose();
    }

    void Dispose()
    {
        _buffer?.Release();
        _buffer = null;
    }
}
