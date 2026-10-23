using UnityEngine;
namespace Amazonia {
public class AdventureAudio : MonoBehaviour {
 [Range(0,1)]public float master=1,music=.5f,sfx=.8f;
 public AudioClip exploration,landBattle,transition,waterExploration,waterBattle;
 public AudioSource effects,voice;
 AudioSource a,b;float blend=1;bool useA=true;
 void Awake(){a=gameObject.AddComponent<AudioSource>();b=gameObject.AddComponent<AudioSource>();a.loop=b.loop=true;a.playOnAwake=b.playOnAwake=false;effects=gameObject.AddComponent<AudioSource>();voice=gameObject.AddComponent<AudioSource>();}
 public void Music(AudioClip clip){var current=useA?a:b;if(current.clip==clip)return;useA=!useA;current=useA?a:b;current.clip=clip;blend=0;if(clip)current.Play();}
 void Update(){blend=Mathf.MoveTowards(blend,1,Time.unscaledDeltaTime);(useA?a:b).volume=blend*master*music;(useA?b:a).volume=(1-blend)*master*music;effects.volume=voice.volume=master*sfx;}
 public void Emote(AudioClip clip){voice.Stop();voice.clip=clip;if(clip)voice.Play();}
}
}
