using UnityEngine;

namespace UnderwaterAdventure
{
    public class ActorVisual : MonoBehaviour
    {
        [SerializeField] Transform facingRoot;
        [SerializeField] Animator animator;
        SpriteRenderer[] renderers;
        Color[] baseColors;
        string currentMotion;
        float hurtUntil, attackUntil;
        bool dead, warning, phaseTwo;
        public void Configure(Transform facing, Animator animationController) { facingRoot = facing; animator = animationController; }
        void Awake()
        {
            renderers = GetComponentsInChildren<SpriteRenderer>();
            baseColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++) baseColors[i] = renderers[i].color;
        }
        public void Face(float x)
        {
            if (dead || Mathf.Abs(x) < 0.05f) return;
            facingRoot.localScale = new Vector3(x < 0 ? -1 : 1, 1, 1);
        }
        public void SetMotion(string state)
        {
            if (dead || Time.time < hurtUntil || Time.time < attackUntil) return;
            Play(state);
        }
        void Play(string state)
        {
            if (currentMotion == state || animator == null) return;
            currentMotion = state;
            animator.Play(state, 0, 0);
        }
        public void Hurt(float duration) { if (dead) return; hurtUntil = Time.time + duration; Play("Hurt"); }
        public void Attack() { if (dead || Time.time < hurtUntil) return; attackUntil = Time.time + 0.22f; Play("Attack"); }
        public void Die() { dead = true; Play("Death"); }
        public void SetWarning(bool value, bool secondPhase) { warning = value; phaseTwo = secondPhase; }
        void LateUpdate()
        {
            Color tint = dead ? new Color(0.65f, 0.75f, 0.8f) : Color.white;
            if (!dead && Time.time < hurtUntil) tint = Mathf.Sin(Time.time * 55f) > 0 ? new Color(1, 0.35f, 0.35f) : Color.white;
            else if (!dead && warning) tint = Color.Lerp(Color.white, new Color(1f, 0.35f, 0.15f), (Mathf.Sin(Time.time * 20) + 1) * 0.5f);
            else if (!dead && phaseTwo) tint = new Color(1f, 0.85f, 0.72f);
            for (int i = 0; i < renderers.Length; i++) renderers[i].color = baseColors[i] * tint;
        }
    }
}
