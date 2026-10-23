using System;
using UnityEngine;

namespace UnderwaterAdventure
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] int maximumHealth = 5;
        [SerializeField] float invulnerabilityDuration = 0.7f;
        [SerializeField] ActorVisual visual;
        float vulnerableAt;
        public int Current { get; private set; }
        public int Maximum => maximumHealth;
        public bool IsDead => Current <= 0;
        public event Action Died;
        public void Configure(ActorVisual actorVisual) => visual = actorVisual;
        void Awake() => Current = maximumHealth;
        public bool TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead || Time.time < vulnerableAt || (GameManager.Instance != null && GameManager.Instance.IsFinished)) return false;
            Current = Mathf.Max(0, Current - amount);
            vulnerableAt = Time.time + invulnerabilityDuration;
            if (visual != null) visual.Hurt(invulnerabilityDuration);
            if (IsDead)
            {
                if (visual != null) visual.Die();
                Died?.Invoke();
            }
            return true;
        }
    }
}
