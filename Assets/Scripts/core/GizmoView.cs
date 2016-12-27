using UnityEngine;
using System.Linq;

public class GizmoView : MonoBehaviour
{
    public bool wireframe = true;
    public float size = 1.0f;
    public string text = "";

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        // Draw the gizmo
        Gizmos.matrix = transform.localToWorldMatrix;
        var selected =
            UnityEditor.Selection.gameObjects.Any(
                x => x == gameObject || (transform.parent != null && x == transform.parent.gameObject));

        Gizmos.color = selected ? Color.green : Color.yellow;

        if (wireframe) Gizmos.DrawWireSphere(Vector3.zero, size);
        else Gizmos.DrawSphere(Vector3.zero, size);

        var sz = Vector3.Scale(transform.lossyScale, new Vector3(size, size, size));

        UnityEditor.Handles.Label(transform.position + sz, text.Length == 0 ? name : text);
    }
#endif
}