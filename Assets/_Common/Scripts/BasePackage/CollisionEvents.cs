using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    [SerializeField] private bool destroyOtherOnEnter = false;

    [SerializeField] private UnityEvent triggerEnter;
    [SerializeField] private UnityEvent triggerStay;
    [SerializeField] private UnityEvent triggerExit;
    [SerializeField] private UnityEvent collisionEnter;
    [SerializeField] private UnityEvent collisionStay;
    [SerializeField] private UnityEvent collisionExit;

    private void OnTriggerEnter(Collider other)
    {
        triggerEnter?.Invoke();

        if (destroyOtherOnEnter)
            Destroy(other.gameObject);
    }

    private void OnTriggerStay(Collider other) => triggerStay?.Invoke();

    private void OnTriggerExit(Collider other) => triggerExit?.Invoke();

    private void OnCollisionEnter(Collision collision)
    {
        collisionEnter?.Invoke();

        if (destroyOtherOnEnter)
            Destroy(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision) => collisionStay?.Invoke();

    private void OnCollisionExit(Collision collision) => collisionExit?.Invoke();
}