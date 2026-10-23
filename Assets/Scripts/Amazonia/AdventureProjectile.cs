using UnityEngine;
namespace Amazonia {
[RequireComponent(typeof(Rigidbody2D),typeof(Collider2D))]
public class AdventureProjectile : MonoBehaviour {
 public float lifetime=3;
 Rigidbody2D body;Collider2D shape;CombatHealth owner;int damage;bool enemy,spent;
 readonly RaycastHit2D[] hits=new RaycastHit2D[12];
 void Awake(){body=GetComponent<Rigidbody2D>();shape=GetComponent<Collider2D>();}
 public void Launch(Vector2 velocity,CombatHealth source,int amount,bool hostile){owner=source;damage=amount;enemy=hostile;body.linearVelocity=velocity;body.gravityScale=hostile?1.3f:0;transform.rotation=Quaternion.Euler(0,0,Mathf.Atan2(velocity.y,velocity.x)*Mathf.Rad2Deg);Physics2D.IgnoreCollision(shape,source.GetComponent<Collider2D>());Destroy(gameObject,lifetime);}
 void FixedUpdate(){
  if(spent||owner==null)return;
  Vector2 delta=body.linearVelocity*Time.fixedDeltaTime;
  int count=Physics2D.CircleCast(transform.position,.12f,delta.normalized,new ContactFilter2D().NoFilter(),hits,delta.magnitude);
  for(int i=0;i<count;i++)if(Hit(hits[i].collider))break;
 }
 void OnTriggerEnter2D(Collider2D c){Hit(c);}
 bool Hit(Collider2D c){
  if(spent||c==null||c==shape||c.isTrigger)return false;
  var target=c.GetComponent<CombatHealth>();
  if(target==owner)return false;
  bool valid=target!=null&&(enemy?target.GetComponent<AdventurePlayer>()!=null:target.GetComponent<AdventureBoss>()!=null);
  if(!valid&&c.gameObject.layer!=LayerMask.NameToLayer("Arena"))return false;
  spent=true;if(valid)target.Damage(damage,body.linearVelocity.normalized*5+Vector2.up*1.5f);
  AdventureFlow.Instance?.Effect(transform.position,enemy?new Color(.65f,.5f,.3f):Color.cyan);Destroy(gameObject);return true;
 }
}
}
