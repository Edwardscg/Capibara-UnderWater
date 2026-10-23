using UnityEngine;

namespace UnderwaterAdventure
{
    [RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
    public class PlayerProjectile : MonoBehaviour
    {
        [SerializeField] float speed = 11f;
        [SerializeField] int damage = 1;
        [SerializeField] float lifetime = 2.5f;
        bool spent;
        void Start() => Destroy(gameObject, lifetime);
        public void Launch(Vector2 direction, Collider2D owner)
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * speed;
            if (owner != null) Physics2D.IgnoreCollision(GetComponent<Collider2D>(), owner);
        }
        void OnTriggerEnter2D(Collider2D other)
        {
            if (spent || other.isTrigger || other.GetComponentInParent<PlayerHealth>() != null) return;
            var boss = other.GetComponentInParent<BossHealth>();
            if (boss == null && other.gameObject.layer != LayerMask.NameToLayer("Arena")) return;
            spent = true;
            if (boss != null) boss.TakeDamage(damage);
            if (GameManager.Instance != null) GameManager.Instance.SpawnImpact(transform.position);
            Destroy(gameObject);
        }
    }
}
