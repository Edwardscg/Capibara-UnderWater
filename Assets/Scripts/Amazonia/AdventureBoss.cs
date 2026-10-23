using UnityEngine;
namespace Amazonia {
[RequireComponent(typeof(Rigidbody2D),typeof(CombatHealth))]
public class AdventureBoss : MonoBehaviour {
 public enum BossState { Patrol,Chase,Positioning,Telegraph,Claw,Push,Rock,Charge,Recovery,Stagger,Retreat,Defeated }
 public bool aquatic;
 public AdventureBalance balance;
 public AdventurePlayer target;
 public AdventureVisual visual;
 public AdventureProjectile rockPrefab;
 public float lakeX=63;
 public BossState State {get; private set;}
 public CombatHealth Health {get; private set;}
 public Vector2 ChargeDirection {get; private set;}
 public bool Enraged=>aquatic&&Health.Current<=Health.maximum/2;
 public bool Stopped;
 public Rigidbody2D Body=>body;
 Rigidbody2D body;float timer;bool hit,executed,diving;float diveTime;BossState planned;int cycle;
 void Awake(){body=GetComponent<Rigidbody2D>();Health=GetComponent<CombatHealth>();State=BossState.Patrol;body.gravityScale=aquatic?0:2.8f;}
 void Start(){Health.Died+=OnDefeated;}
 void OnDestroy(){if(Health!=null)Health.Died-=OnDefeated;}
 void OnDefeated(){Health.Protected=true;visual.Warning=false;if(aquatic){Enter(BossState.Defeated);body.linearVelocity=Vector2.zero;visual.Play("Death");AdventureFlow.Instance.BossDefeated();}else{Enter(BossState.Stagger);visual.Play("Hurt");AdventureFlow.Instance.LandWon();}}
 void FixedUpdate(){
  if(Stopped||target==null||target.Health.Dead){body.linearVelocity=Vector2.zero;return;}
  timer+=Time.fixedDeltaTime;
  Vector2 delta=(Vector2)target.transform.position-body.position;
  float sign=delta.x>=0?1:-1;
  visual.Enraged=Enraged;
  if(State==BossState.Defeated){body.linearVelocity=new Vector2(0,-.4f);return;}
  if(State==BossState.Stagger){body.linearVelocity=new Vector2(0,body.linearVelocity.y);if(timer>1)Enter(BossState.Retreat);return;}
  if(State==BossState.Retreat){visual.Face(1);if(!diving){visual.Play("Run");body.linearVelocity=new Vector2(7,body.linearVelocity.y);if(body.position.x>lakeX){diving=true;diveTime=0;GetComponent<Collider2D>().enabled=false;AdventureFlow.Instance.Effect(transform.position,Color.cyan);}}else{diveTime+=Time.fixedDeltaTime;visual.Play("Death");body.linearVelocity=new Vector2(1.5f,-2);if(diveTime>.8f){AdventureFlow.Instance.OpenLake();gameObject.SetActive(false);}}return;}
  visual.Warning=State==BossState.Telegraph;
  switch(State){
   case BossState.Patrol:
    visual.Play("Idle");body.linearVelocity=new Vector2(0,aquatic?0:body.linearVelocity.y);
    if(delta.magnitude<(aquatic?19:17))Enter(BossState.Chase);break;
   case BossState.Chase:
    visual.Face(delta.x);visual.Play(aquatic?(Enraged?"Enraged":"Swim"):"Run");
    if(aquatic){body.linearVelocity=Steer(delta.normalized)*(balance.chaseWater+(Enraged?.7f:0));if(delta.magnitude<8&&timer>.6f||timer>3)Enter(BossState.Positioning);}
    else{body.linearVelocity=new Vector2(sign*balance.chaseLand,body.linearVelocity.y);
     if(Mathf.Abs(delta.x)<3.3f||timer>1.7f){planned=Mathf.Abs(delta.x)>6||delta.y>1.7f?BossState.Rock:cycle++%2==0?BossState.Claw:BossState.Push;Enter(BossState.Telegraph);}}
    break;
   case BossState.Positioning:
    visual.Face(delta.x);visual.Play("Swim");body.linearVelocity=delta.magnitude<5?Steer(-delta.normalized)*2:Vector2.zero;if(timer>.45f){planned=BossState.Charge;Enter(BossState.Telegraph);}break;
   case BossState.Telegraph:
    body.linearVelocity=new Vector2(0,aquatic?0:body.linearVelocity.y);visual.Face(delta.x);visual.Play(planned==BossState.Rock?"RockPrepare":"Telegraph");
    if(timer>(Enraged?balance.preparation*.78f:balance.preparation)){ChargeDirection=aquatic?delta.normalized:new Vector2(sign,0);Enter(planned);}break;
   case BossState.Claw:
    visual.Play("Claw");if(timer>.18f&&!executed){executed=true;
     Vector2 center=body.position+ChargeDirection*1.6f;
     foreach(var c in Physics2D.OverlapBoxAll(center,new Vector2(2.3f,1.8f),0))if(c.GetComponent<AdventurePlayer>()==target){DamagePlayer();break;}}
    if(timer>.5f)Enter(BossState.Recovery);break;
   case BossState.Push:
    visual.Play("Push");body.linearVelocity=new Vector2(ChargeDirection.x*8,body.linearVelocity.y);if(timer>.45f)Enter(BossState.Recovery);break;
   case BossState.Rock:
    visual.Play("Rock");if(timer>.25f&&!executed){executed=true;Vector2 origin=body.position+new Vector2(sign*1.9f,1.1f);Vector2 travel=(Vector2)target.transform.position-origin;float flight=Mathf.Clamp(Mathf.Abs(travel.x)/8,.55f,1.8f);Vector2 velocity=new Vector2(travel.x/flight,travel.y/flight+.5f*9.81f*1.3f*flight);Instantiate(rockPrefab,origin,Quaternion.identity).Launch(velocity,Health,1,true);}
    if(timer>.7f)Enter(BossState.Recovery);break;
   case BossState.Charge:
    visual.Play("Charge");body.linearVelocity=ChargeDirection*(balance.chargeSpeed+(Enraged?1:0));if(timer>.7f)Enter(BossState.Recovery);break;
   case BossState.Recovery:
    body.linearVelocity=new Vector2(0,aquatic?0:body.linearVelocity.y);visual.Play("Recovery");if(timer>balance.recovery*(Enraged?.82f:1))Enter(BossState.Chase);break;
  }
 }
 Vector2 Steer(Vector2 direction){int mask=LayerMask.GetMask("Arena");if(!Physics2D.CircleCast(body.position,.8f,direction,2,mask))return direction;Vector2 best=direction;float score=-100;for(int i=-3;i<=3;i++){Vector2 d=Quaternion.Euler(0,0,i*40)*direction;var h=Physics2D.CircleCast(body.position,.8f,d,3,mask);float s=(h?h.distance:3)+Vector2.Dot(direction,d)*.4f;if(s>score){score=s;best=d;}}return best;}
 void Enter(BossState state){State=state;timer=0;hit=false;executed=false;body.linearVelocity=new Vector2(0,aquatic?0:body.linearVelocity.y);}
 void OnCollisionEnter2D(Collision2D c)=>Contact(c);
 void OnCollisionStay2D(Collision2D c)=>Contact(c);
 void Contact(Collision2D c){if(Stopped||Health.Dead||State!=BossState.Charge&&State!=BossState.Push)return;if(c.gameObject.GetComponent<AdventurePlayer>()==target)DamagePlayer();else if(c.gameObject.layer==LayerMask.NameToLayer("Arena")&&(aquatic||Mathf.Abs(c.GetContact(0).normal.x)>.5f))Enter(BossState.Recovery);}
 void DamagePlayer(){if(hit)return;hit=true;target.Health.Damage(1,ChargeDirection*7+Vector2.up*2);}
}
}

