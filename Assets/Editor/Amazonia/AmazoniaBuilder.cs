using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Amazonia;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class AmazoniaBuilder {
 const string Art="Assets/Art/Amazonia/Originals/", Anim="Assets/Animations/Amazonia/", Prefabs="Assets/Prefabs/Amazonia/";
 static Material material;static Sprite square,beam;static AdventureBalance balance;
 static List<string> manifest=new List<string>();
 static AmazoniaBuilder(){EditorApplication.update+=Tick;}
 static void Tick(){if(EditorApplication.isCompiling||EditorApplication.isUpdating||EditorApplication.isPlayingOrWillChangePlaymode)return;if(File.Exists("Temp/Amazonia/open.request")){File.Delete("Temp/Amazonia/open.request");EditorSceneManager.OpenScene("Assets/Scenes/AmazoniaTerrestre.unity");return;}if(File.Exists("Temp/Amazonia/refresh.request")){File.Delete("Temp/Amazonia/refresh.request");AssetDatabase.Refresh();return;}if(!File.Exists("Temp/Amazonia/build.request"))return;File.Delete("Temp/Amazonia/build.request");try{Build();File.WriteAllText("Temp/Amazonia/built.txt",DateTime.Now.ToString("s"));}catch(Exception e){File.WriteAllText("Temp/Amazonia/error.txt",e.ToString());Debug.LogException(e);}}
 [MenuItem("Amazonia/Construir aventura conectada")]
 public static void Build(){
  var active=SceneManager.GetActiveScene();if(active.isDirty)EditorSceneManager.SaveScene(active,"Assets/Scenes/BeforeAmazonia_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".unity",true);
  material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Generated/UnderwaterUnlit.mat");
  if(!material)throw new Exception("Falta material Unlit existente");
  balance=AssetDatabase.LoadAssetAtPath<AdventureBalance>("Assets/Prefabs/Amazonia/Balance.asset");if(!balance){balance=ScriptableObject.CreateInstance<AdventureBalance>();AssetDatabase.CreateAsset(balance,Prefabs+"Balance.asset");}
  // Geometría auxiliar: sprite blanco nativo, sin modificar los recursos entregados.
  string squarePath="Assets/Art/Amazonia/Solid.asset";square=AssetDatabase.LoadAssetAtPath<Sprite>(squarePath);
  if(!square){var tex=new Texture2D(2,2);tex.SetPixels(new[]{Color.white,Color.white,Color.white,Color.white});tex.Apply();tex.filterMode=FilterMode.Point;AssetDatabase.CreateAsset(tex,"Assets/Art/Amazonia/SolidTexture.asset");square=Sprite.Create(tex,new Rect(0,0,2,2),Vector2.one*.5f,2);AssetDatabase.CreateAsset(square,squarePath);}
  foreach(string name in new[]{"CapybaraTerrestre","CapybaraAcuatico","CaimanTerrestre","CaimanAcuatico","FondoTerrestre","FondoAcuatico"}){var importer=(TextureImporter)AssetImporter.GetAtPath(Art+name+".png");importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.spritePixelsPerUnit=100;importer.filterMode=FilterMode.Point;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=4096;importer.alphaIsTransparency=true;importer.SaveAndReimport();}
  beam=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Amazonia/Beam.asset");if(!beam){var tex=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"FondoTerrestre.png");beam=Sprite.Create(tex,new Rect(530,724-328-30,150,30),Vector2.one*.5f,100,0,SpriteMeshType.FullRect);AssetDatabase.CreateAsset(beam,"Assets/Art/Amazonia/Beam.asset");}manifest.Clear();
  var pLand=Controller("CapybaraTerrestre",false,false);var pWater=Controller("CapybaraAcuatico",false,true);var bLand=Controller("CaimanTerrestre",true,false);var bWater=Controller("CaimanAcuatico",true,true);
  var plasma=Projectile(false);var rock=Projectile(true);
  var pl=Player(false,pLand,plasma);var pw=Player(true,pWater,plasma);var bl=Boss(false,bLand,rock);var bw=Boss(true,bWater,rock);
  Scene(false,pl,bl);Scene(true,pw,bw);
  var existing=EditorBuildSettings.scenes.Where(s=>!s.path.EndsWith("AmazoniaTerrestre.unity")&&!s.path.EndsWith("AmazoniaAcuatica.unity"));EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene("Assets/Scenes/AmazoniaTerrestre.unity",true),new EditorBuildSettingsScene("Assets/Scenes/AmazoniaAcuatica.unity",true)}.Concat(existing).ToArray();
  if(manifest.Count>0)File.WriteAllLines("Documentation/Amazonia/Recortes.tsv",new[]{"Sheet\tEstado\tFrame\tx\tyDesdeArriba\tancho\talto\tpivoteX\tpivoteY"}.Concat(manifest));AssetDatabase.SaveAssets();EditorSceneManager.OpenScene("Assets/Scenes/AmazoniaTerrestre.unity");Debug.Log("AMAZONIA: construcción completa");
 }
 static AnimatorController Controller(string sheet,bool boss,bool water){
  string dir=Anim+sheet;Directory.CreateDirectory(dir);AssetDatabase.Refresh();string path=dir+"/Controller.controller";
  var controller=AssetDatabase.LoadAssetAtPath<AnimatorController>(path);if(controller)return controller;
  controller=AnimatorController.CreateAnimatorControllerAtPath(path);
  if(!boss&&!water){
   Row(controller,sheet,"Idle",0,120,new[]{30,147,260,378,492},65,.5f,.5f,true);
   Row(controller,sheet,"Run",126,114,new[]{0,150,270,390,510},65,.55f,.5f,true);
   Row(controller,sheet,"Jump",248,128,new[]{20,155,295,435,575},65,.52f,.5f,false);
   Row(controller,sheet,"Fall",248,128,new[]{575,710},65,.5f,.5f,true);
   Row(controller,sheet,"Land",248,128,new[]{830,975},65,.5f,.5f,false);
   Row(controller,sheet,"Melee",384,119,new[]{0,180,330,500,670},65,.4f,.5f,false,11);
   Row(controller,sheet,"Ranged",507,115,new[]{0,150,310,470},65,.38f,.5f,false,12);
   Row(controller,sheet,"Switch",632,105,new[]{0,145,290},65,.5f,.5f,false,12);
   Row(controller,sheet,"Dash",745,82,new[]{155,335,475,650},65,.68f,.5f,true,14);
   Row(controller,sheet,"Hurt",922,80,new[]{0,175,345,510},65,.52f,.5f,false,12);
   Row(controller,sheet,"Death",1007,78,new[]{335,505,680,850,1020},65,.5f,.5f,false,7);
  }else if(!boss){
   Row(controller,sheet,"Idle",0,135,new[]{0,185,350,520,690,850},75,.57f,.5f,true);
   Row(controller,sheet,"Swim",140,99,new[]{0,185,365,545,725},75,.67f,.5f,true);
   Row(controller,sheet,"Down",242,137,new[]{0,210,415,625,835},75,.57f,.5f,true);
   Row(controller,sheet,"Up",381,143,new[]{0,200,400,595,795},75,.57f,.5f,true);
   Row(controller,sheet,"Ranged",634,114,new[]{0,175,340,510},75,.46f,.5f,false,12);
   Row(controller,sheet,"Hurt",858,110,new[]{0,175,340,490},75,.54f,.5f,false,12);
   Row(controller,sheet,"Death",979,107,new[]{360,510,655,820,970},75,.5f,.5f,false,7);
  }else if(!water){
   Row(controller,sheet,"Idle",15,112,new[]{0,210,415,610},70,.5f,.5f,true);
   Row(controller,sheet,"Run",147,101,new[]{0,240,475,715,950},70,.45f,.5f,true);
   Row(controller,sheet,"Telegraph",265,131,new[]{0,245,460},70,.5f,.5f,false,4);
   Row(controller,sheet,"Claw",265,131,new[]{245,460,745},70,.5f,.5f,false,6);
   Row(controller,sheet,"Push",415,112,new[]{0,270},70,.5f,.5f,true);
   Row(controller,sheet,"RockPrepare",417,106,new[]{270,540},70,.5f,.5f,false);
   Row(controller,sheet,"Rock",540,155,new[]{0,200,385,570},70,.5f,.4f,false,6);
   Row(controller,sheet,"Recovery",710,115,new[]{1005,1230,1448},70,.5f,.5f,true);
   Row(controller,sheet,"Hurt",710,115,new[]{0,205,395,570},70,.5f,.5f,false);
   Row(controller,sheet,"Death",980,102,new[]{0,245,520,810},70,.5f,.5f,false,6);
  }else{
   Row(controller,sheet,"Idle",35,105,new[]{0,185,365,545,725},70,.48f,.5f,true);
   Row(controller,sheet,"Swim",178,96,new[]{0,185,365,545,725},70,.48f,.5f,true);
   Row(controller,sheet,"Telegraph",310,124,new[]{0,200,390,575,765},70,.48f,.5f,false,6);
   Row(controller,sheet,"Charge",470,107,new[]{0,305,655,1000,1448},70,.35f,.5f,true,10);
   Row(controller,sheet,"Recovery",609,87,new[]{0,245,485,705,935},70,.48f,.5f,true,7);
   Row(controller,sheet,"Hurt",734,93,new[]{0,245,475,705,945},70,.48f,.5f,false);
   Row(controller,sheet,"Enraged",862,91,new[]{0,245,475,710,950},70,.48f,.5f,true);
   Row(controller,sheet,"Death",988,94,new[]{0,230,440,650,850,1040,1230,1448},70,.48f,.5f,false,7);
  }
  return controller;
 }
 static void Row(AnimatorController controller,string sheet,string state,int y,int height,int[] xs,float ppu,float px,float py,bool loop,float fps=8){
  var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+sheet+".png");string dir=Anim+sheet;var frames=new ObjectReferenceKeyframe[xs.Length];
  for(int i=0;i<xs.Length-1;i++){int x=xs[i]+2,w=xs[i+1]-xs[i]-4;var sp=Sprite.Create(texture,new Rect(x,texture.height-y-height,w,height),new Vector2(px,py),ppu,0,SpriteMeshType.FullRect);sp.name=state+"_"+i;AssetDatabase.CreateAsset(sp,dir+"/"+state+"_"+i+".asset");frames[i]=new ObjectReferenceKeyframe{time=i/fps,value=sp};manifest.Add($"{sheet}\t{state}\t{i}\t{x}\t{y}\t{w}\t{height}\t{px}\t{py}");}
  frames[frames.Length-1]=new ObjectReferenceKeyframe{time=(xs.Length-1)/fps,value=frames[loop?0:frames.Length-2].value};
  var clip=new AnimationClip{name=state,frameRate=fps};AnimationUtility.SetObjectReferenceCurve(clip,new EditorCurveBinding{path="Visual",type=typeof(SpriteRenderer),propertyName="m_Sprite"},frames);var settings=AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=loop;AnimationUtility.SetAnimationClipSettings(clip,settings);AssetDatabase.CreateAsset(clip,dir+"/"+state+".anim");var s=controller.layers[0].stateMachine.AddState(state);s.motion=clip;if(state=="Idle")controller.layers[0].stateMachine.defaultState=s;
 }
 static GameObject Obj(string name,Transform parent=null){var g=new GameObject(name);if(parent)g.transform.SetParent(parent,false);return g;}
 static SpriteRenderer Draw(string name,Transform parent,Vector2 pos,Vector2 size,Color color,string layer="EnvironmentBack",int order=0,Sprite sprite=null){var g=Obj(name,parent);g.transform.localPosition=pos;var r=g.AddComponent<SpriteRenderer>();r.sprite=sprite?sprite:square;r.sharedMaterial=material;r.color=color;r.sortingLayerName=layer;r.sortingOrder=order;g.transform.localScale=new Vector3(size.x/r.sprite.bounds.size.x,size.y/r.sprite.bounds.size.y,1);return r;}
 static Rigidbody2D Body(GameObject g,float gravity){var b=g.AddComponent<Rigidbody2D>();b.gravityScale=gravity;b.constraints=RigidbodyConstraints2D.FreezeRotation;b.collisionDetectionMode=CollisionDetectionMode2D.Continuous;b.interpolation=RigidbodyInterpolation2D.Interpolate;return b;}
 static AdventureVisual Visual(GameObject g,AnimatorController controller,bool left){var a=g.AddComponent<Animator>();a.runtimeAnimatorController=controller;var r=Obj("Visual",g.transform).AddComponent<SpriteRenderer>();r.sharedMaterial=material;r.sortingLayerName="Characters";
  // La primera imagen se obtiene del clip Idle, no de una ruta deducida.
  var idle=controller.animationClips.First(c=>c.name=="Idle");r.sprite=(Sprite)AnimationUtility.GetObjectReferenceCurve(idle,AnimationUtility.GetObjectReferenceCurveBindings(idle)[0])[0].value;
  var v=g.AddComponent<AdventureVisual>();v.animator=a;v.sprite=r;v.facesLeft=left;return v;}
 static AdventureProjectile Projectile(bool rock){var g=Obj(rock?"Piedra":"Plasma");Body(g,0);var c=g.AddComponent<CircleCollider2D>();c.radius=rock?.25f:.14f;c.isTrigger=true;Draw("Núcleo",g.transform,Vector2.zero,rock?new Vector2(.48f,.4f):new Vector2(.65f,.2f),rock?new Color(.45f,.38f,.22f):Color.cyan,"Effects",3);Draw("Brillo",g.transform,Vector2.zero,rock?new Vector2(.24f,.2f):new Vector2(.4f,.07f),rock?new Color(.67f,.6f,.37f):Color.white,"Effects",4);var p=g.AddComponent<AdventureProjectile>();p.lifetime=rock?4:3;var result=PrefabUtility.SaveAsPrefabAsset(g,Prefabs+(rock?"Piedra":"Plasma")+".prefab").GetComponent<AdventureProjectile>();UnityEngine.Object.DestroyImmediate(g);return result;}
 static GameObject Player(bool water,AnimatorController controller,AdventureProjectile shot){var g=Obj(water?"Capibara acuático":"Capibara terrestre");Body(g,water?.12f:2.8f);var c=g.AddComponent<CapsuleCollider2D>();c.size=new Vector2(1.2f,1.55f);var v=Visual(g,controller,false);var h=g.AddComponent<CombatHealth>();h.maximum=balance.playerHP;h.visual=v;var p=g.AddComponent<AdventurePlayer>();p.aquatic=water;p.balance=balance;p.visual=v;p.plasmaPrefab=shot;p.muzzle=Obj("FirePoint",g.transform).transform;
  p.swordGlow=Draw("Espada equipada",g.transform,new Vector2(.7f,-.1f),new Vector2(.09f,.65f),Color.cyan,"Characters",3).gameObject;p.gunGlow=Draw("Cañón equipado",g.transform,new Vector2(.6f,-.15f),new Vector2(.35f,.13f),new Color(.15f,.65f,.7f),"Characters",3).gameObject;
  var result=PrefabUtility.SaveAsPrefabAsset(g,Prefabs+(water?"CapibaraAcuatico":"CapibaraTerrestre")+".prefab");UnityEngine.Object.DestroyImmediate(g);return result;}
 static GameObject Boss(bool water,AnimatorController controller,AdventureProjectile rock){var g=Obj(water?"Caimán acuático":"Caimán terrestre");Body(g,water?0:2.8f).mass=3;var c=g.AddComponent<CapsuleCollider2D>();c.direction=CapsuleDirection2D.Horizontal;c.size=new Vector2(2.55f,1.2f);var v=Visual(g,controller,true);var h=g.AddComponent<CombatHealth>();h.maximum=water?balance.waterBossHP:balance.landBossHP;h.invulnerability=.08f;h.visual=v;var ai=g.AddComponent<AdventureBoss>();ai.aquatic=water;ai.balance=balance;ai.visual=v;ai.rockPrefab=rock;var result=PrefabUtility.SaveAsPrefabAsset(g,Prefabs+(water?"CaimanAcuatico":"CaimanTerrestre")+".prefab");UnityEngine.Object.DestroyImmediate(g);return result;}
 static void Block(Transform root,string name,Vector2 position,Vector2 size,bool visible=true){var g=Obj(name,root);g.layer=LayerMask.NameToLayer("Arena");g.transform.position=position;var collider=g.AddComponent<BoxCollider2D>();collider.size=size;if(size.y<1){collider.usedByEffector=true;var effector=g.AddComponent<PlatformEffector2D>();effector.useOneWay=true;effector.surfaceArc=150;}if(visible&&size.y<1){Draw("Tronco transitable",g.transform,Vector2.zero,size,Color.white,"EnvironmentBack",2,beam);return;}if(visible&&size.y>=1&&name!="Suelo"){UnityEngine.Object.DestroyImmediate(collider);var polygon=g.AddComponent<PolygonCollider2D>();polygon.points=new[]{new Vector2(-size.x*.5f,-size.y*.25f),new Vector2(-size.x*.32f,size.y*.4f),new Vector2(size.x*.25f,size.y*.5f),new Vector2(size.x*.5f,0),new Vector2(size.x*.3f,-size.y*.45f),new Vector2(-size.x*.3f,-size.y*.45f)};var rockSprite=AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Generated/Rock.asset");Draw("Roca",g.transform,Vector2.zero,size,new Color(.13f,.2f,.12f),"EnvironmentBack",0,rockSprite);Draw("Cara iluminada",g.transform,new Vector2(0,.12f),size*.78f,new Color(.32f,.34f,.16f),"EnvironmentBack",1,rockSprite);Draw("Musgo",g.transform,new Vector2(-size.x*.1f,size.y*.3f),new Vector2(size.x*.6f,size.y*.25f),new Color(.39f,.48f,.17f),"EnvironmentBack",2,rockSprite);return;}if(visible){Draw("Contorno",g.transform,Vector2.zero,size,new Color(.06f,.12f,.08f));Draw("Tierra",g.transform,new Vector2(0,.03f),size-new Vector2(.1f,.1f),new Color(.28f,.24f,.12f),"EnvironmentBack",1);Draw("Musgo · superficie sólida",g.transform,new Vector2(0,size.y*.5f),new Vector2(size.x,.12f),new Color(.46f,.57f,.2f),"EnvironmentBack",2);for(float x=-size.x*.5f+.3f;x<size.x*.5f;x+=.6f)Draw("Textura",g.transform,new Vector2(x,0),new Vector2(.3f,.06f),new Color(.4f,.34f,.17f),"EnvironmentBack",2);}}
 static void Scene(bool water,GameObject playerPrefab,GameObject bossPrefab){
  EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
  var root=Obj(water?"Lago amazónico":"Selva y orilla");
  var bg=AssetDatabase.LoadAssetAtPath<Sprite>(Art+(water?"FondoAcuatico":"FondoTerrestre")+".png");Draw("Panorama original",root.transform,new Vector2(33,water?9:5.8f),new Vector2(66,22),Color.white,"Background",-20,bg);
  Block(root.transform,"Suelo",new Vector2(33,-1.1f),new Vector2(66,2.2f),water);Block(root.transform,"Límite izquierdo",new Vector2(-.5f,9),new Vector2(1,25),false);Block(root.transform,"Límite derecho",new Vector2(66.5f,9),new Vector2(1,25),false);Block(root.transform,"Techo",new Vector2(33,20.5f),new Vector2(66,1),false);
  if(water){Block(root.transform,"Raíz sumergida",new Vector2(13,3.6f),new Vector2(4,1));Block(root.transform,"Ruina alta",new Vector2(23,13),new Vector2(4,1.4f));Block(root.transform,"Roca inferior",new Vector2(34,3),new Vector2(3.2f,1.3f));Block(root.transform,"Tronco hundido",new Vector2(46,12),new Vector2(5,1));Block(root.transform,"Ruina lateral",new Vector2(59,5),new Vector2(3,1.3f));}
  else {Block(root.transform,"Tronco inicial",new Vector2(10,2),new Vector2(5,.5f));Block(root.transform,"Raíz elevada",new Vector2(17,3.9f),new Vector2(4,.6f));Block(root.transform,"Plataforma de combate A",new Vector2(32,3.1f),new Vector2(5,.5f));Block(root.transform,"Plataforma de combate B",new Vector2(43,3.2f),new Vector2(5,.5f));Block(root.transform,"Muelle",new Vector2(60,-.05f),new Vector2(12,.3f));}
  var p=((GameObject)PrefabUtility.InstantiatePrefab(playerPrefab)).GetComponent<AdventurePlayer>();p.transform.position=new Vector3(4,water?8:1.2f,0);
  var b=((GameObject)PrefabUtility.InstantiatePrefab(bossPrefab)).GetComponent<AdventureBoss>();b.transform.position=new Vector3(water?30:38,water?8:1,0);b.target=p;
  var camera=Obj("Main Camera");camera.tag="MainCamera";camera.transform.position=new Vector3(11,water?8:4.5f,-10);var cam=camera.AddComponent<Camera>();cam.orthographic=true;cam.orthographicSize=5.8f;cam.backgroundColor=new Color(.02f,.08f,.09f);cam.clearFlags=CameraClearFlags.SolidColor;camera.AddComponent<AudioListener>();var follow=camera.AddComponent<AdventureCamera>();follow.target=p.transform;follow.boss=b;follow.aquatic=water;follow.limits=new Rect(0,water?0:-1.3f,66,water?20:21.3f);
  var flow=Obj("Aventura · progreso y transición").AddComponent<AdventureFlow>();flow.player=p;flow.boss=b;flow.impact=AssetDatabase.LoadAssetAtPath<ParticleSystem>("Assets/Prefabs/Combat/ProjectileImpact.prefab");
  UI(flow,water);
  if(water){var bubbles=Obj("Burbujas ambientales").AddComponent<ParticleSystem>();bubbles.transform.position=new Vector3(33,0,0);var main=bubbles.main;main.startLifetime=14;main.startSpeed=.8f;main.startSize=.075f;main.startColor=new Color(.5f,1,.8f,.5f);main.maxParticles=160;main.simulationSpace=ParticleSystemSimulationSpace.World;var emission=bubbles.emission;emission.rateOverTime=7;var shape=bubbles.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(64,.2f,.1f);var velocity=bubbles.velocityOverLifetime;velocity.enabled=true;velocity.y=new ParticleSystem.MinMaxCurve(.7f);var renderer=bubbles.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=material;renderer.sortingLayerName="Effects";}
  EditorSceneManager.SaveScene(SceneManager.GetActiveScene(),"Assets/Scenes/"+(water?"AmazoniaAcuatica":"AmazoniaTerrestre")+".unity");
 }
 static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size){var g=new GameObject(name,typeof(RectTransform));var r=g.GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=anchor;r.pivot=new Vector2(.5f,.5f);r.anchoredPosition=pos;r.sizeDelta=size;return r;}
 static Text Label(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size,string text,int font=22){var r=Rect(name,parent,anchor,pos,size);var t=r.gameObject.AddComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=font;t.alignment=TextAnchor.MiddleCenter;t.color=new Color(.88f,1,.88f);t.text=text;t.raycastTarget=false;return t;}
 static Image Bar(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size,Color color){var back=Rect(name,parent,anchor,pos,size).gameObject.AddComponent<Image>();back.color=new Color(.015f,.04f,.04f,.9f);var r=Rect("Vida",back.transform,Vector2.one*.5f,Vector2.zero,size-new Vector2(6,6));var fill=r.gameObject.AddComponent<Image>();fill.sprite=square;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;fill.color=color;return fill;}
 static void UI(AdventureFlow flow,bool water){var g=Obj("HUD · Amazonía");var c=g.AddComponent<Canvas>();c.renderMode=RenderMode.ScreenSpaceOverlay;var scaler=g.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,900);scaler.matchWidthOrHeight=.5f;g.AddComponent<GraphicRaycaster>();
  flow.healthLabel=Label("Capibara",g.transform,new Vector2(0,1),new Vector2(170,-31),new Vector2(300,40),"CAPIBARA");flow.healthFill=Bar("Salud",g.transform,new Vector2(0,1),new Vector2(170,-65),new Vector2(280,19),new Color(.3f,.9f,.55f));
  flow.bossLabel=Label("Caimán",g.transform,new Vector2(.5f,1),new Vector2(0,-28),new Vector2(600,35),"CAIMÁN");flow.bossFill=Bar("Salud caimán",g.transform,new Vector2(.5f,1),new Vector2(0,-62),new Vector2(540,19),new Color(.94f,.42f,.2f));
  flow.status=Label("Aviso",g.transform,new Vector2(.5f,1),new Vector2(0,-103),new Vector2(1000,42),"AMAZONÍA PERUANA",21);
  flow.weaponLabel=Label("Arma",g.transform,new Vector2(.5f,0),new Vector2(0,68),new Vector2(1000,35),"",19);
  flow.controls=Label("Controles",g.transform,new Vector2(.5f,0),new Vector2(0,29),new Vector2(1500,40),water?"WASD / FLECHAS  NADAR     J / CLIC  PLASMA     ESC  PAUSA":"A D / FLECHAS  MOVER    ESPACIO  SALTAR    J / CLIC  ATACAR    Q  ARMA    SHIFT  DASH    E  LAGO    ESC  PAUSA",18);
  var panel=Rect("Resultado y pausa",g.transform,Vector2.one*.5f,Vector2.zero,new Vector2(800,330)).gameObject;panel.AddComponent<Image>().color=new Color(.02f,.08f,.08f,.96f);flow.resultPanel=panel;flow.resultTitle=Label("Título",panel.transform,Vector2.one*.5f,new Vector2(0,95),new Vector2(750,70),"",48);flow.resultDetail=Label("Detalle",panel.transform,Vector2.one*.5f,new Vector2(0,7),new Vector2(740,95),"",23);
  var button=Rect("REINTENTAR",panel.transform,Vector2.one*.5f,new Vector2(0,-100),new Vector2(330,55)).gameObject;button.AddComponent<Image>().color=new Color(.16f,.4f,.3f);var bt=button.AddComponent<Button>();UnityEditor.Events.UnityEventTools.AddPersistentListener(bt.onClick,flow.Restart);Label("Texto",button.transform,Vector2.one*.5f,Vector2.zero,new Vector2(320,50),"REINTENTAR",24);panel.SetActive(false);
  var fade=Rect("Fundido",g.transform,Vector2.one*.5f,Vector2.zero,new Vector2(4000,3000)).gameObject.AddComponent<Image>();fade.color=new Color(0,.025f,.03f,0);fade.raycastTarget=false;flow.fade=fade;fade.gameObject.SetActive(false);
  var events=Obj("EventSystem");events.AddComponent<EventSystem>();events.AddComponent<InputSystemUIInputModule>().AssignDefaultActions();
 }
}





