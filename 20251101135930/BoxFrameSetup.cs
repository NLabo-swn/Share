using UnityEngine;

// Attach to the box GameObject. Assign the material used by the renderer (shared material ok).
[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class BoxFrameSetup : MonoBehaviour
{
    public Material targetMaterial;

    [Tooltip("Edge thickness in world units (e.g. 0.02)")]
    public float thickness = 0.02f;

    void Update()
    {
        if (targetMaterial == null)
            return;
        var mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
            return;

        // Mesh bounds extents are in local space; convert to world extents by lossyScale.
        Vector3 meshHalfExtentsLocal = mf.sharedMesh.bounds.extents; // local-space half extents
        Vector3 worldHalfExtents = Vector3.Scale(meshHalfExtentsLocal, transform.lossyScale);

        targetMaterial.SetVector(
            "_HalfExtents",
            new Vector4(worldHalfExtents.x, worldHalfExtents.y, worldHalfExtents.z, 0)
        );
        targetMaterial.SetVector(
            "_WorldCenter",
            new Vector4(transform.position.x, transform.position.y, transform.position.z, 0)
        );
        targetMaterial.SetFloat("_Thickness", thickness);
    }
}
