using UnityEngine;

namespace Root
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private GameObject[] prefabs;

        private void Start()
        {
            SpawnOneRandom();
        }

        public void SpawnOneRandom()
        {
            Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform.position, Quaternion.identity);
        }
    }
}