using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Video;
namespace Amazonia {
// Secuencias pequeñas y cancelables; no gobierna las físicas de combate.
[DefaultExecutionOrder(-100)]
public class AdventureDirector : MonoBehaviour {
 [System.Serializable] public class Line {public string speaker;[TextArea]public string text;public bool caiman;public Line(string s,string t,bool c=false){speaker=s;text=t;caiman=c;}}
 public Line[] landIntro={new Line("Narrador","El agua estaba más turbia de lo normal. La selva parecía contener la respiración."),new Line("Caimán","¡Alto ahí! Nadie cruza mi orilla sin permiso.",true),new Line("Capibara","Solo busco saber quién está ensuciando el lago."),new Line("Caimán","Eso dicen todos los que llegan. ¡Fuera de mi territorio!",true)};
 public Line[] escape={new Line("Capibara","¿Eso cuenta como ganar?"),new Line("Caimán","No cantes victoria... aquí abajo mando yo.",true)};
 public Line[] immersion={new Line("Capibara","No puedo dejarlo escapar... y todavía necesito respuestas.")};
 public Line[] waterIntro={new Line("Narrador","Bajo las raíces, el lago ocultaba cicatrices que la superficie no mostraba."),new Line("Caimán","Entraste a mis aguas. Aquí tus saltos no servirán.",true),new Line("Capibara","Entonces tendré que aprender a nadar más rápido.")};
 public Line[] ending={new Line("Narrador","Entre los sedimentos yacían restos de metal y marcas de maquinaria. El verdadero peligro no tenía escamas."),new Line("Capibara","Esto no terminó aquí... alguien está dañando la selva.")};
 public Sprite capyPortrait,caimanPortrait,capsuleFull,capsuleEmpty,dialogFrame;
 public VideoClip escapeClip,immersionClip;
 public string escape_video_path="",immersion_video_path="";
 public bool Busy {get;private set;} public bool IntroDone {get;private set;}
 public bool VideoPlaying=>video&&video.isPlaying;public bool VideoPrepared {get;private set;}
 public bool SkipRequested {get;private set;} public long VideoFrame=>video?video.frame:-1; public void AdvanceDialogue(){advanceDialogue=true;} bool advanceDialogue;
 AdventureFlow flow;AdventureCamera cameraRig;AdventureAudio audioRig;GameObject dialog,videoPanel;Text dialogText,title,ammoText;Image portrait,reload;Image[] capsules=new Image[3];VideoPlayer video;RawImage screen;RenderTexture videoTexture;
 public static RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=Vector2.one*.5f;r.anchoredPosition=pos;r.sizeDelta=size;return r;}
 public static Text Label(string name,Transform parent,Vector2 pos,Vector2 size,string value,int font=22){var t=Rect(name,parent,pos,size).gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=font;t.color=Color.white;t.alignment=TextAnchor.MiddleCenter;t.text=value;t.raycastTarget=false;return t;}
 void Awake(){flow=GetComponent<AdventureFlow>();audioRig=GetComponent<AdventureAudio>();}
 void Start(){cameraRig=Camera.main.GetComponent<AdventureCamera>();BuildUI();flow.boss.Stopped=true;flow.boss.Health.Protected=true;flow.boss.visual.sprite.enabled=false;flow.bossLabel.gameObject.SetActive(false);flow.bossFill.transform.parent.gameObject.SetActive(false);audioRig.Music(flow.player.aquatic?audioRig.waterExploration:audioRig.exploration);if(flow.player.aquatic&&AdventureFlow.WaterRetry){IntroDone=true;ShowBossBar();flow.boss.visual.sprite.enabled=true;flow.boss.Health.Protected=false;flow.boss.Stopped=false;} }
 void ShowBossBar(){flow.bossLabel.gameObject.SetActive(true);flow.bossFill.transform.parent.gameObject.SetActive(true);}
 void BuildUI(){var canvas=flow.healthLabel.canvas.transform;
  dialog=Rect("Diálogo narrativo",canvas,new Vector2(0,-255),new Vector2(1120,240)).gameObject;var bg=dialog.AddComponent<Image>();bg.color=new Color(.02f,.06f,.045f,.98f);if(dialogFrame){bg.sprite=dialogFrame;bg.color=Color.white;}
  portrait=Rect("Retrato",dialog.transform,new Vector2(-435,0),new Vector2(180,180)).gameObject.AddComponent<Image>();portrait.preserveAspect=true;
  dialogText=Label("Texto editable",dialog.transform,new Vector2(100,10),new Vector2(820,130),"",26);Label("Continuar",dialog.transform,new Vector2(10,-80),new Vector2(600,35),"ENTER / CLIC: continuar · ESC: omitir",18);dialog.SetActive(false);
  var next=Label("Botón continuar",dialog.transform,new Vector2(410,-80),new Vector2(180,35),"CONTINUAR →",18);next.raycastTarget=true;next.gameObject.AddComponent<Button>().onClick.AddListener(AdvanceDialogue);
  title=Label("Presentación",canvas,new Vector2(0,180),new Vector2(1200,110),"",46);
  ammoText=Label("Cargador",canvas,new Vector2(640,360),new Vector2(300,45),"",17);
  for(int i=0;i<3;i++){capsules[i]=Rect("Cápsula "+(i+1),canvas,new Vector2(590+i*50,302),new Vector2(38,60)).gameObject.AddComponent<Image>();capsules[i].preserveAspect=true;}
  var back=Rect("Recarga",canvas,new Vector2(640,260),new Vector2(200,10)).gameObject.AddComponent<Image>();back.color=new Color(.05f,.12f,.12f);reload=Rect("Progreso continuo",back.transform,Vector2.zero,new Vector2(200,10)).gameObject.AddComponent<Image>();reload.color=Color.cyan;reload.rectTransform.pivot=new Vector2(0,.5f);reload.rectTransform.anchoredPosition=new Vector2(-100,0);
  videoPanel=Rect("Video con bandas",canvas,Vector2.zero,new Vector2(4000,3000)).gameObject;videoPanel.AddComponent<Image>().color=Color.black;screen=Rect("Video",videoPanel.transform,Vector2.zero,new Vector2(1600,900)).gameObject.AddComponent<RawImage>();Label("Omitir video",videoPanel.transform,new Vector2(0,-410),new Vector2(900,40),"ESC: OMITIR VIDEO",22);videoPanel.SetActive(false);
  video=gameObject.AddComponent<VideoPlayer>();video.playOnAwake=false;video.renderMode=VideoRenderMode.RenderTexture;video.audioOutputMode=VideoAudioOutputMode.AudioSource;video.controlledAudioTrackCount=1;video.SetTargetAudioSource(0,audioRig.effects);
 }
 void Update(){if(flow==null||!flow.player)return;var magazine=flow.player.GetComponent<PlasmaMagazine>();if(ammoText&&magazine){ammoText.text=magazine.Reloading?$"0 / 3 · RECARGANDO {magazine.Remaining:0.0}s":"PLASMA "+magazine.Ammo+" / 3";reload.rectTransform.sizeDelta=new Vector2(200*magazine.Progress,10);for(int i=0;i<3;i++){capsules[i].sprite=i<magazine.Ammo?capsuleFull:capsuleEmpty;capsules[i].color=i<magazine.Ammo?Color.white:new Color(.45f,.6f,.6f);}}
  if(!Busy&&flow.LakeOpen&&cameraRig.focus==flow.boss.transform)cameraRig.focus=null;
  if(Busy&&Keyboard.current?.escapeKey.wasPressedThisFrame==true)RequestSkip();
  if(!IntroDone&&!Busy&&!flow.Finished&&(flow.player.aquatic||flow.player.transform.position.x>21))StartCoroutine(Intro());
 }
 public void RequestSkip(){SkipRequested=true;}
 void Lock(){Busy=true;SkipRequested=false;flow.player.GetComponent<AdventureEmotes>()?.Cancel();flow.player.Stop();flow.player.Health.Protected=true;flow.boss.Stopped=true;foreach(var p in FindObjectsByType<AdventureProjectile>())Destroy(p.gameObject);}
 void Unlock(){dialog.SetActive(false);title.text="";cameraRig.focus=null;Busy=false;SkipRequested=false;flow.player.Health.Protected=false;if(!flow.Finished)flow.player.Resume();}
 IEnumerator Wait(float duration){for(float t=0;t<duration&&!SkipRequested;t+=Time.deltaTime)yield return null;}
 public IEnumerator Talk(Line[] lines){dialog.SetActive(true);foreach(var line in lines){if(SkipRequested)break;portrait.sprite=line.caiman?caimanPortrait:capyPortrait;dialogText.text=line.speaker+"\n"+line.text;advanceDialogue=false;yield return null;while(!SkipRequested){var k=Keyboard.current;if(advanceDialogue||k!=null&&k.enterKey.wasPressedThisFrame||Mouse.current!=null&&Mouse.current.leftButton.wasPressedThisFrame)break;yield return null;}}dialog.SetActive(false);}
 IEnumerator Intro(){Lock();IntroDone=true;cameraRig.focus=flow.boss.transform;flow.boss.visual.sprite.enabled=true;flow.boss.visual.Face(-1);flow.boss.visual.Play("Appear");yield return Wait(1.5f);yield return Talk(flow.player.aquatic?waterIntro:landIntro);ShowBossBar();title.text=flow.player.aquatic?"CAIMÁN\nFASE II: AGUAS PROFUNDAS":"CAIMÁN\nGUARDIÁN DEL LAGO";flow.boss.visual.Play("Telegraph");yield return Wait(1.2f);flow.boss.Health.Protected=false;Unlock();flow.boss.Stopped=false;audioRig.Music(flow.player.aquatic?audioRig.waterBattle:audioRig.landBattle);}
 public void LandDefeated(){StartCoroutine(EscapeSequence());}
 IEnumerator EscapeSequence(){Lock();cameraRig.focus=flow.boss.transform;flow.boss.visual.Play("Hurt");yield return Wait(.8f);flow.player.visual.Play("Celebrate");yield return Wait(1.1f);yield return Talk(escape);if(!SkipRequested)yield return PlayVideo(escapeClip,escape_video_path);if(!VideoPrepared&&!SkipRequested){flow.player.visual.Play("Ranged");flow.Effect(flow.boss.transform.position,Color.gray);yield return Wait(.6f);flow.boss.visual.Play("Rock");flow.Effect(flow.player.transform.position,Color.gray);yield return Wait(.7f);}Unlock();cameraRig.focus=flow.boss.transform;flow.boss.BeginRetreat();audioRig.Music(audioRig.transition);}
 public IEnumerator PrepareDive(){Lock();cameraRig.focus=flow.player.transform;yield return Talk(immersion);flow.player.visual.Play("DivePrepare");yield return Wait(.8f);if(!SkipRequested)yield return PlayVideo(immersionClip,immersion_video_path);flow.player.visual.Play("Dive");yield return Wait(.6f);Unlock();flow.player.Stop();}
 public IEnumerator Ending(){Lock();flow.player.visual.Play("Celebrate");yield return Wait(.8f);yield return Talk(ending);Unlock();}
 public IEnumerator PlayVideo(VideoClip clip,string path){VideoPrepared=false;if(!clip&&string.IsNullOrWhiteSpace(path))yield break;bool failed=false;VideoPrepared=false;VideoPlayer.ErrorEventHandler error=(v,m)=>{failed=true;Debug.LogWarning("Video no disponible; continúa la animación alternativa: "+m);};video.errorReceived+=error;video.source=string.IsNullOrWhiteSpace(path)?VideoSource.VideoClip:VideoSource.Url;if(video.source==VideoSource.VideoClip)video.clip=clip;else video.url=path;video.Prepare();float timeout=Time.realtimeSinceStartup+10;
  while(!video.isPrepared&&!failed&&!SkipRequested&&Time.realtimeSinceStartup<timeout)yield return null;
  if(video.isPrepared&&!failed&&!SkipRequested){VideoPrepared=true;float ratio=(float)video.width/Mathf.Max(1,video.height);var canvasRect=flow.healthLabel.canvas.GetComponent<RectTransform>().rect;float maxWidth=canvasRect.width,maxHeight=canvasRect.height;float h=maxHeight,w=h*ratio;if(w>maxWidth){w=maxWidth;h=w/ratio;}videoPanel.transform.Find("Omitir video").GetComponent<RectTransform>().anchoredPosition=new Vector2(0,-maxHeight*.5f+32);screen.rectTransform.sizeDelta=new Vector2(w,h);videoTexture=new RenderTexture((int)video.width,(int)video.height,0);videoTexture.Create();video.targetTexture=videoTexture;screen.texture=videoTexture;videoPanel.SetActive(true);video.Play();yield return null;timeout=Time.realtimeSinceStartup+(float)video.length+5;while(!SkipRequested&&!failed&&Time.realtimeSinceStartup<timeout&&(video.isPlaying||video.frame<1))yield return null;}
  video.Stop();videoPanel.SetActive(false);video.targetTexture=null;screen.texture=null;if(videoTexture){videoTexture.Release();Destroy(videoTexture);}video.errorReceived-=error;
 }
 void OnDestroy(){if(video)video.Stop();if(videoTexture){videoTexture.Release();Destroy(videoTexture);}}
}
}


