using System;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollisionListener : MonoBehaviour
{
    public event Action<Collision> onCollisionEnter;
    public event Action<Collision> onCollisionStay;
    public event Action<Collision> onCollisionExit;

    public event Action<Collider> onTriggerEnter;
    public event Action<Collider> onTriggerStay;
    public event Action<Collider> onTriggerExit;

    public Collider col { get; private set; }

    private bool _isInit;

    private void _init()
    {
        if (_isInit) return;

        col = GetComponent<Collider>();

        _isInit = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        _init();

        if (onCollisionEnter != null)
        {
            onCollisionEnter.Invoke(collision);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        _init();

        if (onCollisionStay != null)
        {
            onCollisionStay.Invoke(collision);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        _init();

        if (onCollisionExit != null)
        {
            onCollisionExit.Invoke(collision);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _init();

        if (onTriggerEnter != null)
        {
            onTriggerEnter.Invoke(other);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        _init();

        if (onTriggerStay != null)
        {
            onTriggerStay.Invoke(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _init();

        if (onTriggerExit != null)
        {
            onTriggerExit.Invoke(other);
        }
    }

    private void Awake()
    {
        _init();
    }
}