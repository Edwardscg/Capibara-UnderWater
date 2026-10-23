using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnderwaterAdventure;

// Herramienta de creación: la escena final contiene objetos y referencias serializados.
// No se genera el nivel durante Play y no depende de este script en un ejecutable.
[InitializeOnLoad]
public static class UnderwaterLevelBuilder
{
    const string ScenePath = "Assets/Scenes/UnderwaterBoss.unity";
    const string ArtPath = "Assets/Art/Generated/";
    const string Ocean = "Assets/Art/Environment/Underwater/Ocean View Files/Layers/Day/";
    static Sprite disc, square, triangle, rock;
    static Material material;
    static Font font;
    static int arenaLayer;
    static UnderwaterLevelBuilder() { EditorApplication.update += CheckRequest; }
    static void CheckRequest()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (File.Exists("Temp/UnderwaterWork/polish.request"))
        {
            File.Delete("Temp/UnderwaterWork/polish.request");
            PolishExisting();
        }
        string request = "Temp/UnderwaterWork/build.request";
        if (!File.Exists(request)) return;
        File.Delete(request);
        try { Build(); } catch (Exception e) { Debug.LogException(e); File.WriteAllText("Temp/UnderwaterWork/build-error.txt", e.ToString()); }
    }
    [MenuItem("Underwater/Crear nivel (solo si no existe)")]
    public static void Build()
    {
        if (File.Exists(ScenePath)) { Debug.LogWarning("UnderwaterBoss ya existe. No se sobrescribió la escena."); return; }
        foreach (string path in new[] { ArtPath, "Assets/Animations/Player", "Assets/Animations/Boss", "Assets/Prefabs/Player", "Assets/Prefabs/Boss", "Assets/Prefabs/Combat", "Assets/Prefabs/Environment", "Assets/Scenes", "Assets/UI", "Documentation" }) Directory.CreateDirectory(path);
        AssetDatabase.Refresh();
        SetupLayers();
        arenaLayer = LayerMask.NameToLayer("Arena");
        material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
        AssetDatabase.CreateAsset(material, ArtPath + "UnderwaterUnlit.mat");
        disc = Shape("Disc", (x,y) => x*x+y*y <= 1);
        square = Shape("Square", (x,y) => true);
        triangle = Shape("Triangle", (x,y) => x >= -0.9f && x <= 0.95f && Mathf.Abs(y) < (0.95f-x)*0.5f);
        rock = Shape("Rock", (x,y) => x*x/0.95f + y*y/(0.77f+0.12f*Mathf.Sin(x*9)) < 1);
        font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        var oldScenes = Enumerable.Range(0, SceneManager.sceneCount).Select(SceneManager.GetSceneAt).ToArray();
        // Preserva cualquier edición previa del usuario antes de cambiar de escena.
        foreach (var old in oldScenes)
            if (old.isDirty) EditorSceneManager.SaveScene(old, "Assets/Scenes/BeforeUnderwater_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".unity", true);
        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
        SceneManager.SetActiveScene(scene);
        BuildEnvironment();
        var projectile = BuildProjectile();
        var player = BuildPlayer(projectile);
        var boss = BuildBoss();
        var playerInstance = (GameObject)PrefabUtility.InstantiatePrefab(player);
        playerInstance.transform.position = new Vector3(-6f, 0.7f, 0);
        var bossInstance = (GameObject)PrefabUtility.InstantiatePrefab(boss);
        bossInstance.transform.position = new Vector3(7f, -1f, 0);
        var ai = bossInstance.GetComponent<BossController>();
        ai.SetTarget(playerInstance.transform);
        PrefabUtility.RecordPrefabInstancePropertyModifications(ai);
        var manager = new GameObject("GameManager").AddComponent<GameManager>();
        BuildUI(manager, playerInstance.GetComponent<PlayerHealth>(), bossInstance.GetComponent<BossHealth>(), ai);
        EditorSceneManager.SaveScene(scene, ScenePath);
        foreach (var old in oldScenes) if (old.IsValid() && old.isLoaded) EditorSceneManager.CloseScene(old, true);
        var existing = EditorBuildSettings.scenes.Where(s => s.path != ScenePath).ToList();
        existing.Insert(0, new EditorBuildSettingsScene(ScenePath, true));
        EditorBuildSettings.scenes = existing.ToArray();
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = playerInstance;
        File.WriteAllText("Temp/UnderwaterWork/build-complete.txt", "UnderwaterBoss creada " + DateTime.Now);
        Debug.Log("UNDERWATER: nivel creado, prefabs y referencias guardados. Abrir UnderwaterBoss y pulsar Play.");
    }
    static void SetupLayers()
    {
        var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layers = settings.FindProperty("layers");
        if (LayerMask.NameToLayer("Arena") < 0)
            for (int i = 8; i < 32; i++) if (string.IsNullOrEmpty(layers.GetArrayElementAtIndex(i).stringValue)) { layers.GetArrayElementAtIndex(i).stringValue = "Arena"; break; }
        var sorting = settings.FindProperty("m_SortingLayers");
        int id = 51700;
        foreach (var name in new[] { "Background", "EnvironmentBack", "Characters", "EnvironmentFront", "Effects", "UI" })
        {
            bool exists = false;
            for (int i=0;i<sorting.arraySize;i++) if (sorting.GetArrayElementAtIndex(i).FindPropertyRelative("name").stringValue == name) exists = true;
            if (exists) continue;
            sorting.InsertArrayElementAtIndex(sorting.arraySize);
            var entry = sorting.GetArrayElementAtIndex(sorting.arraySize-1);
            entry.FindPropertyRelative("name").stringValue = name;
            entry.FindPropertyRelative("uniqueID").intValue = ++id;
            entry.FindPropertyRelative("locked").boolValue = false;
        }
        settings.ApplyModifiedProperties();
    }
    static Sprite Shape(string name, Func<float,float,bool> contains)
    {
        var texture = new Texture2D(64,64,TextureFormat.RGBA32,false) { name = name, filterMode = FilterMode.Point };
        var pixels = new Color[4096];
        for (int y=0;y<64;y++) for(int x=0;x<64;x++) pixels[y*64+x] = contains((x-31.5f)/32,(y-31.5f)/32) ? Color.white : Color.clear;
        texture.SetPixels(pixels); texture.Apply();
        AssetDatabase.CreateAsset(texture, ArtPath+name+"Texture.asset");
        var sprite = Sprite.Create(texture,new Rect(0,0,64,64),Vector2.one*0.5f,64);
        sprite.name = name;
        AssetDatabase.CreateAsset(sprite, ArtPath+name+".asset");
        return sprite;
    }
    static Color Hex(string value) { ColorUtility.TryParseHtmlString("#"+value,out Color c); return c; }
    static GameObject SpriteObject(string name, Transform parent, Sprite sprite, Vector2 pos, Vector2 size, Color color, string layer, int order=0)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent,false); go.transform.localPosition=pos;
        go.transform.localScale = new Vector3(size.x/sprite.bounds.size.x,size.y/sprite.bounds.size.y,1);
        var renderer = go.AddComponent<SpriteRenderer>(); renderer.sprite=sprite; renderer.color=color; renderer.sharedMaterial=material; renderer.sortingLayerName=layer; renderer.sortingOrder=order;
        return go;
    }
    static Sprite Slice(string source, string name, Rect rect)
    {
        var texture=AssetDatabase.LoadAssetAtPath<Texture2D>(Ocean+source);
        var sprite=Sprite.Create(texture,rect,Vector2.one*0.5f,16,0,SpriteMeshType.FullRect);
        sprite.name=name; AssetDatabase.CreateAsset(sprite,ArtPath+name+".asset"); return sprite;
    }
    static void BuildEnvironment()
    {
        var camera = new GameObject("Main Camera").AddComponent<Camera>();
        camera.tag="MainCamera"; camera.transform.position=new Vector3(0,0,-10); camera.orthographic=true; camera.orthographicSize=8.2f;
        camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=Hex("071D31");
        camera.gameObject.AddComponent<AudioListener>(); camera.gameObject.AddComponent<ArenaCamera>();
        var env=new GameObject("Arena · arrecife sumergido").transform;
        for(int i=0;i<24;i++) SpriteObject("Agua · profundidad "+i,env,square,new Vector2(0,-7.8f+i*0.68f),new Vector2(100,0.7f),Color.Lerp(Hex("07304A"),Hex("175469"),Mathf.Clamp01((i-7)/16f)),"Background");
        for(int i=0;i<5;i++)
        {
            var ray=SpriteObject("Rayo submarino "+i,env,triangle,new Vector2(-11+i*5.5f,3),new Vector2(12,1.7f),new Color(0.3f,0.85f,0.9f,0.035f),"Background",1);
            ray.transform.rotation=Quaternion.Euler(0,0,-65);
        }
        var vegetation=Slice("Middle.png","OceanFloor",new Rect(0,0,144,70));
        for(int i=0;i<4;i++) SpriteObject("Suelo y vegetación original "+i,env,vegetation,new Vector2(-13.5f+i*9,-5.4f),new Vector2(9,4.375f),new Color(0.65f,0.85f,0.8f),"EnvironmentBack").GetComponent<SpriteRenderer>().sharedMaterial=VegetationMaterial();
        var walls=Slice("Tile.png","OceanWall",new Rect(32,48,32,32));
        for(int side=-1;side<=1;side+=2)
        {
            for(int j=0;j<5;j++)
            {
                var wall=SpriteObject("Pared rocosa "+side+"-"+j,env,walls,new Vector2(side*13.8f,-6+j*3),new Vector2(3,3),Hex("65A5AA"),"EnvironmentBack",2);
                if(side<0) wall.GetComponent<SpriteRenderer>().flipX=true;
            }
            Boundary("Límite lateral "+side,new Vector2(side*13.6f,0),new Vector2(1.6f,15),env);
        }
        Boundary("Fondo marino",new Vector2(0,-6.8f),new Vector2(28,1.2f),env);
        Boundary("Límite superior sumergido",new Vector2(0,6.8f),new Vector2(28,1.2f),env);
        var dome=Slice("props/dome-1.png","OceanDome",new Rect(0,0,80,96));
        SpriteObject("Ruina submarina · fondo",env,dome,new Vector2(9,-3.5f),new Vector2(4,4.8f),new Color(0.38f,0.66f,0.74f,0.42f),"EnvironmentBack",-1);
        Vector2[] positions={new Vector2(-7,3.6f),new Vector2(6,3.5f),new Vector2(1,-1.7f),new Vector2(-7,-4.8f)};
        Vector2[] sizes={new Vector2(2.8f,1.3f),new Vector2(2.5f,1.4f),new Vector2(2.3f,1.55f),new Vector2(3.1f,1.3f)};
        for(int i=0;i<positions.Length;i++)
        {
            var obstacle=new GameObject("Roca sólida "+(i+1)); obstacle.layer=arenaLayer;
            SpriteObject("Silueta",obstacle.transform,rock,Vector2.zero,sizes[i],Hex("203E4D"),"EnvironmentBack",5);
            SpriteObject("Cara iluminada",obstacle.transform,rock,new Vector2(-0.1f,0.12f),sizes[i]*0.86f,Hex("426C76"),"EnvironmentBack",6);
            SpriteObject("Musgo",obstacle.transform,disc,new Vector2(-0.18f,sizes[i].y*0.27f),new Vector2(sizes[i].x*0.65f,0.2f),Hex("58A591"),"EnvironmentBack",7);
            var collider=obstacle.AddComponent<PolygonCollider2D>();
            float x=sizes[i].x*0.48f,y=sizes[i].y*0.43f;
            collider.points=new[]{new Vector2(-x*0.9f,-y*0.5f),new Vector2(-x*0.5f,-y),new Vector2(x*0.6f,-y),new Vector2(x,y*0.1f),new Vector2(x*0.55f,y),new Vector2(-x*0.5f,y)};
            var prefab=PrefabUtility.SaveAsPrefabAsset(obstacle,"Assets/Prefabs/Environment/Rock"+(i+1)+".prefab");
            UnityEngine.Object.DestroyImmediate(obstacle);
            var instance=(GameObject)PrefabUtility.InstantiatePrefab(prefab); instance.transform.SetParent(env); instance.transform.position=positions[i];
        }
        for(int i=0;i<12;i++)
        {
            float x=-11.8f+i*2.1f;
            for(int branch=0;branch<3;branch++)
            {
                var seaweed=SpriteObject("Alga decorativa",env,disc,new Vector2(x+branch*0.14f,-5.7f+branch*0.12f),new Vector2(0.14f,0.7f+branch*0.35f),i%3==0?Hex("C77D93"):Hex("3D998D"),"EnvironmentFront");
                seaweed.transform.rotation=Quaternion.Euler(0,0,(branch-1)*20);
            }
        }
        CreateBubbles(env);
    }
    [MenuItem("Underwater/Ajustar presentación de la escena")]
    public static void PolishExisting()
    {
        if(SceneManager.GetActiveScene().name!="UnderwaterBoss" || EditorApplication.isPlaying) return;
        material=AssetDatabase.LoadAssetAtPath<Material>(ArtPath+"UnderwaterUnlit.mat");
        var env=GameObject.Find("Arena · arrecife sumergido").transform;
        foreach(var renderer in env.GetComponentsInChildren<SpriteRenderer>())
        {
            if(renderer.name.StartsWith("Agua · profundidad "))
            {
                int i=int.Parse(renderer.name.Substring("Agua · profundidad ".Length));
                renderer.color=Color.Lerp(Hex("07304A"),Hex("175469"),Mathf.Clamp01((i-7)/16f));
                renderer.transform.localScale=new Vector3(100,0.7f,1);
            }
            if(renderer.name.StartsWith("Suelo y vegetación"))
            {
                renderer.color=new Color(0.65f,0.85f,0.8f);
                renderer.sharedMaterial=VegetationMaterial();
            }
            if(renderer.name.StartsWith("Pared rocosa")) UnityEngine.Object.DestroyImmediate(renderer.gameObject);
        }
        var walls=AssetDatabase.LoadAssetAtPath<Sprite>(ArtPath+"OceanWallContinuous.asset");
        if(walls==null)walls=Slice("Tile.png","OceanWallContinuous",new Rect(32,48,32,32));
        for(int side=-1;side<=1;side+=2)for(int j=0;j<5;j++)
        {
            var wall=SpriteObject("Pared rocosa "+side+"-"+j,env,walls,new Vector2(side*13.8f,-6+j*3),new Vector2(3,3),Hex("65A5AA"),"EnvironmentBack",2);
            if(side<0)wall.GetComponent<SpriteRenderer>().flipX=true;
        }
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
        Debug.Log("UNDERWATER: presentación ajustada sin alterar física o combate.");
    }
    static Material VegetationMaterial()
    {
        string path=ArtPath+"OceanVegetation.mat";
        var mat=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(mat==null){mat=new Material(Shader.Find("Underwater/OceanVegetation"));AssetDatabase.CreateAsset(mat,path);}
        return mat;
    }
    static void Boundary(string name,Vector2 pos,Vector2 size,Transform parent)
    {
        var go=new GameObject(name); go.transform.SetParent(parent);go.transform.position=pos;go.layer=arenaLayer;go.AddComponent<BoxCollider2D>().size=size;
    }
    static void CreateBubbles(Transform parent)
    {
        var go=new GameObject("Burbujas ambientales");go.transform.SetParent(parent);go.transform.position=new Vector3(0,-6,0);
        var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        var main=ps.main; main.startLifetime=18; main.startSpeed=0.65f; main.startSize=new ParticleSystem.MinMaxCurve(0.035f,0.12f);main.startColor=new Color(0.6f,0.95f,1,0.3f);main.maxParticles=130;main.simulationSpace=ParticleSystemSimulationSpace.World;
        var emission=ps.emission;emission.rateOverTime=5;
        var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Box;shape.scale=new Vector3(24,0.1f,0.1f);shape.rotation=new Vector3(-90,0,0);
        var renderer=go.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=ParticleMaterial();renderer.sortingLayerName="Effects";
    }
    static Material ParticleMaterial()
    {
        string path=ArtPath+"BubbleMaterial.mat";
        var existing=AssetDatabase.LoadAssetAtPath<Material>(path);if(existing!=null)return existing;
        var mat=new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        mat.SetTexture("_BaseMap",disc.texture);mat.SetFloat("_Surface",1);mat.SetFloat("_Blend",0);mat.SetFloat("_SrcBlend",5);mat.SetFloat("_DstBlend",10);mat.SetFloat("_ZWrite",0);mat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");mat.renderQueue=3000;
        AssetDatabase.CreateAsset(mat,path);return mat;
    }
    static PlayerProjectile BuildProjectile()
    {
        var go=new GameObject("PlayerProjectile");
        SpriteObject("Arpón",go.transform,square,Vector2.zero,new Vector2(0.65f,0.055f),Hex("DEF9EC"),"Effects",2);
        SpriteObject("Punta",go.transform,triangle,new Vector2(0.32f,0),new Vector2(0.24f,0.19f),Hex("84F6E1"),"Effects",3);
        var body=go.AddComponent<Rigidbody2D>();body.gravityScale=0;body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;body.constraints=RigidbodyConstraints2D.FreezeRotation;
        var collider=go.AddComponent<BoxCollider2D>();collider.size=new Vector2(0.7f,0.16f);collider.isTrigger=true;
        go.AddComponent<PlayerProjectile>();
        var prefab=PrefabUtility.SaveAsPrefabAsset(go,"Assets/Prefabs/Combat/PlayerProjectile.prefab");UnityEngine.Object.DestroyImmediate(go);return prefab.GetComponent<PlayerProjectile>();
    }
    static Transform Part(Transform parent,string name,Sprite sprite,float x,float y,float w,float h,string color,int order)
        => SpriteObject(name,parent,sprite,new Vector2(x,y),new Vector2(w,h),Hex(color),"Characters",order).transform;
    static GameObject BuildPlayer(PlayerProjectile projectile)
    {
        var go=new GameObject("Player");go.tag="Player";
        var body=go.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Dynamic;body.gravityScale=0.16f;body.linearDamping=3;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.interpolation=RigidbodyInterpolation2D.Interpolate;body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
        var capsule=go.AddComponent<CapsuleCollider2D>();capsule.size=new Vector2(1.35f,0.95f);capsule.direction=CapsuleDirection2D.Horizontal;
        var facing=new GameObject("Visual").transform;facing.SetParent(go.transform,false);
        var pose=new GameObject("Pose").transform;pose.SetParent(facing,false);
        // Placeholder construido con formas nativas: reemplazable sin cambiar la física.
        Part(pose,"Mochila",disc,-0.45f,0.26f,0.58f,0.85f,"5B4741",0);
        Part(pose,"Cuerpo contorno",disc,-0.12f,0,1.3f,0.92f,"493731",1);
        Part(pose,"Cuerpo",disc,-0.12f,0.04f,1.2f,0.82f,"AD7C4C",2);
        Part(pose,"Vientre",disc,0.01f,-0.13f,0.82f,0.39f,"D9B880",3);
        Part(pose,"Pata trasera",disc,-0.48f,-0.34f,0.4f,0.22f,"8C603E",4).localRotation=Quaternion.Euler(0,0,20);
        Part(pose,"Pata delantera",disc,0.28f,-0.3f,0.44f,0.2f,"9D6D43",4).localRotation=Quaternion.Euler(0,0,-25);
        Part(pose,"Cabeza contorno",disc,0.45f,0.21f,0.87f,0.7f,"493731",5);
        Part(pose,"Cabeza",disc,0.47f,0.22f,0.79f,0.62f,"C79660",6);
        Part(pose,"Hocico ancho",disc,0.76f,0.12f,0.41f,0.43f,"C79660",7);
        Part(pose,"Oreja",disc,0.2f,0.5f,0.25f,0.29f,"A57249",8);
        Part(pose,"Oreja interior",disc,0.22f,0.51f,0.13f,0.16f,"674534",9);
        Part(pose,"Pañuelo",square,0.2f,0.39f,0.6f,0.12f,"3EAFAF",10);
        Part(pose,"Nudo pañuelo",triangle,-0.21f,0.35f,0.4f,0.21f,"3EAFAF",9);
        Part(pose,"Ojo",disc,0.55f,0.3f,0.1f,0.12f,"202F3E",11);
        Part(pose,"Brillo ojo",disc,0.57f,0.33f,0.028f,0.03f,"FFFFFF",12);
        Part(pose,"Nariz",disc,0.89f,0.19f,0.075f,0.075f,"513F34",11);
        Part(pose,"Arpón equipado",square,0.64f,-0.25f,0.8f,0.04f,"DBE5D2",12);
        Part(pose,"Punta arpón",triangle,1.04f,-0.25f,0.2f,0.16f,"78E5D7",13);
        var animator=go.AddComponent<Animator>();animator.runtimeAnimatorController=MakeController(false);
        var visual=go.AddComponent<ActorVisual>();visual.Configure(facing,animator);
        go.AddComponent<PlayerHealth>().Configure(visual);
        go.AddComponent<PlayerController>().Configure(visual);
        var muzzle=new GameObject("FirePoint").transform;muzzle.SetParent(go.transform,false);muzzle.localPosition=Vector3.right;
        go.AddComponent<PlayerAttack>().Configure(projectile,muzzle,visual);
        var prefab=PrefabUtility.SaveAsPrefabAsset(go,"Assets/Prefabs/Player/Player.prefab");UnityEngine.Object.DestroyImmediate(go);return prefab;
    }
    static GameObject BuildBoss()
    {
        var go=new GameObject("CaimanBoss");
        var body=go.AddComponent<Rigidbody2D>();body.bodyType=RigidbodyType2D.Dynamic;body.gravityScale=0;body.mass=3;body.constraints=RigidbodyConstraints2D.FreezeRotation;body.interpolation=RigidbodyInterpolation2D.Interpolate;body.collisionDetectionMode=CollisionDetectionMode2D.Continuous;
        var capsule=go.AddComponent<CapsuleCollider2D>();capsule.direction=CapsuleDirection2D.Horizontal;capsule.size=new Vector2(2.7f,0.85f);capsule.offset=new Vector2(0.22f,0);
        var facing=new GameObject("Visual").transform;facing.SetParent(go.transform,false);
        var pose=new GameObject("Pose").transform;pose.SetParent(facing,false);
        Part(pose,"Cola",triangle,-1.23f,0.05f,1.8f,0.7f,"4B7560",1).localRotation=Quaternion.Euler(0,0,180);
        Part(pose,"Silueta",disc,-0.16f,0,2.5f,1.0f,"172E30",2);
        Part(pose,"Lomo",disc,-0.16f,0.05f,2.35f,0.87f,"507A5B",3);
        Part(pose,"Vientre",disc,0.0f,-0.19f,1.95f,0.33f,"9DAC6C",4);
        Part(pose,"Cabeza",disc,0.9f,0.15f,1.2f,0.63f,"638950",5);
        Part(pose,"Mandíbula",disc,1.35f,-0.03f,1.12f,0.27f,"B6B780",6);
        Part(pose,"Hocico",disc,1.35f,0.13f,1.2f,0.28f,"668A53",7);
        Part(pose,"Boca",square,1.37f,0.015f,0.91f,0.045f,"1C3735",8);
        for(int i=0;i<6;i++) Part(pose,"Diente "+i,triangle,1.02f+i*0.14f,-0.035f,0.11f,0.1f,"F0E6BB",9).localRotation=Quaternion.Euler(0,0,-90);
        Part(pose,"Ojo elevado",disc,0.82f,0.4f,0.3f,0.21f,"476C42",9);
        Part(pose,"Ojo amarillo",disc,0.86f,0.42f,0.14f,0.1f,"F5D66F",10);
        Part(pose,"Pupila",square,0.88f,0.42f,0.027f,0.09f,"142C2D",11);
        for(int i=0;i<7;i++) Part(pose,"Escama "+i,triangle,-1.05f+i*0.26f,0.46f,0.22f,0.17f,"345542",5).localRotation=Quaternion.Euler(0,0,90);
        Part(pose,"Pata trasera",disc,-0.73f,-0.35f,0.62f,0.25f,"3E654D",10).localRotation=Quaternion.Euler(0,0,35);
        Part(pose,"Pata delantera",disc,0.48f,-0.36f,0.58f,0.25f,"3E654D",10).localRotation=Quaternion.Euler(0,0,45);
        var animator=go.AddComponent<Animator>();animator.runtimeAnimatorController=MakeController(true);
        var visual=go.AddComponent<ActorVisual>();visual.Configure(facing,animator);
        go.AddComponent<BossHealth>().Configure(visual);go.AddComponent<BossController>().Configure(null,visual);
        var prefab=PrefabUtility.SaveAsPrefabAsset(go,"Assets/Prefabs/Boss/CaimanBoss.prefab");UnityEngine.Object.DestroyImmediate(go);return prefab;
    }
    static AnimatorController MakeController(bool boss)
    {
        string dir=boss?"Assets/Animations/Boss/":"Assets/Animations/Player/";
        string prefix=boss?"Caiman_":"Player_";
        var controller=AnimatorController.CreateAnimatorControllerAtPath(dir+prefix+"Controller.controller");
        string[] states=boss?new[]{"Idle","Swim","Telegraph","Charge","Hurt","Death"}:new[]{"Idle","Swim","Attack","Hurt","Death"};
        foreach(string state in states)
        {
            var clip=new AnimationClip { name=prefix+state, frameRate=12 };
            bool loop=state=="Idle"||state=="Swim"||state=="Telegraph"||state=="Charge";
            float duration=state=="Idle"?1.2f:state=="Death"?0.7f:0.35f;
            float angle=state=="Death"?150:state=="Telegraph"?4:state=="Hurt"?-8:state=="Swim"?3:0;
            clip.SetCurve("Visual/Pose",typeof(Transform),"localEulerAnglesRaw.z",new AnimationCurve(new Keyframe(0,0),new Keyframe(duration*0.5f,angle),new Keyframe(duration,state=="Death"?150:0)));
            clip.SetCurve("Visual/Pose",typeof(Transform),"m_LocalPosition.y",new AnimationCurve(new Keyframe(0,0),new Keyframe(duration*0.5f,state=="Idle"?0.04f:0),new Keyframe(duration,0)));
            var settings=AnimationUtility.GetAnimationClipSettings(clip);settings.loopTime=loop;AnimationUtility.SetAnimationClipSettings(clip,settings);
            AssetDatabase.CreateAsset(clip,dir+prefix+state+".anim");
            var animState=controller.layers[0].stateMachine.AddState(state);animState.motion=clip;
            if(state=="Idle")controller.layers[0].stateMachine.defaultState=animState;
        }
        return controller;
    }
    static RectTransform Rect(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size)
    {
        var go=new GameObject(name,typeof(RectTransform));var rt=go.GetComponent<RectTransform>();rt.SetParent(parent,false);rt.anchorMin=rt.anchorMax=anchor;rt.anchoredPosition=pos;rt.sizeDelta=size;return rt;
    }
    static Text Label(string name,Transform parent,string text,int size,Vector2 pos,Vector2 dimensions,Color color,TextAnchor alignment=TextAnchor.MiddleCenter)
    {
        var rt=Rect(name,parent,new Vector2(0.5f,0.5f),pos,dimensions);var label=rt.gameObject.AddComponent<Text>();label.font=font;label.text=text;label.fontSize=size;label.color=color;label.alignment=alignment;label.raycastTarget=false;return label;
    }
    static Image Panel(string name,Transform parent,Vector2 anchor,Vector2 pos,Vector2 size,Color color)
    { var rt=Rect(name,parent,anchor,pos,size);var image=rt.gameObject.AddComponent<Image>();image.color=color;image.raycastTarget=false;return image; }
    static void BuildUI(GameManager manager,PlayerHealth player,BossHealth boss,BossController ai)
    {
        var canvas=new GameObject("Canvas",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));canvas.GetComponent<Canvas>().renderMode=RenderMode.ScreenSpaceOverlay;
        var scaler=canvas.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,900);scaler.matchWidthOrHeight=0.5f;
        var events=new GameObject("EventSystem",typeof(EventSystem),typeof(InputSystemUIInputModule));events.GetComponent<InputSystemUIInputModule>().AssignDefaultActions();
        var header=Panel("HUD superior",canvas.transform,new Vector2(0.5f,1),new Vector2(0,-65),new Vector2(1560,114),new Color(0.02f,0.08f,0.13f,0.9f));
        Label("Expedición",header.transform,"CAPIBARA / EXPEDICIÓN 01",18,new Vector2(-572,29),new Vector2(365,25),Hex("8FBCBF"),TextAnchor.MiddleLeft);
        MakeBar(header.transform,player,null,new Vector2(-592,-6),new Vector2(320,20),Hex("78E4CB"));
        Label("Nombre jefe",header.transform,"CAIMÁN",26,new Vector2(230,29),new Vector2(740,30),Hex("F1E8D2"));
        MakeBar(header.transform,null,boss,new Vector2(230,-6),new Vector2(840,20),Hex("ECAB6F"));
        var status=Label("Estado del jefe",header.transform,"FASE 1 / MANTÉN LA DISTANCIA",15,new Vector2(230,-38),new Vector2(840,20),Hex("B4CED0"));
        var footer=Panel("Ayuda",canvas.transform,new Vector2(0.5f,0),new Vector2(0,31),new Vector2(1150,42),new Color(0.02f,0.08f,0.13f,0.82f));
        Label("Controles",footer.transform,"WASD / FLECHAS  ·  NADAR Y APUNTAR       ESPACIO  ·  DISPARAR       ESQUIVA EL AVISO ROJO",16,Vector2.zero,new Vector2(1120,35),Hex("B4D7D5"));
        var result=Panel("Resultado",canvas.transform,Vector2.one*0.5f,Vector2.zero,new Vector2(740,340),new Color(0.025f,0.09f,0.14f,0.97f));result.raycastTarget=true;
        var title=Label("Título",result.transform,"¡VICTORIA!",48,new Vector2(0,102),new Vector2(700,70),Hex("78E4CB"));
        var detail=Label("Detalle",result.transform,"",21,new Vector2(0,16),new Vector2(665,92),Hex("BCD2D2"));
        var retry=Panel("Reintentar",result.transform,Vector2.one*0.5f,new Vector2(0,-95),new Vector2(280,58),Hex("56C2B2"));retry.raycastTarget=true;
        var button=retry.gameObject.AddComponent<Button>();button.targetGraphic=retry;
        Label("Texto botón",retry.transform,"REINTENTAR  [R]",20,Vector2.zero,new Vector2(280,58),Hex("072D38"));
        UnityEventTools.AddPersistentListener(button.onClick,manager.Restart);
        var impact=BuildImpact();
        manager.Configure(player,boss,ai,result.gameObject,title,detail,status,impact);
        result.gameObject.SetActive(false);
    }
    static void MakeBar(Transform parent,PlayerHealth p,BossHealth b,Vector2 position,Vector2 size,Color color)
    {
        var background=Panel(p!=null?"Vida Player":"Vida Boss",parent,Vector2.one*0.5f,position,size,Hex("172F3D"));
        var fill=Panel("Relleno",background.transform,Vector2.one*0.5f,Vector2.zero,size-Vector2.one*4,color);fill.sprite=square;fill.type=Image.Type.Filled;fill.fillMethod=Image.FillMethod.Horizontal;fill.fillOrigin=0;
        var label=Label("Valor",background.transform,"",14,Vector2.zero,size,Hex("FFFFFF"));background.gameObject.AddComponent<HealthBar>().Configure(p,b,fill,label);
    }
    static ParticleSystem BuildImpact()
    {
        var go=new GameObject("ProjectileImpact");var ps=go.AddComponent<ParticleSystem>();ps.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
        var main=ps.main;main.loop=false;main.duration=0.3f;main.startLifetime=0.45f;main.startSpeed=1.2f;main.startSize=0.065f;main.startColor=Hex("A8F4E5");main.maxParticles=12;
        var emission=ps.emission;emission.rateOverTime=0;emission.SetBursts(new[]{new ParticleSystem.Burst(0,8)});
        var shape=ps.shape;shape.shapeType=ParticleSystemShapeType.Circle;shape.radius=0.1f;
        var renderer=go.GetComponent<ParticleSystemRenderer>();renderer.sharedMaterial=ParticleMaterial();renderer.sortingLayerName="Effects";
        var prefab=PrefabUtility.SaveAsPrefabAsset(go,"Assets/Prefabs/Combat/ProjectileImpact.prefab");UnityEngine.Object.DestroyImmediate(go);return prefab.GetComponent<ParticleSystem>();
    }
}
