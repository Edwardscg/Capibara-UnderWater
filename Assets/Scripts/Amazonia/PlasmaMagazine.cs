using UnityEngine;
namespace Amazonia {
// Un cargador por jugador. El reloj sólo avanza durante la simulación jugable.
public class PlasmaMagazine : MonoBehaviour {
 public const int Capacity=3; public const float ReloadSeconds=6f;
 public int Ammo {get;private set;}=Capacity;
 public float Remaining {get;private set;}
 public bool Reloading=>Remaining>0;
 public float Progress=>Reloading?1-Remaining/ReloadSeconds:1;
 void Update(){var f=AdventureFlow.Instance;if(f==null||f.Finished||f.Transitioning||f.Paused||f.GetComponent<AdventureDirector>()?.Busy==true)return;Tick(Time.deltaTime);}
 public void Tick(float seconds){if(!Reloading)return;Remaining=Mathf.Max(0,Remaining-seconds);if(Remaining==0){Ammo=Capacity;AdventureFlow.Instance?.Effect(transform.position,Color.cyan);}}
 public bool Consume(){if(Ammo<=0||Reloading)return false;Ammo--;if(Ammo==0)Remaining=ReloadSeconds;return true;}
 public void Restore(int ammo,float remaining){Ammo=Mathf.Clamp(ammo,0,Capacity);Remaining=Mathf.Clamp(remaining,0,ReloadSeconds);if(Ammo==0&&Remaining==0)Remaining=ReloadSeconds;}
}
}

