using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace Amazonia {
public class AdventureFlow : MonoBehaviour {
 public static AdventureFlow Instance {get; private set;}
 static int savedHP;static bool fromLand;
 public AdventurePlayer player;public AdventureBoss boss;
 public Text healthLabel,bossLabel,status,weaponLabel,controls,resultTitle,resultDetail;
 public Image healthFill,bossFill,fade;
 public GameObject resultPanel;
 public ParticleSystem impact;
 public bool Finished {get; private set;}
 public bool LakeOpen {get; private set;}
 public bool Transitioning {get; private set;}
 public bool Paused {get; private set;}
 public bool Won {get; private set;}
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]static void ResetStatics(){savedHP=0;fromLand=false;Instance=null;}
 void Awake(){Instance=this;Time.timeScale=1;}
 void Start(){if(!player.aquatic){fromLand=false;savedHP=0;}else if(fromLand){player.Health.Restore(savedHP);fromLand=false;}
  player.Health.Died+=Lose;resultPanel.SetActive(false);StartCoroutine(FadeIn());}
 void Update(){
  var k=Keyboard.current;
  if(k!=null&&k.escapeKey.wasPressedThisFrame&&!Finished&&!Transitioning)Pause();
  if(Finished&&k!=null&&k.rKey.wasPressedThisFrame)Restart();
  healthFill.fillAmount=(float)player.Health.Current/player.Health.maximum;bossFill.fillAmount=(float)boss.Health.Current/boss.Health.maximum;
  healthLabel.text="CAPIBARA   "+player.Health.Current+" / "+player.Health.maximum;
  bossLabel.text="CAIMÁN   /   "+(player.aquatic?"LAGO":"SELVA")+"   "+boss.Health.Current+" / "+boss.Health.maximum;
  weaponLabel.text=player.aquatic?"PLASMA  ·  ESPADA NO DISPONIBLE EN AGUA":(player.Sword?"ESPADA DE LUZ":"CAÑÓN DE PLASMA")+"   |   DASH "+(player.DashReady>=1?"LISTO":Mathf.RoundToInt(player.DashReady*100)+"%");
  if(Finished||Paused||Transitioning)return;
  if(LakeOpen){bool near=player.transform.position.x>58;status.text=near?"[ E ]  SUMERGIRSE · SIGUE AL CAIMÁN":"EL CAIMÁN ESCAPÓ · VE A LA ORILLA →";if(near&&k!=null&&k.eKey.wasPressedThisFrame)TryDive();}
  else if(!player.aquatic&&boss.Health.Dead)status.text="EL CAIMÁN HUYE HACIA EL LAGO →";
  else status.text=(boss.Enraged?"¡FURIA!  ·  ":"")+(boss.State==AdventureBoss.BossState.Telegraph?"¡PELIGRO! PREPÁRATE PARA ESQUIVAR":boss.State==AdventureBoss.BossState.Recovery?"AGOTADO · ¡ATACA!":boss.State==AdventureBoss.BossState.Patrol?(player.aquatic?"LAGO AMAZÓNICO · SIGUE AL CAIMÁN":"AMAZONÍA PERUANA · EXPLORA LA ORILLA"):player.aquatic?"CAMBIA DE ALTURA PARA ESQUIVAR":"SALTA, USA LAS PLATAFORMAS Y ATACA");
 }
 IEnumerator FadeIn(){fade.gameObject.SetActive(true);for(float t=0;t<.6f;t+=Time.deltaTime){fade.color=new Color(0,.025f,.03f,1-t/.6f);yield return null;}fade.gameObject.SetActive(false);}
 public void LandWon(){foreach(var shot in FindObjectsByType<AdventureProjectile>())Destroy(shot.gameObject);}
 public void OpenLake(){LakeOpen=true;}
 public bool TryDive(){if(player.aquatic||!LakeOpen||player.transform.position.x<58||Transitioning||Finished)return false;StartCoroutine(Dive());return true;}
 IEnumerator Dive(){Transitioning=true;player.Stop();status.text="DESCENDIENDO AL LAGO…";Vector3 start=player.transform.position;for(float t=0;t<1.1f;t+=Time.deltaTime){player.transform.position=Vector3.Lerp(start,new Vector3(64,-1,0),t/1.1f)+Vector3.up*Mathf.Sin(t/1.1f*Mathf.PI)*1.3f;player.visual.Face(1);player.visual.Play(t<.55f?"Jump":"Fall");yield return null;}Effect(player.transform.position,Color.cyan);savedHP=player.Health.Current;fromLand=true;fade.gameObject.SetActive(true);for(float t=0;t<.6f;t+=Time.deltaTime){fade.color=new Color(0,.025f,.03f,t/.6f);yield return null;}SceneManager.LoadScene("AmazoniaAcuatica");}
 public void BossDefeated(){if(!Finished)StartCoroutine(Victory());}
 IEnumerator Victory(){Finished=true;Won=true;player.Stop();foreach(var shot in FindObjectsByType<AdventureProjectile>())Destroy(shot.gameObject);Effect(boss.transform.position,Color.cyan);yield return new WaitForSeconds(1.2f);boss.Stopped=true;PlayerPrefs.SetInt("AmazoniaCompleted",1);PlayerPrefs.Save();ShowResult("¡VICTORIA!","El caimán ha sido derrotado.\nLos residuos del lago revelan una amenaza humana mayor.");}
 void Lose(){if(Finished)return;Finished=true;boss.Stopped=true;player.Stop();foreach(var shot in FindObjectsByType<AdventureProjectile>())Destroy(shot.gameObject);ShowResult("DERROTA","Observa la preparación y aprovecha la recuperación del caimán.");}
 void ShowResult(string title,string detail){resultPanel.SetActive(true);resultTitle.text=title;resultDetail.text=detail;}
 public void Pause(){Paused=!Paused;Time.timeScale=Paused?0:1;resultPanel.SetActive(Paused);resultTitle.text="PAUSA";resultDetail.text="Escape para continuar · R después de terminar para reintentar";}
 public void Restart(){Time.timeScale=1;savedHP=0;fromLand=false;SceneManager.LoadScene("AmazoniaTerrestre");}
 public void Effect(Vector3 position,Color color){if(!impact)return;var fx=Instantiate(impact,position,Quaternion.identity);var main=fx.main;main.startColor=color;fx.Play();Destroy(fx.gameObject,1.5f);}
 void OnDestroy(){if(player!=null&&player.Health!=null)player.Health.Died-=Lose;if(Instance==this)Instance=null;Time.timeScale=1;}
}
}

