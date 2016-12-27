using UnityEngine;

public class Building : MonoBehaviour, IPoolObject
{
    public string info;
    public Material material = null;
    public Material transparentMaterial = null;
    public MeshRenderer mesh = null;
    public Transform quadsContainer = null;
    public MeshRenderer[] quads = null;

    public bool halfX;
    public bool halfY;

    public bool canBuild { get; private set; }

    private bool _firstCheckDone;

    public void setPosition(Vector3 pos)
    {
        transform.position = pos;

        if (quadsContainer != null)
        {
            pos.x = halfX == GameManager.Instance.fieldHalfX ? Mathf.Round(pos.x) : Mathf.Round(pos.x + 0.5f) - 0.5f;
            pos.z = halfY == GameManager.Instance.fieldHalfY ? Mathf.Round(pos.z) : Mathf.Round(pos.z + 0.5f) - 0.5f;
            var check = !_firstCheckDone ||
                        Mathf.FloorToInt(quadsContainer.position.x) != Mathf.FloorToInt(pos.x) ||
                        Mathf.FloorToInt(quadsContainer.position.z) != Mathf.FloorToInt(pos.z);
            quadsContainer.position = pos;
            if (check)
            {
                _firstCheckDone = true;
                _checkCellsAvailability();
            }
        }
    }

    public void destroy()
    {
        if (quads != null)
        {
            for (var i = 0; i < quads.Length; ++i)
            {
                var quad = quads[i];
                GameManager.Instance.setCellAvailability(quad.transform.position, false);
            }
        }
        GameManager.Instance.pool.unlockObj(this);
    }

    public bool build()
    {
        _checkCellsAvailability();
        if (!canBuild) return false;

        canBuild = false;
        if (quads != null)
        {
            for (var i = 0; i < quads.Length; ++i)
            {
                var quad = quads[i];
                GameManager.Instance.setCellAvailability(quad.transform.position, true);
            }
        }
        transform.position = quadsContainer.position;
        quadsContainer.localPosition = Vector3.zero;
        quadsContainer.gameObject.SetActive(false);

        mesh.material = material;

        return true;
    }

    public void onBeforeLock()
    {
        mesh.material = transparentMaterial;
        quadsContainer.gameObject.SetActive(true);
        _firstCheckDone = false;
        GameManager.Instance.buildingContainer.addChildResetPRS(transform);
        gameObject.SetActive(true);
    }

    public void onBeforeUnlock()
    {
        gameObject.SetActive(false);
    }

    private void _checkCellsAvailability()
    {
        if (quads != null)
        {
            canBuild = true;
            for (var i = 0; i < quads.Length; ++i)
            {
                var quad = quads[i];
                var avail = GameManager.Instance.checkCellAvailability(quad.transform.position);
                canBuild &= avail;
                quad.material.color = avail ? GameManager.Instance.cellRightColor : GameManager.Instance.cellWrongColor;
            }
        }
    }
}