using UnityEngine;

namespace Root
{
    /// <summary>
    /// Conveyer belt that has a uniform direction and speed and will push objects that collide with it.
    /// </summary>
    [RequireComponent(typeof(MeshRenderer))]
    public class ConveyorBelt : MonoBehaviour
    {
        /// <summary>
        /// How does this belt move objects, by pushing objects sitting
        /// on top of the belt or by "moving" the belt forward and pulling
        /// them forward with the object.
        /// </summary>
        public enum BeltForceMode
        {
            Push,
            Pull
        }

        /// <summary>
        /// Relative direction to face from a local transform.
        /// </summary>
        public enum RelativeDirection
        {
            Up,
            Down,
            Left,
            Right,
            Forward,
            Backward
        }

        [SerializeField] private float velocity;

        [SerializeField, Tooltip("Local direction does this push objects.")]
        private RelativeDirection direction = RelativeDirection.Down;

        [SerializeField] public int materialIndex = 1;
        private Material materialInstance;
        private int materialScrollPropertyId;
        private float currentScrollValue = 0f;

        private BeltForceMode beltMode = BeltForceMode.Push;

        private Rigidbody body;
        private Vector3 pos;

        public void Awake()
        {
            body = GetComponent<Rigidbody>();
            pos = transform.position;
            materialInstance = GetComponent<MeshRenderer>().materials[materialIndex];
            materialScrollPropertyId = Shader.PropertyToID("_ScrollY");
        }

        private void Update()
        {
            currentScrollValue += velocity * Time.deltaTime;
            currentScrollValue %= 1f;
            materialInstance.SetFloat(materialScrollPropertyId, -currentScrollValue);
        }

        /// <summary>
        /// Fixed update to move the belt and pull objects along with it.
        /// </summary>
        public void FixedUpdate()
        {
            if (enabled && body != null && beltMode == BeltForceMode.Pull)
            {
                Vector3 movement = velocity * GetDirection() * Time.fixedDeltaTime;
                transform.position = pos - movement;
                body.MovePosition(pos);
            }
        }

        /// <summary>
        /// When colliding with an object, if it has a rigidbody and is not kinematic, push it forward by the speed of
        /// the conveyer belt.
        /// </summary>
        /// <param name="other">Collision event between the objects.</param>
        public void OnCollisionStay(Collision other)
        {
            if (enabled && other.rigidbody != null && !other.rigidbody.isKinematic && beltMode == BeltForceMode.Push)
            {
                Vector3 movement = velocity * GetDirection() * Time.deltaTime;
                other.rigidbody.MovePosition(other.transform.position + movement);
            }
        }

        /// <summary>
        /// Get the world space direction that the conveyer belt will push objects.
        /// </summary>
        /// <returns>The direction in world space of teh conveyer belt, will be 1 unit long.</returns>
        public Vector3 GetDirection()
        {
            switch (direction)
            {
                case RelativeDirection.Up:
                    return transform.up;
                case RelativeDirection.Down:
                    return -transform.up;
                case RelativeDirection.Left:
                    return -transform.right;
                case RelativeDirection.Right:
                    return transform.right;
                case RelativeDirection.Forward:
                    return transform.forward;
                case RelativeDirection.Backward:
                    return -transform.forward;
            }
            return transform.forward;
        }
    }
}