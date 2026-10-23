using UnityEngine;
namespace Amazonia {
public class AdventureVisual : MonoBehaviour {
 public Animator animator;
 public SpriteRenderer sprite;
 public bool facesLeft;
 string current;
 float flash;
 public void Face(float x) { if(Mathf.Abs(x)>.01f) sprite.flipX=facesLeft ? x>0 : x<0; }
 public void Play(string state) {
  if(Time.time<flash&&state!="Hurt"&&state!="Death")return;
  if(current==state)return;
  int id=Animator.StringToHash(state);
  if(!animator.HasState(0,id))return;
  current=state; animator.Play(id,0,0);
 }
 public void Flash() {flash=Time.time+.18f;Play("Hurt");}
 public bool Warning,Enraged;
 void LateUpdate(){sprite.color=Time.time<flash ? new Color(1,.45f,.4f) : Warning?Color.Lerp(Color.white,new Color(1,.35f,.12f),(Mathf.Sin(Time.time*23)+1)*.5f):Enraged?new Color(1,.8f,.7f):Color.white;}
}
}

