using UnityEngine;
namespace Amazonia
{
    public class AdventureCamera : MonoBehaviour
    {
        public Transform target; public Transform focus;
        public AdventureBoss boss;
        public Rect limits = new Rect(0, -3, 66, 24);
        public bool aquatic;
        [Range(0, .3f)] public float impactShake = .07f; float shakeUntil; Vector3 speed, basePosition;
        public void Shake() { shakeUntil = Time.unscaledTime + .16f; }
        Camera view;
        void Awake() { view = GetComponent<Camera>(); basePosition = transform.position; }
        void LateUpdate()
        {
            if (!target) return; float aspect = (float)Screen.width / Mathf.Max(1, Screen.height); view.rect = new Rect(0, 0, 1, 1); view.orthographicSize = 5.3f; float half = view.orthographicSize * aspect; Vector3 wanted = target.position + new Vector3(2, aquatic ? 0 : 2.7f, -10);
            if (boss && boss.isActiveAndEnabled && !boss.Health.Dead && Mathf.Abs(boss.transform.position.x - target.position.x) < 13) { wanted.x = Mathf.Lerp(target.position.x, boss.transform.position.x, .28f); }
            if (focus) wanted = focus.position + new Vector3(0, aquatic ? 0 : 2, -10);
            wanted.x = Mathf.Clamp(wanted.x, limits.xMin + half, limits.xMax - half); wanted.y = Mathf.Clamp(wanted.y, limits.yMin + view.orthographicSize, limits.yMax - view.orthographicSize); wanted.z = -10;
            basePosition = Vector3.SmoothDamp(basePosition, wanted, ref speed, .18f, 100, Time.unscaledDeltaTime); float strength = Mathf.Clamp01((shakeUntil - Time.unscaledTime) / .16f) * impactShake; transform.position = basePosition + new Vector3(Mathf.Sin(Time.unscaledTime * 97), Mathf.Cos(Time.unscaledTime * 83), 0) * strength;
        }
    }
}




