using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadePanel : BasePanel
{
    public bool disableAfterHide = true;
    public float fadeTime = 0.3f;

    public override bool isShowing
    {
        get { return _fadeTimer > 0 && _isShowing; }
    }

    public override bool isHiding
    {
        get { return _fadeTimer > 0 && !_isShowing; }
    }

    private CanvasGroup _canvasGroup;
    private float _fadeTimer;
    private bool _isShowing;

    public override bool isShown { get { return _isShowing; } }

    protected override bool _onInit()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        return _canvasGroup != null;
    }

    protected override void _onUpdate(float dt)
    {
        if (_fadeTimer > 0)
        {
            _fadeTimer -= dt;
            var k = Mathf.Clamp01(_fadeTimer/fadeTime);

            if (_fadeTimer <= 0)
            {
                if (_isShowing)
                {
                    _canvasGroup.alpha = 1;
                    _doOnAfterShow();
                }
                else
                {
                    _canvasGroup.alpha = 0;
                    _doOnAfterHide();

                    _disable();
                }
            }
            else
            {
                    _canvasGroup.alpha = _isShowing ? 1 - k : k;
            }
        }
    }

    protected override void _onShow(bool immediate)
    {
        gameObject.SetActive(true);
        _isShowing = true;

        if (immediate)
        {
            _canvasGroup.alpha = 1;
            _doOnAfterShow();
        }
        else
        {
            _fadeTimer = fadeTime;
        }
    }

    protected override void _onHide(bool immediate)
    {
        _isShowing = false;

        if (immediate)
        {
            _canvasGroup.alpha = 0;
            _disable();
            _doOnAfterHide();
        }
        else
        {
            _fadeTimer = fadeTime;
        }
    }

    private void _disable()
    {
        if (disableAfterHide)
        {
            gameObject.SetActive(false);
        }
    }
}