using UnityEngine;
namespace Amazonia {
[CreateAssetMenu(menuName="Amazonia/Balance")]
public class AdventureBalance : ScriptableObject {
 public int playerHP=8, landBossHP=18, waterBossHP=22;
 public float runSpeed=6, acceleration=40, jumpSpeed=12.5f, landGravity=2.8f;
 public float dashSpeed=15, dashDuration=.18f, dashCooldown=1.1f;
 public float swimSpeed=5.3f, swimAcceleration=24, waterDrag=3, waterGravity=.12f;
 public float plasmaCooldown=.42f, plasmaSpeed=16, meleeCooldown=.48f;
 public int plasmaDamage=1, swordDamage=3;
 public float chaseLand=2.8f, chaseWater=3.1f, chargeSpeed=12, preparation=.85f, recovery=1.15f;
}
}

