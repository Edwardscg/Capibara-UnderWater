using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UnderwaterAdventure
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        [SerializeField] PlayerHealth player;
        [SerializeField] BossHealth boss;
        [SerializeField] BossController bossController;
        [SerializeField] GameObject resultPanel;
        [SerializeField] Text resultTitle;
        [SerializeField] Text resultDetail;
        [SerializeField] Text bossStatus;
        [SerializeField] ParticleSystem impactPrefab;
        public bool IsFinished { get; private set; }
        public bool Won { get; private set; }
        public void Configure(PlayerHealth p, BossHealth b, BossController ai, GameObject panel, Text title, Text detail, Text status, ParticleSystem impact)
        { player = p; boss = b; bossController = ai; resultPanel = panel; resultTitle = title; resultDetail = detail; bossStatus = status; impactPrefab = impact; }
        void Awake() { Instance = this; Time.timeScale = 1f; }
        void Start()
        {
            resultPanel.SetActive(false);
            player.Died += Lose;
            boss.Died += Win;
        }
        void Update()
        {
            if (IsFinished)
            {
                if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame) Restart();
                return;
            }
            string action = bossController.CurrentState switch
            {
                BossController.State.Telegraph => "¡PREPÁRATE PARA ESQUIVAR!",
                BossController.State.Charge => "¡EMBESTIDA!",
                BossController.State.Recovery => "AGOTADO · ¡ATACA!",
                _ => "MANTÉN LA DISTANCIA"
            };
            bossStatus.text = (boss.PhaseTwo ? "FASE 2  /  " : "FASE 1  /  ") + action;
        }
        void Win() => Finish(true);
        void Lose() => Finish(false);
        void Finish(bool victory)
        {
            if (IsFinished) return;
            IsFinished = true;
            Won = victory;
            player.GetComponent<PlayerController>().StopControl();
            bossController.StopAI();
            foreach (var shot in FindObjectsByType<PlayerProjectile>()) Destroy(shot.gameObject);
            resultPanel.SetActive(true);
            resultTitle.text = victory ? "¡VICTORIA!" : "DERROTA";
            resultTitle.color = victory ? new Color(0.45f, 1, 0.85f) : new Color(1, 0.55f, 0.4f);
            resultDetail.text = victory ? "El guardián del arrecife ha sido derrotado.\nTu aventura continúa." : "El caimán ganó esta vez.\nEsquiva el aviso rojo y ataca durante su descanso.";
        }
        public void Restart() { Time.timeScale = 1f; SceneManager.LoadScene("UnderwaterBoss"); }
        public void SpawnImpact(Vector3 position)
        {
            if (impactPrefab == null) return;
            var effect = Instantiate(impactPrefab, position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 1.5f);
        }
        void OnDestroy()
        {
            if (player != null) player.Died -= Lose;
            if (boss != null) boss.Died -= Win;
            if (Instance == this) Instance = null;
        }
    }
}
