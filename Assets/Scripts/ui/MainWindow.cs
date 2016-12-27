using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainWindow : MonoBehaviour, IPointerClickHandler
{
    public event Action<Vector3> onLeftMouseButtonClick;
    public event Action<Vector3> onRightMouseButtonClick;

    public event Action oneHideInfoPanel;

    public RectTransform pTurnOff = null;
    public SlidePanel pBuild = null;
    public InfoPanel infoPanelPrefab = null;
    public FadePanel pInfoWindow = null;
    public Text lbInfo = null;

    public RectTransform pInfoContainer = null;

    public void showInfo(Vector2 mousePos, Vector3 watchPos)
    {
        hideInfo();

        var pInfo = GameManager.Instance.pool.lockObj(infoPanelPrefab);
        var slideTop = mousePos.y >= Screen.height/2f;
        oneHideInfoPanel += () =>
        {
            pInfo.hide();
            GameManager.Instance.resetSelectedBuilding();
            pTurnOff.gameObject.SetActive(false);
        };

        var x = mousePos.x >= Screen.width/2f ? 1f : 0f;
        var y = mousePos.y >= Screen.height/2f ? 1f : 0f;

        pInfo.rect.anchorMin = pInfo.rect.anchorMax = pInfo.rect.pivot = new Vector2(x, y);

        pInfo.show(pInfoContainer, slideTop, watchPos);
        pTurnOff.gameObject.SetActive(true);
    }

    public void hideInfo()
    {
        if (oneHideInfoPanel != null)
        {
            oneHideInfoPanel();
        }
        oneHideInfoPanel = null;
    }

    public void onShowInfoClick()
    {
        if (GameManager.Instance.selectedBuilding == null)
        {
            Debug.LogError("MainWindow: Selected building is null.");
            lbInfo.text = "ERROR";
        }
        else
        {
            lbInfo.text = GameManager.Instance.selectedBuilding.info;
            pInfoWindow.show();
            hideInfo();
            pTurnOff.gameObject.SetActive(true);
        }
    }

    public void onDeleteClick()
    {
        if (GameManager.Instance.selectedBuilding == null)
        {
            Debug.LogError("MainWindow: Selected building is null.");
        }
        else
        {
            GameManager.Instance.selectedBuilding.destroy();
            hideInfo();
        }
    }

    public void onTurnOffPanelClick()
    {
        pTurnOff.gameObject.SetActive(false);
        pBuild.hide();
        pInfoWindow.hide();
        hideInfo();
        GameManager.Instance.resetCurrentBuilding();
    }

    public void onBuildButtonClick()
    {
        hideInfo();
        pInfoWindow.hide();
        pTurnOff.gameObject.SetActive(!pBuild.isShown);
        if (pBuild.isShown)
        {
            pBuild.hide();
        }
        else
        {
            pBuild.show();
        }

        GameManager.Instance.resetCurrentBuilding();
    }

    public void onBuildingButtonClick(Building prefab)
    {
        pTurnOff.gameObject.SetActive(false);
        pBuild.hide();

        GameManager.Instance.lockBuilding(prefab);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && onLeftMouseButtonClick != null)
        {
            onLeftMouseButtonClick(eventData.position);
        }
        else if (eventData.button == PointerEventData.InputButton.Right && onRightMouseButtonClick != null)
        {
            onRightMouseButtonClick(eventData.position);
        }
    }
}