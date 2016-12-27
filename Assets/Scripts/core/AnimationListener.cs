using UnityEngine;
using UnityEngine.Events;

public class AnimationListener : MonoBehaviour
{
    public UnityEvent onAnimationStart;
    public UnityEvent onAnimationEnd;
    public UnityEvent onAnimationEvent1;

    public void OnAnimationStart()
    {
        if (onAnimationStart != null)
        {
            onAnimationStart.Invoke();
        }
    }

    public void OnAnimationEnd()
    {
        if (onAnimationEnd != null)
        {
            onAnimationEnd.Invoke();
        }
    }

    public void OnAnimationEvent1()
    {
        if (onAnimationEvent1 != null)
        {
            onAnimationEvent1.Invoke();
        }
    }
}