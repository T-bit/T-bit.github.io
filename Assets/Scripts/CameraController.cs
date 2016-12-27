using UnityEngine;
#if UNITY_EDITOR
using System.Linq;
#endif

public class CameraController : MonoBehaviour
{
    public float horizontalSlideCamPercent = 5;
    public float verticalSlideCamPercent = 5;
    public Bounds cameraBounds = new Bounds(Vector3.zero, new Vector3(100, 100, 100));
    public RangeF cameraFOVRange = new RangeF {min = 10, max = 80};

    public float cameraMoveSpeed = 10;
    public float cameraZoomSpeed = 10;

    public Camera cam { get; private set; }

    public bool isInit { get; private set; }

    private void _init()
    {
        if (isInit) return;

        cam = GetComponent<Camera>();
        if (cam == null)
        {
            Debug.LogError("CameraController: Camera component is requred!");
            return;
        }

        isInit = true;
    }

    private void Start()
    {
        _init();
    }

    private void Update()
    {
        if (!isInit) return;

        var dt = Time.deltaTime;
        var mousePos = Input.mousePosition;
        var horizontalSpeed = mousePos.x >= 0 && mousePos.x <= horizontalSlideCamPercent
            ? -1
            : mousePos.x <= Screen.width && mousePos.x >= Screen.width - horizontalSlideCamPercent
                ? 1
                : 0;
        var verticalSpeed = mousePos.y >= 0 && mousePos.y <= verticalSlideCamPercent
            ? -1
            : mousePos.y <= Screen.height && mousePos.y >= Screen.height - verticalSlideCamPercent
                ? 1
                : 0;

        if (horizontalSpeed != 0 || verticalSpeed != 0)
        {
            move(horizontalSpeed*dt, verticalSpeed*dt);
        }
        else
        {
            move(Input.GetAxis("Horizontal") * dt, Input.GetAxis("Vertical") * dt);
        }

        var scroll = -Input.mouseScrollDelta.y;
        cam.fieldOfView += scroll * dt * cameraZoomSpeed;
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, cameraFOVRange.min, cameraFOVRange.max);
    }

    public void move(float horizontalSpeed, float verticalSpeed)
    {
        var pos = transform.position + new Vector3(horizontalSpeed*cameraMoveSpeed, 0, verticalSpeed*cameraMoveSpeed);

        if (pos.x < cameraBounds.min.x)
        {
            pos.x = cameraBounds.min.x;
        }
        else if (pos.x > cameraBounds.max.x)
        {
            pos.x = cameraBounds.max.x;
        }

        if (pos.y < cameraBounds.min.y)
        {
            pos.y = cameraBounds.min.y;
        }
        else if (pos.y > cameraBounds.max.y)
        {
            pos.y = cameraBounds.max.y;
        }

        if (pos.z < cameraBounds.min.z)
        {
            pos.z = cameraBounds.min.z;
        }
        else if (pos.z > cameraBounds.max.z)
        {
            pos.z = cameraBounds.max.z;
        }

        transform.position = pos;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (UnityEditor.Selection.gameObjects.Any(
            x => x == gameObject || (transform.parent != null && x == transform.parent.gameObject)))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(cameraBounds.center, cameraBounds.size);
        }
    }
#endif
}