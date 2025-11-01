using UnityEngine;

// プレハブを複数配置しても個別にマテリアルプロパティを設定するために
// MaterialPropertyBlock を使う実装に変更しました。
// - シェーダーはオブジェクト空間で計算する前提のため、_HalfExtents はメッシュ bounds の local extents を渡します。
// - public thickness は「ワールド単位の太さ」を想定します。見た目を保つため、オブジェクトの最大スケールでローカル単位に変換して渡します。
// - 共有マテリアルは変更せず、レンダラー単位でプロパティを上書きします。

[ExecuteAlways]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BoxFrameSetup : MonoBehaviour
{
    [Tooltip("Edge thickness in world units (e.g. 0.02). 表示上の太さ（ワールド単位）")]
    public float thickness = 0.02f;

    Renderer _renderer;
    MeshFilter _mf;
    MaterialPropertyBlock _mpb;

    void OnEnable()
    {
        _renderer = GetComponent<Renderer>();
        _mf = GetComponent<MeshFilter>();
        _mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        if (_renderer == null || _mf == null || _mf.sharedMesh == null)
            return;

        // メッシュの bounds.extents はローカル空間の半寸法（object-space half extents）
        Vector3 meshHalfExtentsLocal = _mf.sharedMesh.bounds.extents;

        // オブジェクトのワールドスケール（lossyScale）が 0 の軸があるとまずいため保護
        Vector3 ls = transform.lossyScale;
        float maxScale = Mathf.Max(Mathf.Abs(ls.x), Mathf.Abs(ls.y), Mathf.Abs(ls.z));
        if (maxScale <= 0f)
            maxScale = 1f;

        // ワールド単位の thickness をローカル単位に変換する（maxScale で割る）
        // 単一の _Thickness を扱うため簡便に最大スケール基準で変換している。
        float thicknessLocal = thickness / maxScale;

        // MPB にプロパティをセット（レンダラーごとの上書き、共有マテリアルは変更しない）
        _renderer.GetPropertyBlock(_mpb);
        _mpb.SetVector(
            "_HalfExtents",
            new Vector4(meshHalfExtentsLocal.x, meshHalfExtentsLocal.y, meshHalfExtentsLocal.z, 0f)
        );
        _mpb.SetFloat("_Thickness", thicknessLocal);
        _renderer.SetPropertyBlock(_mpb);
    }
}
