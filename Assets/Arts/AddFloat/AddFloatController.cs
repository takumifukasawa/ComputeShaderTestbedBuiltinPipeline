using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 
 * 2つの float を足し算する compute shader を操作する
 * 
 */
public class AddFloatController : MonoBehaviour
{
    [SerializeField]
    private ComputeShader _computeShader;

    [SerializeField]
    private float _a = 1;

    [SerializeField]
    private float _b = 2;

    private ComputeBuffer _buffer;

    // -----------------------------------------------------------------
    // life cycles
    // -----------------------------------------------------------------

    void Start()
    {
        Exec();
    }

    void OnDisable()
    {
        Dispose();
    }

    void OnDestroy()
    {
        Dispose();
    }

    // -----------------------------------------------------------------
    // 実行関数
    // -----------------------------------------------------------------

    void Exec()
    {
        // -----------------------------------------------------------------
        // 使用する compute shader の関数（kernel: カーネル）を取得
        // - 同じファイル内に複数入っている場合など、indexで判別できる
        // - 関数が一個だけの場合は index = 0 を直指定でも動く
        // -----------------------------------------------------------------

        int kernelIndex = _computeShader.FindKernel("CSAddFloat");
        // int kernelIndex = 0;

        // -----------------------------------------------------------------
        // 読み書き用のbufferを生成
        // - 足し算の結果だけ取得できればよいので、長さ1のfloatバッファ
        // -----------------------------------------------------------------

        _buffer = new ComputeBuffer(1, sizeof(float));

        // -----------------------------------------------------------------
        // compute shader に buffer を割り当てる 
        // - 足し算の結果だけ取得できればよいので、長さ1のfloatバッファ
        // - 引数
        //   - 第一引数: compute shader の関数のindex
        //   - 第二引数: 名前
        //   - 第三引数: buffer
        // -----------------------------------------------------------------

        _computeShader.SetBuffer(kernelIndex, "buffer", _buffer);

        // -----------------------------------------------------------------
        // compute shader に値を送る
        // - 足し算の結果だけ取得できればよいので、長さ1のfloatバッファ
        // - kernel で共通の値になるので、複数 kernel 存在する場合は注意
        // -----------------------------------------------------------------

        _computeShader.SetFloat("a", _a);
        _computeShader.SetFloat("b", _a);

        // -----------------------------------------------------------------
        // kernel index (kernelの場所) を指定して実行
        // - 第二引数以降は thread の x,y,z 次元
        // -----------------------------------------------------------------

        _computeShader.Dispatch(kernelIndex, 1, 1, 1);

        // -----------------------------------------------------------------
        // buffer経由で計算結果を受け取る
        // - 計算結果は float 1つなので 長さ1のfloat配列を作って渡す
        // -----------------------------------------------------------------

        float[] result = new float[1];

        _buffer.GetData(result);

        // -----------------------------------------------------------------
        // 計算結果出力
        // -----------------------------------------------------------------

        Debug.Log("--- AddFloat ---");
        Debug.Log("a: " + _a + ", b: " + _b + ", result: " + result[0]);
    }

    // -----------------------------------------------------------------
    // 破棄処理
    // - bufferを明示的に開放してあげる
    // -----------------------------------------------------------------

    void Dispose()
    {
        _buffer?.Release();
        _buffer = null;
    }
}
