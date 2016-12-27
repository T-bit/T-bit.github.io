using System;
using UnityEngine;

public class SlidePanel : BasePanel
{
    #region Public Members

    public bool disableAfterHide = true;
    public float lerpSpeed = 5;
    public float outShift = 15;
    public float endDistance = 1;
    public SlideSide side = SlideSide.Left;

    public RectTransform rect { get; private set; }

    [Serializable]
    public enum SlideSide
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public override bool isShowing
    {
        get { return _lerp && _isShowing; }
    }

    public override bool isHiding
    {
        get { return _lerp && !_isShowing; }
    }

    #endregion //Public Members

    #region Private Members

    private Vector2 _pos;
    private Vector2 _outPos;

    private Vector2 _outTopPos;
    private Vector2 _outBottomPos;
    private Vector2 _outLeftPos;
    private Vector2 _outRightPos;

    private Vector2 _desiredPos;
    private bool _isShowing;
    private bool _lerp;

    private SlideSide _side;

    #endregion //Private Members

    #region Public Methods

    public void show(SlideSide s)
    {
        show(s, false);
    }

    public void show(SlideSide s, bool immediate)
    {
        show(s, immediate, null);
    }

    public void show(SlideSide s, Action oneShow)
    {
        show(s, false, oneShow);
    }

    public void show(SlideSide s, bool immediate, Action oneShow)
    {
        _side = s;
        _resetOutPos();
        base.show(immediate, oneShow);
    }

    public void hide(SlideSide s)
    {
        hide(s, false);
    }

    public void hide(SlideSide s, bool immediate)
    {
        hide(s, immediate, null);
    }

    public void hide(SlideSide s, Action oneHide)
    {
        hide(s, false, oneHide);
    }

    public void hide(SlideSide s, bool immediate, Action oneHide)
    {
        _side = s;
        _resetOutPos();
        base.hide(immediate, oneHide);
    }

    #region BasePanel Public

    public override void show()
    {
        show(false);
    }

    public override void show(bool immediate)
    {
        show(immediate, null);
    }

    public override void show(Action oneShow)
    {
        show(false, oneShow);
    }

    public override void show(bool immediate, Action oneShow)
    {
        _side = side;
        _resetOutPos();
        base.show(immediate, oneShow);
    }

    public override void hide()
    {
        hide(false);
    }

    public override void hide(bool immediate)
    {
        hide(immediate, null);
    }

    public override void hide(Action oneHide)
    {
        hide(false, oneHide);
    }

    public override void hide(bool immediate, Action oneHide)
    {
        _side = side;
        _resetOutPos();
        base.hide(immediate, oneHide);
    }

    #endregion //BasePanel Public

    #endregion //Public Methods

    #region Private Methods

    private void _resetOutPos()
    {
        switch (_side)
        {
            case SlideSide.Top:
                _outPos = _outTopPos;
                break;
            case SlideSide.Bottom:
                _outPos = _outBottomPos;
                break;
            case SlideSide.Left:
                _outPos = _outLeftPos;
                break;
            case SlideSide.Right:
                _outPos = _outRightPos;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void _disable()
    {
        if (disableAfterHide)
        {
            rect.gameObject.SetActive(false);
        }
    }

    #endregion //Private Methods

    #region BasePanel

    public override bool isShown
    {
        get { return _isShowing; }
    }

    protected override bool _onInit()
    {
        rect = GetComponent<RectTransform>();

        if (rect == null) return false;

        _outPos = _outTopPos = _outBottomPos = _outLeftPos = _outRightPos = _pos = rect.anchoredPosition;

        _outTopPos.y += rect.rect.height + outShift;
        _outBottomPos.y -= rect.rect.height + outShift;
        _outLeftPos.x -= rect.rect.width + outShift;
        _outRightPos.x += rect.rect.width + outShift;

        _side = side;
        _resetOutPos();

        return true;
    }

    protected override void _onLateUpdate(float dt)
    {
        if (_lerp)
        {
            rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, _desiredPos, lerpSpeed * dt);
            //rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, _desiredPos, 0.2f + _getLerpFPSDelta(0.2f));
            if ((rect.anchoredPosition - _desiredPos).sqrMagnitude < endDistance)
            {
                rect.anchoredPosition = _desiredPos;
                _lerp = false;
                if (_isShowing)
                {
                    _doOnAfterShow();
                }
                else
                {
                    _disable();
                    _doOnAfterHide();
                }
            }
        }
    }

    protected override void _onShow(bool immediate)
    {
        if (!gameObject.activeSelf)
        {
            rect.anchoredPosition = _outPos;
        }

        gameObject.SetActive(true);
        _isShowing = true;
        _desiredPos = _pos;

        if (immediate)
        {
            rect.anchoredPosition = _desiredPos;
            _doOnAfterShow();
        }
        else
        {
            _lerp = true;
        }
    }

    protected override void _onHide(bool immediate)
    {
        _isShowing = false;
        _desiredPos = _outPos;

        if (immediate)
        {
            rect.anchoredPosition = _desiredPos;
            _disable();
            _doOnAfterHide();
        }
        else
        {
            _lerp = true;
        }
    }

    #endregion //BasePanel
}