using UnityEngine;
using UnityEngine.InputSystem;

namespace UnderwaterAdventure
{
    [RequireComponent(typeof(PlayerController), typeof(PlayerHealth))]
    public class PlayerAttack : MonoBehaviour
    {
        [SerializeField] PlayerProjectile projectilePrefab;
        [SerializeField] Transform firePoint;
        [SerializeField] ActorVisual visual;
        [SerializeField] float cooldown = 0.45f;
        [SerializeField] float muzzleDistance = 1.65f;
        PlayerController movement;
        PlayerHealth health;
        float nextAttack;
        public void Configure(PlayerProjectile projectile, Transform muzzle, ActorVisual actorVisual)
        { projectilePrefab = projectile; firePoint = muzzle; visual = actorVisual; }
        void Awake() { movement = GetComponent<PlayerController>(); health = GetComponent<PlayerHealth>(); }
        void Update()
        {
            if (Keyboard.current != null && Keyboard.current.spaceKey.isPressed) TryFire();
        }
        public bool TryFire()
        {
            if (health.IsDead || Time.time < nextAttack || projectilePrefab == null || (GameManager.Instance != null && GameManager.Instance.IsFinished)) return false;
            nextAttack = Time.time + cooldown;
            Vector2 direction = movement.AimDirection.normalized;
            firePoint.localPosition = direction * muzzleDistance;
            var shot = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            shot.Launch(direction, GetComponent<Collider2D>());
            if (visual != null) visual.Attack();
            return true;
        }
    }
}
