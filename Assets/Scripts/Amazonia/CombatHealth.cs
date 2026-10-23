using System;
using UnityEngine;
namespace Amazonia {
public class CombatHealth : MonoBehaviour {
 public int maximum=8;
 public float invulnerability=.7f;
 public AdventureVisual visual;
 public int Current {get; private set;}
 public bool Dead=>Current<=0;
 public bool Protected;
 float nextHit;
 public event Action Died;
 void Awake(){Current=maximum;}
 public void Restore(int value){Current=Mathf.Clamp(value,1,maximum);nextHit=0;}
 public bool Damage(int amount,Vector2 impulse) {
  if(amount<=0||Dead||Protected||Time.time<nextHit||AdventureFlow.Instance!=null&&AdventureFlow.Instance.Finished)return false;
  Current=Mathf.Max(0,Current-amount);nextHit=Time.time+invulnerability;
  visual.Flash();
  var player=GetComponent<AdventurePlayer>();
  if(player!=null)player.Knockback(impulse);
  else if(!Dead) GetComponent<Rigidbody2D>().AddForce(impulse,ForceMode2D.Impulse);
  if(Dead){visual.Play("Death");Died?.Invoke();}
  return true;
 }
}
}
