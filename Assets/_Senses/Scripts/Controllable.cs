using System;
using UnityEngine;

namespace Root
{
    [RequireComponent(typeof(CharacterController))]
    public class Controllable : MonoBehaviour
    {
        [SerializeField] private float speed = 3f;
        [SerializeField] private float rotationSpeed = 3f;

        [NonSerialized] public Vector2 input;
        private CharacterController controller;

        void Start()
        {
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            controller.SimpleMove(speed * input.y * transform.forward);
            transform.Rotate(0f, rotationSpeed * input.x * Time.deltaTime, 0f);
        }
    }
}