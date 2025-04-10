using UnityEngine;
using UnityEngine.Events;

public class MonoBehaviourEvents : MonoBehaviour
{
    [SerializeField] private UnityEvent awake;
    [SerializeField] private UnityEvent start;
    [SerializeField] private UnityEvent onEnable;
    [SerializeField] private UnityEvent onDisable;
    [SerializeField] private UnityEvent onDestroy;

    private void Awake() => awake?.Invoke();

    private void Start() => start?.Invoke();

    private void OnEnable() => onEnable?.Invoke();

    private void OnDisable() => onDisable?.Invoke();

    private void OnDestroy() => onDestroy?.Invoke();
}