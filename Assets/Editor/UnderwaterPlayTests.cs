using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnderwaterAdventure;

// Pruebas de integración reproducibles, ejecutadas con el motor físico real en Play.
// No se incluyen en una build. No necesitan instalar paquetes adicionales.
[InitializeOnLoad]
public static class UnderwaterPlayTests
{
    static IEnumerator routine;
    static float resumeAt;
    static Keyboard keyboard, previousKeyboard;
    static Key[] held = Array.Empty<Key>();
    static readonly List<string> results = new List<string>();
    static readonly List<string> errors = new List<string>();
    static PlayerController player;
    static PlayerHealth playerHealth;
    static Rigidbody2D playerBody, bossBody;
    static BossController boss;
    static BossHealth bossHealth;
    static InputSettings.BackgroundBehavior previousBehavior;
    static bool previousRunInBackground;
    static UnderwaterPlayTests()
    {
        EditorApplication.update += Tick;
        EditorApplication.playModeStateChanged += OnPlayMode;
    }
    [MenuItem("Underwater/Probar nivel automáticamente")]
    public static void Run()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        Audit();
        SessionState.SetBool("UnderwaterTests",true);
        EditorApplication.isPlaying=true;
    }
    static void OnPlayMode(PlayModeStateChange state)
    {
        if (state==PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("UnderwaterTests",false))
        {
            SessionState.SetBool("UnderwaterTests",false);
            results.Clear();errors.Clear();
            Application.logMessageReceived += Log;
            previousKeyboard=Keyboard.current;keyboard=InputSystem.AddDevice<Keyboard>();keyboard.MakeCurrent();
            previousBehavior=InputSystem.settings.backgroundBehavior;
            previousRunInBackground=Application.runInBackground;
            InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
            Application.runInBackground=true;
            routine=Suite();resumeAt=0;
        }
        if(state==PlayModeStateChange.ExitingPlayMode && routine!=null) Finish("Interrumpido antes de finalizar");
    }
    static void Log(string message,string stack,LogType type)
    { if(type==LogType.Error || type==LogType.Exception || type==LogType.Assert)errors.Add(message); }
    static void Tick()
    {
        if(!EditorApplication.isCompiling && !EditorApplication.isUpdating && !EditorApplication.isPlayingOrWillChangePlaymode && File.Exists("Temp/UnderwaterWork/test.request"))
        {File.Delete("Temp/UnderwaterWork/test.request");Run();}
        if(routine==null || !EditorApplication.isPlaying)return;
        if(keyboard!=null)InputSystem.QueueStateEvent(keyboard,new KeyboardState(held));
        if(Time.time<resumeAt)return;
        try
        {
            if(!routine.MoveNext()){Finish(null);EditorApplication.isPlaying=false;return;}
            resumeAt=Time.time+(routine.Current is float seconds?seconds:0);
        }
        catch(Exception e){results.Add("FAIL: "+e.Message);Finish(e.ToString());EditorApplication.isPlaying=false;}
    }
    static void Finish(string failure)
    {
        routine=null;held=Array.Empty<Key>();
        Application.logMessageReceived -= Log;
        if(keyboard!=null){InputSystem.RemoveDevice(keyboard);keyboard=null;}
        if(previousKeyboard!=null && previousKeyboard.added)previousKeyboard.MakeCurrent();
        InputSystem.settings.backgroundBehavior=previousBehavior;
        Application.runInBackground=previousRunInBackground;
        if(failure!=null)results.Add(failure);
        results.Add(errors.Count==0?"PASS: Sin errores o excepciones durante Play":"FAIL: Errores de ejecución: "+string.Join(" | ",errors));
        File.WriteAllLines("Documentation/PlayTestResults.txt",new[]{"Unity "+Application.unityVersion,"Pruebas realizadas "+DateTime.Now.ToString("s")}.Concat(results));
        Debug.Log("UNDERWATER TESTS: "+string.Join("\n",results));
    }
    static void Check(bool condition,string message)
    {results.Add((condition?"PASS: ":"FAIL: ")+message);if(!condition)throw new Exception(message);}
    static void Acquire()
    {
        player=UnityEngine.Object.FindAnyObjectByType<PlayerController>();playerHealth=player.GetComponent<PlayerHealth>();playerBody=player.GetComponent<Rigidbody2D>();
        boss=UnityEngine.Object.FindAnyObjectByType<BossController>();bossHealth=boss.GetComponent<BossHealth>();bossBody=boss.GetComponent<Rigidbody2D>();
    }
    static void Place(Rigidbody2D body,Vector2 position){body.position=position;body.linearVelocity=Vector2.zero;Physics2D.SyncTransforms();}
    static void Keys(params Key[] keys){held=keys;InputSystem.QueueStateEvent(keyboard,new KeyboardState(keys));}
    static IEnumerator Suite()
    {
        yield return 0.4f;
        Acquire();boss.enabled=false;bossBody.linearVelocity=Vector2.zero;
        Check(playerHealth.Current==5 && bossHealth.Current==12,"Vida inicial 5 / 12");
        Check(playerBody.bodyType==RigidbodyType2D.Dynamic && playerBody.freezeRotation && playerBody.gravityScale==0.16f,"Rigidbody Player dinámico, rotación bloqueada, gravedad 0.16");
        Check(bossBody.bodyType==RigidbodyType2D.Dynamic && bossBody.gravityScale==0,"Rigidbody Boss dinámico sin gravedad");
        ScreenCapture.CaptureScreenshot("Documentation/UnderwaterBoss-Play.png");
        var movementKeys=new[]{Key.A,Key.D,Key.W,Key.S,Key.LeftArrow,Key.RightArrow,Key.UpArrow,Key.DownArrow};
        foreach(var key in movementKeys)
        {
            Place(playerBody,new Vector2(-4,1));Keys(key);yield return 0.6f;
            var delta=playerBody.position-new Vector2(-4,1);
            bool success=(key==Key.A||key==Key.LeftArrow)?delta.x < -0.8f:(key==Key.D||key==Key.RightArrow)?delta.x>0.8f:(key==Key.W||key==Key.UpArrow)?delta.y>0.8f:delta.y < -0.8f;
            Check(success,"Movimiento físico "+key+" (desplazamiento "+delta+")");Keys();yield return 0.05f;
        }
        Place(playerBody,new Vector2(-4,1));Keys(Key.W,Key.D);yield return 0.8f;
        Check(playerBody.linearVelocity.magnitude<4.6f,"Velocidad diagonal limitada");
        Keys();Place(playerBody,new Vector2(-4,1));yield return 1.4f;
        float sink=1-playerBody.position.y;
        Check(sink>0.25f && sink<1.1f,"Descenso lento al soltar controles: "+sink.ToString("F2")+" unidades / 1.4 s");
        Place(playerBody,new Vector2(11.5f,0));Keys(Key.D);yield return 1f;Check(playerBody.position.x<12.2f,"Pared derecha contiene al Player");
        Place(playerBody,new Vector2(-11.5f,0));Keys(Key.A);yield return 1f;Check(playerBody.position.x > -12.2f,"Pared izquierda contiene al Player");
        Place(playerBody,new Vector2(0,5));Keys(Key.W);yield return 1f;Check(playerBody.position.y<5.8f,"Límite superior contiene al Player");
        Place(playerBody,new Vector2(-3,-5));Keys(Key.S);yield return 1f;Check(playerBody.position.y > -5.8f,"Fondo contiene al Player");
        Place(playerBody,new Vector2(1,0.1f));Keys(Key.S);yield return 0.8f;Check(playerBody.position.y > -1f,"Roca central bloquea al Player");Keys();
        Place(playerBody,new Vector2(-3,1));Place(bossBody,new Vector2(2,1));Keys(Key.D);yield return 0.04f;Keys();
        player.enabled=false;playerBody.gravityScale=0;playerBody.linearVelocity=Vector2.zero;
        var attack=player.GetComponent<PlayerAttack>();
        Check(attack.TryFire(),"Disparo permitido");Check(!attack.TryFire(),"Cooldown impide disparo duplicado");yield return 0.6f;
        Check(bossHealth.Current==11,"Proyectil físico impacta y resta 1 HP al Boss");
        Check(playerHealth.Current==5,"El proyectil no daña al Player");
        Check(playerHealth.TakeDamage(1),"Player recibe daño");Check(!playerHealth.TakeDamage(1)&&playerHealth.Current==4,"Invulnerabilidad bloquea daño repetido");yield return 0.75f;
        Check(playerHealth.TakeDamage(1)&&playerHealth.Current==3,"Invulnerabilidad termina después de 0.7 s");
        bossHealth.TakeDamage(5);Check(bossHealth.Current==6&&bossHealth.PhaseTwo,"Segunda fase activada exactamente al 50%");
        boss.enabled=true;Place(playerBody,new Vector2(-3,1));Place(bossBody,new Vector2(2,1));
        float deadline=Time.time+6;bool chase=false,telegraph=false,charge=false,recovery=false,locked=false,chargeHit=false;Vector2 fixedDirection=Vector2.zero;float telegraphStart=0,telegraphDuration=0;
        while(Time.time<deadline && !recovery)
        {
            if(boss.CurrentState==BossController.State.Chase)chase=true;
            if(boss.CurrentState==BossController.State.Telegraph&&!telegraph){telegraph=true;telegraphStart=Time.time;}
            if(boss.CurrentState==BossController.State.Charge&&!charge)
            {
                charge=true;telegraphDuration=Time.time-telegraphStart;fixedDirection=boss.ChargeDirection;
                Place(playerBody,bossBody.position+fixedDirection*1.6f);
            }
            else if(boss.CurrentState==BossController.State.Charge&&charge)
            {
                if(playerHealth.Current<3)chargeHit=true;
                Place(playerBody,new Vector2(-3,4));
                locked=boss.ChargeDirection==fixedDirection;
            }
            if(boss.CurrentState==BossController.State.Recovery)recovery=true;
            yield return 0.025f;
        }
        Check(chase&&telegraph&&charge&&recovery,"IA recorre Chase, Telegraph, Charge y Recovery");
        Check(locked,"La dirección de Charge no sigue al Player al moverlo");
        Check(chargeHit,"Contacto durante Charge causa daño físico real");
        Check(telegraphDuration>=0.6f && telegraphDuration<0.85f,"Aviso de segunda fase: "+telegraphDuration.ToString("F2")+" s");
        Check(bossBody.linearVelocity.magnitude<0.1f,"Boss se detiene durante Recovery");
        yield return 1.4f;Check(boss.CurrentState==BossController.State.Chase,"Recovery devuelve a Chase");
        boss.enabled=false;bossBody.linearVelocity=Vector2.zero;Place(bossBody,new Vector2(2,1));Place(playerBody,new Vector2(-3,1));
        Keys(Key.Space);yield return 3.4f;Keys();
        Check(bossHealth.IsDead&&GameManager.Instance.IsFinished&&GameManager.Instance.Won,"Victoria mediante disparos reales hasta 0 HP");
        Check(!attack.TryFire(),"No se dispara después de finalizar");
        Check(GameObject.Find("Resultado")!=null,"Panel de victoria visible");
        ScreenCapture.CaptureScreenshot("Documentation/UnderwaterBoss-Victory.png");yield return 0.1f;
        var retry=UnityEngine.Object.FindAnyObjectByType<Button>();retry.onClick.Invoke();yield return 0.5f;Acquire();
        Check(playerHealth.Current==5&&bossHealth.Current==12&&!GameManager.Instance.IsFinished,"Botón REINTENTAR recarga vidas y partida");
        boss.enabled=false;bossBody.linearVelocity=Vector2.zero;
        for(int i=0;i<5;i++){playerHealth.TakeDamage(1);yield return 0.75f;}
        Check(playerHealth.IsDead&&GameManager.Instance.IsFinished&&!GameManager.Instance.Won,"Derrota al llegar el Player a 0 HP");
        Check(!player.GetComponent<PlayerAttack>().TryFire(),"Player muerto no puede atacar");
        Vector2 deathPosition=playerBody.position;Keys(Key.D);yield return 0.35f;Keys();
        Check(Vector2.Distance(deathPosition,playerBody.position)<0.05f,"Player muerto no puede moverse");
        ScreenCapture.CaptureScreenshot("Documentation/UnderwaterBoss-Defeat.png");yield return 0.1f;
        Keys(Key.R);yield return 0.4f;Keys();yield return 0.1f;Acquire();
        Check(playerHealth.Current==5&&bossHealth.Current==12&&!GameManager.Instance.IsFinished,"Tecla R reinicia después de derrota");
        // Verifica caducidad del proyectil con una dirección que no alcanza al jefe.
        boss.enabled=false;bossBody.linearVelocity=Vector2.zero;Place(playerBody,new Vector2(-4,0));Keys(Key.W);yield return 0.04f;Keys();
        player.GetComponent<PlayerAttack>().TryFire();yield return 2.8f;
        Check(UnityEngine.Object.FindObjectsByType<PlayerProjectile>().Length==0,"Proyectiles se eliminan por colisión o tiempo de vida");
    }
    [MenuItem("Underwater/Auditar referencias de la escena")]
    public static void Audit()
    {
        if(SceneManager.GetActiveScene().name!="UnderwaterBoss")EditorSceneManager.OpenScene("Assets/Scenes/UnderwaterBoss.unity");
        var problems=new List<string>();int objects=0,scripts=0;
        foreach(var root in SceneManager.GetActiveScene().GetRootGameObjects())
        foreach(var transform in root.GetComponentsInChildren<Transform>(true))
        {
            objects++;
            if(GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(transform.gameObject)>0)problems.Add("Missing Script: "+transform.name);
            foreach(var component in transform.GetComponents<Component>())
            {
                if(component==null)continue;if(component is MonoBehaviour)scripts++;
                var serialized=new SerializedObject(component);var property=serialized.GetIterator();
                while(property.NextVisible(true))
                    if(property.propertyType==SerializedPropertyType.ObjectReference&&property.objectReferenceValue==null&&property.objectReferenceEntityIdValue!=default)problems.Add("Missing Reference: "+transform.name+"."+property.propertyPath);
            }
        }
        File.WriteAllLines("Documentation/SceneAudit.txt",new[]{"Objetos: "+objects,"Componentes MonoBehaviour: "+scripts,"Referencias rotas: "+problems.Count}.Concat(problems));
        if(problems.Count>0)throw new Exception(string.Join("\n",problems));
    }
}
