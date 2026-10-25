using UnityEngine;
using UnityEngine.InputSystem;
namespace Amazonia {
[RequireComponent(typeof(Rigidbody2D),typeof(CombatHealth))]
public class AdventurePlayer : MonoBehaviour {
 public bool aquatic;
 public AdventureBalance balance;
 public AdventureVisual visual;
 public Transform muzzle;
 public GameObject swordGlow,gunGlow;
 public AdventureProjectile plasmaPrefab;
 public enum ActionState { Idle,Run,Jump,Fall,Land,Dash,Melee,Ranged,Switch,Hurt,Defeated }
 public ActionState State {get; private set;}
 public bool Sword {get; private set;}
 public bool Locked;
 public Vector2 Aim {get; private set;}=Vector2.right;
 public float Facing {get; private set;}=1;
 public bool Grounded {get; private set;}
 public float DashReady=>Mathf.Clamp01(1-(dashNext-Time.time)/balance.dashCooldown);
 Rigidbody2D body;CombatHealth health;Collider2D hitbox;
 Vector2 input;float jumpBuffered=-10,lastGround=-10,dashUntil,dashNext,hurtUntil,actionUntil,nextAttack,meleeAt,landUntil;
 bool meleePending,wasGrounded;float dashDirection;
 public Rigidbody2D Body=>body;
 public CombatHealth Health=>health;
 void Awake(){body=GetComponent<Rigidbody2D>();health=GetComponent<CombatHealth>();hitbox=GetComponent<Collider2D>();Sword=!aquatic;body.gravityScale=aquatic?balance.waterGravity:balance.landGravity;body.linearDamping=aquatic?balance.waterDrag:0;}
 void Start(){SetWeaponVisual();}
 void Update(){
  if(health.Dead){State=ActionState.Defeated;visual.Play("Death");return;}
  if(Locked||Time.timeScale==0){input=Vector2.zero;return;}
  var k=Keyboard.current; if(k==null)return;
  input=new Vector2((k.dKey.isPressed||k.rightArrowKey.isPressed?1:0)-(k.aKey.isPressed||k.leftArrowKey.isPressed?1:0),aquatic?(k.wKey.isPressed||k.upArrowKey.isPressed?1:0)-(k.sKey.isPressed||k.downArrowKey.isPressed?1:0):0);
  input=Vector2.ClampMagnitude(input,1);
  if(Mathf.Abs(input.x)>.01f&&Time.time>=actionUntil){Facing=Mathf.Sign(input.x);visual.Face(Facing);}
  if(aquatic&&input.sqrMagnitude>.01f)Aim=input; else if(!aquatic)Aim=Vector2.right*Facing;
  if(!aquatic&&k.spaceKey.wasPressedThisFrame)jumpBuffered=Time.time;
  if(k.qKey.wasPressedThisFrame)SwitchWeapon();
  if(!aquatic&&k.leftShiftKey.wasPressedThisFrame)Dash();
  if(k.jKey.isPressed || Mouse.current!=null&&Mouse.current.leftButton.isPressed)Attack();
  if(meleePending&&Time.time>=meleeAt){meleePending=false;MeleeHit();}
  if(Time.time<hurtUntil){State=ActionState.Hurt;visual.Play("Hurt");}
  else if(Time.time<dashUntil){State=ActionState.Dash;visual.Play("Dash");}
  else if(Time.time<actionUntil){} // La acción termina antes de recuperar la animación de movimiento.
  else if(aquatic){State=input.sqrMagnitude>.01f?ActionState.Run:ActionState.Idle;visual.Play(input.y>.25f?"Up":input.y<-.25f?"Down":input.sqrMagnitude>.01f?"Swim":IdleAnimation);}
  else if(!Grounded){State=body.linearVelocity.y>0?ActionState.Jump:ActionState.Fall;visual.Play(body.linearVelocity.y>0?"Jump":"Fall");}
  else if(Time.time<landUntil){State=ActionState.Land;visual.Play("Land");}
  else {State=Mathf.Abs(input.x)>.01f?ActionState.Run:ActionState.Idle;visual.Play(State==ActionState.Run?"Run":IdleAnimation);}
 }
 string IdleAnimation=>!Sword&&GetComponent<PlasmaMagazine>()?.Reloading==true?"Reload":"Idle";
 void LateUpdate(){ if(swordGlow){swordGlow.transform.localPosition=new Vector3(Facing*.7f,-.1f,0);swordGlow.SetActive(Sword&&!health.Dead&&Time.time>=actionUntil);}if(gunGlow){gunGlow.transform.localPosition=new Vector3(Facing*.6f,-.15f,0);gunGlow.SetActive(!Sword&&!health.Dead&&Time.time>=actionUntil);}}
 void FixedUpdate(){
  if(health.Dead||Locked){body.linearVelocity=Vector2.zero;return;}
  if(Time.time<hurtUntil)return;
  if(aquatic){body.linearVelocity=Vector2.ClampMagnitude(body.linearVelocity+input*balance.swimAcceleration*Time.fixedDeltaTime,balance.swimSpeed);return;}
  var b=hitbox.bounds;
  Grounded=Physics2D.BoxCast(new Vector2(b.center.x,b.min.y+.04f),new Vector2(b.size.x*.7f,.07f),0,Vector2.down,.14f,LayerMask.GetMask("Arena"));
  if(Grounded){lastGround=Time.time;if(!wasGrounded)landUntil=Time.time+.1f;}wasGrounded=Grounded;
  if(Time.time<dashUntil){body.linearVelocity=new Vector2(dashDirection*balance.dashSpeed,0);return;}
  body.linearVelocity=new Vector2(Mathf.MoveTowards(body.linearVelocity.x,input.x*balance.runSpeed,balance.acceleration*Time.fixedDeltaTime),body.linearVelocity.y);
  if(Time.time-jumpBuffered<.12f&&Time.time-lastGround<.10f){body.linearVelocity=new Vector2(body.linearVelocity.x,balance.jumpSpeed);jumpBuffered=-10;lastGround=-10;Grounded=false;AdventureFlow.Instance?.Effect(transform.position,Color.white);}
 }
 public bool SwitchWeapon(){if(aquatic||Locked||health.Dead||Time.time<actionUntil||Time.time<hurtUntil)return false;Sword=!Sword;State=ActionState.Switch;actionUntil=Time.time+.12f;visual.Play("Switch");SetWeaponVisual();return true;}
 void SetWeaponVisual(){if(swordGlow)swordGlow.SetActive(Sword);if(gunGlow)gunGlow.SetActive(!Sword);}
 public bool Dash(){if(aquatic||Locked||health.Dead||Time.time<dashNext||Time.time<hurtUntil||Time.time<actionUntil)return false;dashDirection=Mathf.Abs(input.x)>.1f?Mathf.Sign(input.x):Facing;dashUntil=Time.time+balance.dashDuration;dashNext=Time.time+balance.dashCooldown;AdventureFlow.Instance?.Effect(transform.position,new Color(.4f,1,1));return true;}
 public bool Attack(){
  if(Locked||health.Dead||Time.time<nextAttack||Time.time<hurtUntil||Time.time<dashUntil||Time.time<actionUntil||Time.timeScale==0)return false;
  if(!Sword&&GetComponent<PlasmaMagazine>() is PlasmaMagazine magazine&&!magazine.Consume())return false;
  nextAttack=Time.time+(Sword?balance.meleeCooldown:balance.plasmaCooldown);actionUntil=Time.time+(Sword?.38f:.27f);
  State=Sword?ActionState.Melee:ActionState.Ranged;visual.Play(Sword?"Melee":"Ranged");
  if(Sword){meleePending=true;meleeAt=Time.time+.14f;}
  else {Vector2 direction=aquatic?Aim:Vector2.right*Facing;Vector2 origin=(Vector2)transform.position+direction.normalized*1.1f; muzzle.position=origin;
   var shot=Instantiate(plasmaPrefab,origin,Quaternion.identity);shot.Launch(direction*balance.plasmaSpeed,health,balance.plasmaDamage,false);}
  return true;
 }
 void MeleeHit(){if(health.Dead||Locked||Time.time<hurtUntil)return;Vector2 center=(Vector2)transform.position+Vector2.right*Facing*1.35f;
  foreach(var c in Physics2D.OverlapBoxAll(center,new Vector2(2f,1.65f),0)){var enemy=c.GetComponent<AdventureBoss>();if(enemy!=null){enemy.Health.Damage(balance.swordDamage,new Vector2(Facing*2,0));AdventureFlow.Instance?.Effect(c.transform.position,Color.cyan);break;}}
 }
 public void Knockback(Vector2 force){GetComponent<AdventureEmotes>()?.Cancel();hurtUntil=Time.time+.25f;meleePending=false;dashUntil=0;body.linearVelocity=force;}
 public void Resume(){Locked=false;body.gravityScale=aquatic?balance.waterGravity:balance.landGravity;}
 public void Stop(){Locked=true;input=Vector2.zero;meleePending=false;body.linearVelocity=Vector2.zero;body.gravityScale=0;}
}
}





