using UnityEngine;
namespace Amazonia {
public class AdventureCamera : MonoBehaviour {
 public Transform target;
 public AdventureBoss boss;
 public Rect limits=new Rect(0,-3,66,24);
 public bool aquatic;
 Vector3 speed;
 Camera view;
 void Awake(){view=GetComponent<Camera>();}
 void LateUpdate(){if(!target)return;float aspect=(float)Screen.width/Mathf.Max(1,Screen.height);view.rect=new Rect(0,0,1,1);view.orthographicSize=5.3f;float half=view.orthographicSize*aspect;Vector3 wanted=target.position+new Vector3(2,aquatic?0:2.7f,-10);
 if(boss&&boss.isActiveAndEnabled&&!boss.Health.Dead&&Mathf.Abs(boss.transform.position.x-target.position.x)<13){wanted.x=Mathf.Lerp(target.position.x,boss.transform.position.x,.28f);}
 wanted.x=Mathf.Clamp(wanted.x,limits.xMin+half,limits.xMax-half);wanted.y=Mathf.Clamp(wanted.y,limits.yMin+view.orthographicSize,limits.yMax-view.orthographicSize);wanted.z=-10;
 transform.position=Vector3.SmoothDamp(transform.position,wanted,ref speed,.18f,100,Time.unscaledDeltaTime);
 }
}
}


