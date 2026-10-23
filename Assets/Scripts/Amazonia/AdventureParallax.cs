using UnityEngine;
namespace Amazonia {
public class AdventureParallax : MonoBehaviour {
 public float factor=.08f;Vector3 origin,camOrigin;Transform cam;
 void Start(){origin=transform.position;cam=Camera.main.transform;camOrigin=cam.position;}
 void LateUpdate(){if(cam)transform.position=origin+new Vector3((cam.position.x-camOrigin.x)*factor,(cam.position.y-camOrigin.y)*factor*.3f,0);}
}
}
