using NaughtyAttributes;
using System.Collections;
using System.Linq;
using UnityEngine;

public class Leg : MonoBehaviour
{
    //Additional
    [SerializeField] private AudioSource stepSFX;

    [SerializeField] private Transform bodyTransform;
    [SerializeField] private Transform rayForwardOrigin;
    [SerializeField] private Transform rayDownOrigin;
    public GameObject ikTarget;

    [SerializeField] private AnimationCurve speedCurve;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private bool useForwardRay = true;

    [Foldout("Advanced")] public float tipAnimationDuration = 0.15f;
    [Foldout("Advanced"), SerializeField] private float tipAnimationFrameTime = 1 / 60.0f;

    [Foldout("Advanced"), SerializeField] private float tipMaxHeight = 0.2f;
    [Foldout("Advanced"), SerializeField] private float initTipPosZOffset = 0f;
    [Foldout("Advanced"), SerializeField] private float ikYOffset = 1.0f;
    [Foldout("Advanced"), SerializeField] private float tipMoveDist = 0.55f;
    [Foldout("Advanced"), SerializeField] private float maxRayDownDist = 2.0f;
    [Foldout("Advanced"), SerializeField] private float maxRayForwardDist = 2.0f;
    [Foldout("Advanced"), SerializeField] private float perchedRayOriginDistFromDown = 0.5f;
    [Foldout("Advanced"), SerializeField] private float maxPerchedRayDist = 0.7f;
    //Currently should stay at 0, for preventing passing tip through wall
    [Foldout("Advanced"), SerializeField] private float tipPassOver = 0.55f / 2.0f;

    //Used for forwardRay's direction
    private Vector3 lastPos;
    private Vector3 lastLastPos;

    public Vector3 TipPos { get; private set; }
    public Vector3 RaycastTipPos { get; private set; }

    public bool Animating { get; private set; } = false;
    public bool Movable { get; set; } = false;
    public float TipDistance { get; private set; }

    //Does not reset if no movement
    public Vector3 SafePositionDelta => transform.position - lastLastPos;

    public Ray ForwardRay => new(rayForwardOrigin.position,
        Vector3.ProjectOnPlane(SafePositionDelta, bodyTransform.up).normalized);
    public Ray DownRay => new(rayDownOrigin.position, bodyTransform.up * -1);
    public Ray PerchedRay => new(DownRay.origin + DownRay.direction * perchedRayOriginDistFromDown,
        Vector3.Project(bodyTransform.position - transform.position, transform.forward));
    public Ray InversedPerchedRay
    {
        get
        {
            Ray lPerchedRay = PerchedRay;
            return new(lPerchedRay.origin + lPerchedRay.direction * maxPerchedRayDist, -lPerchedRay.direction);
        }
    }

    private void Awake()
    {
        TipPos = transform.TransformPoint(ikTarget.transform.localPosition);
    }

    private void Start()
    {
        lastLastPos = transform.position;
        lastPos = transform.position;
        UpdateIKTargetTransform();
        RefreshRaycastTipPos();
        TipPos = RaycastTipPos + bodyTransform.forward * initTipPosZOffset;
    }

    private void Update()
    {
        UpdateIKTargetTransform();
        RefreshRaycastTipPos();

        //TipDistance = Vector3.ProjectOnPlane(RaycastTipPos - TipPos, bodyTransform.up).magnitude;
        TipDistance = (RaycastTipPos - TipPos).magnitude;

        // If the distance gets too far, animate and move the tip to new position
        if (!Animating && TipDistance > tipMoveDist && Movable)
            StartCoroutine(AnimateLeg());

        if (lastPos != transform.position)
        {
            lastLastPos = lastPos;
            lastPos = transform.position;
        }
    }

    private void RefreshRaycastTipPos()
    {
        RaycastHit?[] lHits = new RaycastHit?[] { Raycast(DownRay, maxRayDownDist) };

        if (useForwardRay)
        {
            Ray lForwardRay = ForwardRay;
            RaycastHit? lForwardHit = Raycast(lForwardRay, maxRayForwardDist);

            if (lForwardHit != null)
            {
                //float lAddedForwardRayDistance = Vector3.Project(lForwardRay.origin - rayDownOrigin.position, lForwardRay.direction).magnitude;
                RaycastHit lForwardHitCopy = lForwardHit.Value;
                //lForwardHitCopy.distance += lAddedForwardRayDistance;
                lHits = lHits.Append(lForwardHitCopy).ToArray();
            }
        }

        //Choose the closest valid hit
        RaycastHit? lHit = lHits.OrderBy(hit => hit == null ? Mathf.Infinity : hit.Value.distance).First();

        if (lHit == null)
        {
            //"Perched" raycasts (as a perched bird on a tree branch) for checking if surface was between previous raycasts
            lHit = Raycast(PerchedRay, maxPerchedRayDist);

            //If perched raycast does not hit, do another one, coming from the raycast's end as its origin
            //if (lHit == null)
            //    lHit = Raycast(InversedPerchedRay, maxPerchedRayDist);
        }

        if (lHit != null)
            RaycastTipPos = lHit.Value.point;
    }

    private RaycastHit? Raycast(Ray ray, float maxDistance)
    {
        if (Physics.Raycast(ray, out RaycastHit lHit, maxDistance))
            return lHit;

        return null;
    }

    private IEnumerator AnimateLeg()
    {
        if (stepSFX != null)
            stepSFX.PlayDelayed(tipAnimationDuration + tipAnimationFrameTime);

        Animating = true;

        float lTimer = 0.0f;
        float lAnimTime;
        Vector3 lInitTipPos = TipPos;
        Vector3 lTipUpDir;

        while (lTimer < tipAnimationDuration + tipAnimationFrameTime)
        {
            lAnimTime = speedCurve.Evaluate(lTimer / tipAnimationDuration);

            Vector3 lTipToTarget = RaycastTipPos - lInitTipPos;
            Vector3 lTipDirection = lTipToTarget.normalized;
            lTipToTarget += lTipDirection * tipPassOver;
            lTipUpDir = Vector3.Cross(lTipDirection, Vector3.Cross(bodyTransform.up, lTipDirection).normalized);

            TipPos = lInitTipPos + lTipToTarget * lAnimTime;
            TipPos += heightCurve.Evaluate(lAnimTime) * tipMaxHeight * lTipUpDir;

            lTimer += tipAnimationFrameTime;
            yield return new WaitForSeconds(tipAnimationFrameTime);
        }

        Animating = false;
    }

    private void UpdateIKTargetTransform()
    {
        // Update leg ik target transform depending on tip information
        Vector3 lWorldIKTargetPosition = TipPos + bodyTransform.up.normalized * ikYOffset;
        ikTarget.transform.localPosition = transform.InverseTransformPoint(lWorldIKTargetPosition);
        Quaternion lWorldIKTargetRotation = Quaternion.LookRotation(bodyTransform.forward, bodyTransform.up);
        ikTarget.transform.localRotation = Quaternion.Inverse(transform.rotation) * lWorldIKTargetRotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(RaycastTipPos, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(TipPos, RaycastTipPos);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(ikTarget.transform.position, 0.1f);

        if (useForwardRay)
        {
            //Forward ray
            ColorUtility.TryParseHtmlString("#FF4545", out Color lForwardColor);
            Gizmos.color = lForwardColor;
            Gizmos.DrawSphere(rayForwardOrigin.position, 0.05f);
            Gizmos.DrawRay(ForwardRay);
        }

        //Down ray
        ColorUtility.TryParseHtmlString("#2D0000", out Color lDownColor);
        Gizmos.color = lDownColor;
        Gizmos.DrawSphere(rayDownOrigin.position, 0.05f);
        Gizmos.DrawRay(DownRay);

        //Perched ray
        Ray lPerchedRay = PerchedRay;
        ColorUtility.TryParseHtmlString("#FF93D226", out Color lPerchedColor);
        Gizmos.color = lPerchedColor;
        Gizmos.DrawSphere(lPerchedRay.origin, 0.05f);
        Gizmos.DrawRay(lPerchedRay);
    }
}