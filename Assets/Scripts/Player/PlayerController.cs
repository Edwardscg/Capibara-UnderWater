using UnityEngine;
using UnityEngine.InputSystem;

namespace UnderwaterAdventure
{
    [RequireComponent(typeof(Rigidbody2D), typeof(PlayerHealth))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float acceleration = 18f;
        [SerializeField] float maximumSpeed = 4.2f;
        [SerializeField] float waterResistance = 3f;
        [SerializeField] float gravityScale = 0.16f;
        [SerializeField] ActorVisual visual;
        Rigidbody2D body;
        PlayerHealth health;
        Vector2 input;
        bool stopped;
        public Vector2 AimDirection { get; private set; } = Vector2.right;
        public Vector2 MovementInput => input;
        public void Configure(ActorVisual actorVisual) => visual = actorVisual;
        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            health = GetComponent<PlayerHealth>();
            body.gravityScale = gravityScale;
            body.linearDamping = waterResistance;
        }
        void Update()
        {
            input = Vector2.zero;
            if (stopped || health.IsDead) return;
            var keyboard = Keyboard.current;
            if (keyboard == null) return;
            input.x = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1 : 0)
                    - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed ? 1 : 0);
            input.y = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed ? 1 : 0)
                    - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed ? 1 : 0);
            input = Vector2.ClampMagnitude(input, 1f);
            if (input.sqrMagnitude > 0.01f) AimDirection = input;
            if (visual != null)
            {
                visual.Face(AimDirection.x);
                visual.SetMotion(input.sqrMagnitude > 0.01f ? "Swim" : "Idle");
            }
        }
        void FixedUpdate()
        {
            if (stopped || health.IsDead) return;
            body.AddForce(input * acceleration, ForceMode2D.Force);
            body.linearVelocity = Vector2.ClampMagnitude(body.linearVelocity, maximumSpeed);
        }
        public void StopControl()
        {
            stopped = true;
            input = Vector2.zero;
            body.linearVelocity = Vector2.zero;
            body.gravityScale = 0f;
        }
    }
}
