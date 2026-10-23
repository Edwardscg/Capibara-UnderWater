using System;
using UnityEngine;

namespace UnderwaterAdventure
{
    public class BossHealth : MonoBehaviour
    {
        [SerializeField] int maximumHealth = 12;
        [SerializeField] ActorVisual visual;
        public int Current { get; private set; }
        public int Maximum => maximumHealth;
        public bool IsDead => Current <= 0;
        public bool PhaseTwo => Current <= maximumHealth / 2;
        public event Action Died;
        public void Configure(ActorVisual actorVisual) => visual = actorVisual;
        void Awake() => Current = maximumHealth;
        public void TakeDamage(int amount)
        {
            if (amount <= 0 || IsDead || (GameManager.Instance != null && GameManager.Instance.IsFinished)) return;
            Current = Mathf.Max(0, Current - amount);
            if (visual != null) visual.Hurt(0.16f);
            if (IsDead)
            {
                if (visual != null) visual.Die();
                Died?.Invoke();
            }
        }
    }
}
