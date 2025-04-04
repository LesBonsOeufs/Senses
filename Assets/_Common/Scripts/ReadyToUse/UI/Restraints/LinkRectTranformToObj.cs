using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class LinkRectTranformToObj : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 offset;
    
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        rectTransform.position = (Vector2)target.position + offset;
    }

    private void OnEnable()
    {
        Update();
    }
}
