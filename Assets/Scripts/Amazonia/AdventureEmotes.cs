using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
namespace Amazonia {
[DefaultExecutionOrder(-90)]
public class AdventureEmotes : MonoBehaviour {
 public AudioClip wazaaaa,uiia,sixSeven;public bool Active {get;private set;}public bool WheelOpen {get;private set;}
 public bool ConsumedEscape=>escapeFrame==Time.frameCount;int escapeFrame=-1;GameObject wheel;Text[] options=new Text[3];int selected;float remaining;AdventurePlayer player;AdventureAudio audioRig;AdventureDirector director;AdventureFlow flow;
 void Start(){player=GetComponent<AdventurePlayer>();flow=AdventureFlow.Instance;director=flow.GetComponent<AdventureDirector>();audioRig=flow.GetComponent<AdventureAudio>();wheel=AdventureDirector.Rect("Rueda de emotes",flow.healthLabel.canvas.transform,Vector2.zero,new Vector2(420,320)).gameObject;wheel.AddComponent<Image>().color=new Color(.015f,.06f,.05f,.95f);var names=new[]{"WAZAAAA","UIIA","SIX SEVEN"};for(int i=0;i<3;i++){int index=i;Vector2 pos=i==0?new Vector2(0,90):new Vector2(i==1?-110:110,-35);options[i]=AdventureDirector.Label(names[i],wheel.transform,pos,new Vector2(195,70),names[i],22);options[i].raycastTarget=true;var b=options[i].gameObject.AddComponent<Button>();b.onClick.AddListener(()=>Play(index));}AdventureDirector.Label("Ayuda",wheel.transform,new Vector2(0,-120),new Vector2(410,50),"← → / 1 2 3 · ENTER\nESC: cancelar",17);wheel.SetActive(false);}
 public bool Allowed=>flow&&!flow.Finished&&!flow.Transitioning&&!flow.Paused&&!director.Busy&&(flow.LakeOpen||!director.IntroDone)&&!player.Health.Dead;
 void Update(){var k=Keyboard.current;if(k==null||!flow)return;if((Active||WheelOpen)&&(!Allowed||k.escapeKey.wasPressedThisFrame)){Cancel();return;}
  if(k.tKey.wasPressedThisFrame&&Allowed&&!Active){WheelOpen=!WheelOpen;wheel.SetActive(WheelOpen);player.Locked=WheelOpen;}
  if(WheelOpen){if(k.rightArrowKey.wasPressedThisFrame)selected=(selected+1)%3;if(k.leftArrowKey.wasPressedThisFrame)selected=(selected+2)%3;if(k.digit1Key.wasPressedThisFrame)selected=0;if(k.digit2Key.wasPressedThisFrame)selected=1;if(k.digit3Key.wasPressedThisFrame)selected=2;for(int i=0;i<3;i++)options[i].color=i==selected?Color.cyan:Color.white;if(k.enterKey.wasPressedThisFrame)Play(selected);return;}
  if(Active){if(k.aKey.isPressed||k.dKey.isPressed||k.wKey.isPressed||k.sKey.isPressed||k.leftArrowKey.isPressed||k.rightArrowKey.isPressed||k.upArrowKey.isPressed||k.downArrowKey.isPressed||k.jKey.isPressed||k.spaceKey.isPressed||Mouse.current?.leftButton.isPressed==true){Cancel();return;}remaining-=Time.deltaTime;if(remaining<=0)Cancel();}
 }
 public bool Play(int index){if(!Allowed||index<0||index>2)return false;Cancel();WheelOpen=false;wheel.SetActive(false);Active=true;player.Locked=true;var clip=index==0?wazaaaa:index==1?uiia:sixSeven;remaining=clip?clip.length:2;player.visual.Play(index==0?"Wazaaaa":index==1?"Uiia":"SixSeven");audioRig.Emote(clip);return true;}
 public void Cancel(){if(!Active&&!WheelOpen)return;Active=WheelOpen=false;if(wheel)wheel.SetActive(false);if(audioRig&&audioRig.voice)audioRig.voice.Stop();if(player&&!player.Health.Dead)player.Locked=false;}
}
}

