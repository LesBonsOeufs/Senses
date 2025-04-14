using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(Rigidbody))]
    public class Rigidbody_NoSleep : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Rigidbody>().sleepThreshold = 0.0f;
        }
    }
}