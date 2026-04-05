using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(LineRenderer))]
public sealed class ArcLineRenderer : MonoBehaviour
{
    [Header("Arc Settings")]
    [SerializeField] private int pointCount = 30;
    [SerializeField] private float arcLength = 5f;
    [SerializeField] private float arcHeight = 2f;
    [SerializeField] private Vector3 localDirection = Vector3.forward;

    [Header("References")]
    [SerializeField] private LineRenderer lineRenderer;

    private void Awake()
    {
        EnsureReferences();
        UpdateArc();
    }

    private void OnEnable()
    {
        EnsureReferences();
        UpdateArc();
    }

    private void OnValidate()
    {
        EnsureReferences();
        UpdateArc();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UpdateArc();
        }
#endif
    }

    private void EnsureReferences()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
    }

    private void UpdateArc()
    {
        if (lineRenderer == null)
        {
            return;
        }

        pointCount = Mathf.Max(2, pointCount);

        Vector3 direction = localDirection.sqrMagnitude > 0.0001f
            ? localDirection.normalized
            : Vector3.forward;

        Vector3 worldStart = transform.position;
        Vector3 worldDirection = transform.TransformDirection(direction);

        lineRenderer.positionCount = pointCount;

        for (int i = 0; i < pointCount; i++)
        {
            float t = i / (float)(pointCount - 1);

            Vector3 linearPoint = worldStart + worldDirection * (arcLength * t);

            float heightOffset = 4f * arcHeight * t * (1f - t);

            Vector3 arcPoint = linearPoint + transform.up * heightOffset;

            lineRenderer.SetPosition(i, arcPoint);
        }
    }
}