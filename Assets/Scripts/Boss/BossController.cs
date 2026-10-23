using UnityEngine;

namespace UnderwaterAdventure
{
    [RequireComponent(typeof(Rigidbody2D), typeof(BossHealth))]
    public class BossController : MonoBehaviour
    {
        public enum State { Chase, Telegraph, Charge, Recovery }
        [SerializeField] Transform target;
        [SerializeField] ActorVisual visual;
        [SerializeField] float chaseSpeed = 1.65f;
        [SerializeField] float phaseTwoChaseSpeed = 2.25f;
        [SerializeField] float attackRange = 6f;
        [SerializeField] float telegraphTime = 0.9f;
        [SerializeField] float phaseTwoTelegraphTime = 0.65f;
        [SerializeField] float chargeSpeed = 9f;
        [SerializeField] float chargeDuration = 0.65f;
        [SerializeField] float recoveryTime = 1.3f;
        Rigidbody2D body;
        BossHealth health;
        float stateTime;
        Vector2 chargeDirection;
        bool hitThisCharge, stopped;
        int arenaMask;
        public State CurrentState { get; private set; }
        public Vector2 ChargeDirection => chargeDirection;
        public void Configure(Transform player, ActorVisual actorVisual) { target = player; visual = actorVisual; }
        public void SetTarget(Transform player) => target = player;
        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            health = GetComponent<BossHealth>();
            arenaMask = LayerMask.GetMask("Arena");
            CurrentState = State.Chase;
        }
        void FixedUpdate()
        {
            if (stopped || health.IsDead || target == null) return;
            stateTime += Time.fixedDeltaTime;
            Vector2 delta = (Vector2)target.position - body.position;
            switch (CurrentState)
            {
                case State.Chase:
                    Vector2 direction = Steer(delta.normalized);
                    body.linearVelocity = direction * (health.PhaseTwo ? phaseTwoChaseSpeed : chaseSpeed);
                    visual.Face(delta.x);
                    visual.SetMotion("Swim");
                    if ((delta.magnitude <= attackRange && stateTime > 0.7f) || stateTime > 4f) Enter(State.Telegraph);
                    break;
                case State.Telegraph:
                    body.linearVelocity = Vector2.zero;
                    visual.Face(delta.x);
                    visual.SetMotion("Telegraph");
                    if (stateTime >= (health.PhaseTwo ? phaseTwoTelegraphTime : telegraphTime))
                    {
                        // La dirección se fija una sola vez: la embestida se puede esquivar.
                        chargeDirection = delta.sqrMagnitude > 0.01f ? delta.normalized : Vector2.left;
                        hitThisCharge = false;
                        Enter(State.Charge);
                    }
                    break;
                case State.Charge:
                    body.linearVelocity = chargeDirection * chargeSpeed;
                    visual.SetMotion("Charge");
                    if (stateTime >= chargeDuration) Enter(State.Recovery);
                    break;
                case State.Recovery:
                    body.linearVelocity = Vector2.zero;
                    visual.SetMotion("Idle");
                    if (stateTime >= recoveryTime) Enter(State.Chase);
                    break;
            }
            visual.SetWarning(CurrentState == State.Telegraph, health.PhaseTwo);
        }
        Vector2 Steer(Vector2 desired)
        {
            if (!Physics2D.CircleCast(body.position, 0.7f, desired, 1.7f, arenaMask)) return desired;
            // Rodeo local sencillo; no se crea un sistema de navegación complejo.
            Vector2 best = desired;
            float bestScore = -100f;
            for (int i = -3; i <= 3; i++)
            {
                Vector2 candidate = Quaternion.Euler(0, 0, i * 35f) * desired;
                var hit = Physics2D.CircleCast(body.position, 0.7f, candidate, 2.2f, arenaMask);
                float score = (hit ? hit.distance : 2.2f) + Vector2.Dot(candidate, desired) * 0.55f;
                if (score > bestScore) { bestScore = score; best = candidate; }
            }
            return best;
        }
        void Enter(State state) { CurrentState = state; stateTime = 0; body.linearVelocity = Vector2.zero; }
        void OnCollisionEnter2D(Collision2D collision) => HandleCollision(collision);
        void OnCollisionStay2D(Collision2D collision) => HandleCollision(collision);
        void HandleCollision(Collision2D collision)
        {
            if (stopped || health.IsDead || CurrentState != State.Charge) return;
            var player = collision.gameObject.GetComponent<PlayerHealth>();
            if (player != null && !hitThisCharge)
            {
                player.TakeDamage(1);
                hitThisCharge = true;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Arena")) Enter(State.Recovery);
        }
        public void StopAI()
        {
            stopped = true;
            body.linearVelocity = Vector2.zero;
            visual.SetWarning(false, false);
            if (!health.IsDead) visual.SetMotion("Idle");
        }
    }
}
