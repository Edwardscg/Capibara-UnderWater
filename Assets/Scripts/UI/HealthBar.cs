using UnityEngine;
using UnityEngine.UI;

namespace UnderwaterAdventure
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] PlayerHealth player;
        [SerializeField] BossHealth boss;
        [SerializeField] Image fill;
        [SerializeField] Text value;
        public void Configure(PlayerHealth p, BossHealth b, Image bar, Text label) { player = p; boss = b; fill = bar; value = label; }
        void Update()
        {
            int current = player != null ? player.Current : boss.Current;
            int maximum = player != null ? player.Maximum : boss.Maximum;
            fill.fillAmount = (float)current / maximum;
            value.text = current + " / " + maximum;
        }
    }
}
