using UnityEngine;

namespace Root
{
    public class PackSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject packPrefab;

        private void Start()
        {
            Spawn();
        }

        public void Spawn()
        {
            Instantiate(packPrefab, transform.position, Quaternion.identity);
        }
    }
}