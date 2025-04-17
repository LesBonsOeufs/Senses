using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class CollisionEvents : MonoBehaviour
{
    [SerializeField] private bool destroyOtherOnEnter = false;

    [SerializeField] private UnityEvent enter;
    [SerializeField] private UnityEvent stay;
    [SerializeField] private UnityEvent exit;

    [Foldout("Advanced"), SerializeField, Tag] private string specialTag = "Untagged";
    [Foldout("Advanced"), SerializeField] private UnityEvent specialEnter;
    [Foldout("Advanced"), SerializeField] private UnityEvent otherEnter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(specialTag))
            specialEnter?.Invoke();
        else
            otherEnter?.Invoke();

        enter?.Invoke();

        if (destroyOtherOnEnter)
            Destroy(other.gameObject);
    }

    private void OnTriggerStay(Collider other) => stay?.Invoke();

    private void OnTriggerExit(Collider other) => exit?.Invoke();

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(specialTag))
            specialEnter?.Invoke();
        else
            otherEnter?.Invoke();

        enter?.Invoke();

        if (destroyOtherOnEnter)
            Destroy(collision.gameObject);
    }

    private void OnCollisionStay(Collision collision) => stay?.Invoke();

    private void OnCollisionExit(Collision collision) => exit?.Invoke();
}