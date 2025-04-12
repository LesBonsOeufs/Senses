using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UIElements;

namespace Root
{
    public class SurfaceWalker : MonoBehaviour
    {
        //[InfoBox("Fill for dynamic leg anim duration & maxTipWait (each initial duration will be divided with speed)"), SerializeField]
        //private LegController legController;

        [Foldout("Movement"), SerializeField] private float speed = .5f;
        [Foldout("Movement"), SerializeField] private float pitchSpeed = 6f;
        [Foldout("Movement"), SerializeField] private float yawSpeed = 6f;
        [Foldout("Movement"), SerializeField] private float rollSpeed = 6f;

        [Foldout("Raycasting"), SerializeField] private float sphereCastRadius = 0.2f;
        [Foldout("Raycasting"), SerializeField] private float castLength = 1f;
        [Foldout("Raycasting"), SerializeField, Range(0f, 90f)] private float castAngle = 45f;
        [Foldout("Raycasting"), SerializeField, Range(0f, 90f)] private float castOpening = 90f;

        [SerializeField] private bool autoInitElevation = true;
        [InfoBox("If auto, will be used as fallback value"), SerializeField] private float initialElevation = 0.4f;

        public Vector3 direction = Vector3.zero;

        private float initControllerMaxTipWait;
        private float[] initLegAnimDurations;

        private void Start()
        {
            //initControllerMaxTipWait = legController.maxTipWait;
            //initLegAnimDurations = legController.Legs.Select(leg => leg.tipAnimationDuration).ToArray();

            if (autoInitElevation && Physics.Raycast(new Ray(transform.position, transform.up * -1), out RaycastHit lHit, 1f))
                initialElevation = lHit.distance;
        }

        private void Update()
        {
            UpdateDynamicLegAnimDurations();
            Walk(direction);
        }

        /// <param name="direction">Does not need to be normalized</param>
        private void Walk(Vector3 direction)
        {
            ///This method allows smooth point average, but can easily lose contact
            bool lFrontHasHit = Physics.SphereCast(new Ray(transform.position,
                Quaternion.AngleAxis(-castAngle - castOpening, transform.right) * transform.up * -1), sphereCastRadius, 
                out RaycastHit lFrontHit, castLength);
            bool lBackHasHit = Physics.SphereCast(new Ray(transform.position,
                Quaternion.AngleAxis(-castAngle + castOpening, transform.right) * transform.up * -1), sphereCastRadius, 
                out RaycastHit lBackHit, castLength);

            Vector3 lFrontToBack = lBackHit.point - lFrontHit.point;
            float lFrontProximityRatio = lFrontHasHit ? 1f - (lFrontHit.distance / castLength) : 0f;
            float lBackProximityRatio = lBackHasHit ? 1f - (lBackHit.distance / castLength) : 0f;

            //0 = front point, 1 = back point
            float lDistanceBasedMultiplier = lFrontProximityRatio + lBackProximityRatio;
            if (lDistanceBasedMultiplier == 0)
                lDistanceBasedMultiplier = 0.5f;
            else
                lDistanceBasedMultiplier = lBackProximityRatio / lDistanceBasedMultiplier;

            Vector3 lAveragePoint = lFrontHit.point + lDistanceBasedMultiplier * lFrontToBack.magnitude * lFrontToBack.normalized;
            Vector3 lAverageNormal = ((lFrontHit.normal * lFrontProximityRatio) + (lBackHit.normal * lBackProximityRatio)).normalized;
            Vector3 lPlanePoint = new Plane(lAverageNormal, lAveragePoint).ClosestPointOnPlane(transform.position);
            Vector3 lElevation = lAverageNormal * initialElevation;

            Vector3 lVelocity;

            if (direction == Vector3.zero)
                lVelocity = Vector3.zero;
            else
                lVelocity = speed * Time.deltaTime * Vector3.ProjectOnPlane(transform.forward, lAverageNormal).normalized;

            if (lVelocity != Vector3.zero)
            {
                transform.position = lPlanePoint + lElevation + lVelocity;

                Quaternion lTargetRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, lAverageNormal), lAverageNormal);
                Quaternion lRelativeRot = Quaternion.Inverse(transform.rotation) * lTargetRotation;
                Vector3 lRelativeEuleer = lRelativeRot.eulerAngles;

                Quaternion lPitchRot = Quaternion.Euler(lRelativeEuleer.x, 0, 0);
                Quaternion lYawRot = Quaternion.Euler(0, lRelativeEuleer.y, 0);
                Quaternion lRollRot = Quaternion.Euler(0, 0, lRelativeEuleer.z);

                transform.rotation *= Quaternion.Slerp(Quaternion.identity, lPitchRot, pitchSpeed * Time.deltaTime);
                transform.rotation *= Quaternion.Slerp(Quaternion.identity, lYawRot, yawSpeed * Time.deltaTime);
                transform.rotation *= Quaternion.Slerp(Quaternion.identity, lRollRot, rollSpeed * Time.deltaTime);
            }
        }

        private void UpdateDynamicLegAnimDurations()
        {
            //if (speed == 0f)
            //    return;

            //for (int i = legController.Legs.Length - 1; i >= 0; i--)
            //    legController.Legs[i].tipAnimationDuration = initLegAnimDurations[i] / Mathf.Abs(speed);

            //legController.maxTipWait = initControllerMaxTipWait / Mathf.Abs(speed);
        }
    }
}