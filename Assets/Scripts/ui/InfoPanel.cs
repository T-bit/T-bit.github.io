using UnityEngine;

public class InfoPanel : MonoBehaviour, IPoolObject
{
    public RectTransform rect = null;
    public SlidePanel pSlideInfo = null;

    private Vector3? _wathPos;
    private bool _slideTop;

    public void show(Transform container, bool slideTop, Vector3 watchPos)
    {
        pSlideInfo.hide(true);
        //gameObject.SetActive(true);

        container.addChildResetPRS(transform);

        _wathPos = watchPos;
        _slideTop = slideTop;

        pSlideInfo.show(_slideTop ? SlidePanel.SlideSide.Top : SlidePanel.SlideSide.Bottom);
    }

    public void hide()
    {
        pSlideInfo.hide(_slideTop ? SlidePanel.SlideSide.Top : SlidePanel.SlideSide.Bottom, () =>
        {
            GameManager.Instance.pool.unlockObj(this);
        });
    }

    public void onInfoClick()
    {
        GameManager.Instance.mainWindow.onShowInfoClick();
    }

    public void onDeleteClick()
    {
        GameManager.Instance.mainWindow.onDeleteClick();
    }

    private void Update()
    {
        if (_wathPos.HasValue)
        {
            rect.position = GameManager.Instance.camController.cam.WorldToScreenPoint(_wathPos.Value);
        }
    }

    public void onBeforeLock()
    {
        gameObject.SetActive(true);
    }

    public void onBeforeUnlock()
    {
        pSlideInfo.hide(true);
        gameObject.SetActive(false);
    }
}