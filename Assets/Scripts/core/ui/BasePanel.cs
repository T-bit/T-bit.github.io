using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public abstract class BasePanel : MonoBehaviour, IShowable
{
    public event Action<BasePanel> onBeforeShow;
    public event Action<BasePanel> onAfterShow;
    public event Action<BasePanel> onBeforeHide;
    public event Action<BasePanel> onAfterHide;

    private Action _oneHide;
    private Action _oneShow;

    public bool hideAfterInit = true;
    public bool isInit { get; private set; }

    public virtual bool isShown { get {return gameObject.activeSelf;} }

    public abstract bool isShowing { get; }
    public abstract bool isHiding { get; }

    protected virtual void _onAwake() { }
    protected virtual void _onStart() { }
    protected virtual void _onUpdate(float dt) { }
    protected virtual void _onFixedUpdate(float dt) { }
    protected virtual void _onLateUpdate(float dt) { }

    protected virtual bool _onInit()
    {
        return true;
    }

    protected abstract void _onShow(bool immediate);
    protected abstract void _onHide(bool immediate);

    private void Awake()
    {
        //_init();
        _onAwake();
    }

    private void Start()
    {
        _init();
        _onStart();
    }

    private void Update()
    {
        var dt = Time.deltaTime;

        _onUpdate(dt);
    }

    private void FixedUpdate()
    {
        var dt = Time.fixedDeltaTime;

        _onFixedUpdate(dt);
    }

    private void LateUpdate()
    {
        var dt = Time.deltaTime;

        _onLateUpdate(dt);
    }

    private void _init()
    {
        if (isInit) return;
        isInit = _onInit();
        if (isInit)
        {
            if (hideAfterInit)
            {
                _onHide(true);
            }
            else
            {
                _onShow(true);
            }
        }
        else
        {
            Debug.LogWarning("Failed to initialize " + GetType() + " name = " + name);
        }
    }

    [ContextMenu("show")]
    public virtual void show()
    {
        show(false);
    }

    public virtual void show(bool immediate)
    {
        show(immediate, null);
    }

    public virtual void show(Action oneShow)
    {
        show(false, oneShow);
    }

    public virtual void show(bool immediate, Action oneShow)
    {
        _init();

        if (isShown) return;

        if (isHiding)
        {
            _doOnAfterHide();
        }

        if (oneShow != null)
        {
            _oneShow = oneShow;
            onAfterShow += _oneAfterShow;
        }

        _doOnBeforeShow();
        _onShow(immediate);
    }

    [ContextMenu("hide")]
    public virtual void hide()
    {
        hide(false);
    }

    public virtual void hide(bool immediate)
    {
        hide(immediate, null);
    }

    public virtual void hide(Action oneHide)
    {
        hide(false, oneHide);
    }

    public virtual void hide(bool immediate, Action oneHide)
    {
        _init();

        if (!isShown) return;

        if (isShowing)
        {
            _doOnAfterShow();
        }

        if (oneHide != null)
        {
            _oneHide = oneHide;
            onAfterHide += _oneAfterHide;
        }

        _doOnBeforeHide();
        _onHide(immediate);
    }

    private void _oneAfterShow(BasePanel panel)
    {
        onAfterShow -= _oneAfterShow;

        if (_oneShow != null)
        {
            _oneShow();
            _oneShow = null;
        }
    }

    private void _oneAfterHide(BasePanel panel)
    {
        onAfterHide -= _oneAfterHide;

        if (_oneHide != null)
        {
            _oneHide();
            _oneHide = null;
        }
    }

    protected virtual void _doOnBeforeShow()
    {
        if (onBeforeShow != null)
        {
            onBeforeShow(this);
        }
    }

    protected virtual void _doOnAfterShow()
    {
        if (onAfterShow != null)
        {
            onAfterShow(this);
        }
    }

    protected virtual void _doOnBeforeHide()
    {
        if (onBeforeHide != null)
        {
            onBeforeHide(this);
        }
    }

    protected virtual void _doOnAfterHide()
    {
        if (onAfterHide != null)
        {
            onAfterHide(this);
        }
    }

    protected static float _getLerpFPSDelta(float power)
    {

        float s = 0;
        float current = 0;
        float cicles = Time.deltaTime / Time.fixedDeltaTime;
        while (cicles > 0)
        {
            current = (1 - current) * power;
            if (cicles < 1)
            {
                current *= cicles;
            }
            s += current;
            cicles--;
        }
        return s - power;
    }

    public void enableLayouts()
    {
        _enableLayouts(true);
    }

    private void _enableLayouts(bool enable)
    {
        var layoutElements = GetComponentsInChildren<LayoutElement>(true);
        var layoutGroups = GetComponentsInChildren<LayoutGroup>(true);
        var fitters = GetComponentsInChildren<ContentSizeFitter>(true);

        for (var i = 0; i < layoutGroups.Length; ++i)
        {
            var layoutGroup = layoutGroups[i];
            if (layoutGroup.GetComponent<DontDisableLayoutMark>() == null)
            {
                layoutGroup.enabled = enable;
            }
        }
        for (var i = 0; i < fitters.Length; ++i)
        {
            var fitter = fitters[i];
            if (fitter.GetComponent<DontDisableLayoutMark>() == null)
            {
                fitter.enabled = enable;
            }
        }
        for (var i = 0; i < layoutElements.Length; ++i)
        {
            var layoutElement = layoutElements[i];
            if (layoutElement.GetComponent<DontDisableLayoutMark>() == null)
            {
                layoutElement.enabled = enable;
            }
        }
    }

    public void disableLayouts()
    {
        if (!gameObject.activeInHierarchy)
        {
            _enableLayouts(false);
            return;
        }

        StartCoroutine(_disableLayoutsCoroutine());
    }

    private IEnumerator _disableLayoutsCoroutine()
    {
        //var layoutElements = GetComponentsInChildren<LayoutElement>(true);
        //var layoutGroups = GetComponentsInChildren<LayoutGroup>(true);
        //var fitters = GetComponentsInChildren<ContentSizeFitter>(true);

        yield return new WaitForEndOfFrame();
        _enableLayouts(false);

        //for (var i = 0; i < layoutGroups.Length; ++i)
        //{
        //    layoutGroups[i].enabled = false;
        //}
        //for (var i = 0; i < fitters.Length; ++i)
        //{
        //    fitters[i].enabled = false;
        //}
        //for (var i = 0; i < layoutElements.Length; ++i)
        //{
        //    layoutElements[i].enabled = false;
        //}
    }
}