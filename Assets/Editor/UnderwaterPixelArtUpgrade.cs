using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Migración explícita, ejecutada una sola vez mediante archivo de solicitud o menú.
// Conserva el arte anterior desactivado y los PNG generados originales.
[InitializeOnLoad]
public static class UnderwaterPixelArtUpgrade
{
    const string Capy = "Assets/Art/Characters/Capybara/PixelArt/Capybara.png";
    const string Caiman = "Assets/Art/Characters/Caiman/PixelArt/Caiman.png";
    const string Background = "Assets/Art/Environment/Underwater/PixelRuins/UnderwaterRuins.png";
    static UnderwaterPixelArtUpgrade() { EditorApplication.update += Tick; }
    static void Tick()
    {
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        const string request = "Temp/UnderwaterWork/pixel-art.request";
        if (!File.Exists(request)) return;
        File.Delete(request);
        try { Upgrade(); }
        catch (Exception exception) { File.WriteAllText("Temp/UnderwaterWork/pixel-art-error.txt", exception.ToString()); Debug.LogException(exception); }
    }
    static Sprite Import(string path, bool character)
    {
        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100;
        importer.filterMode = FilterMode.Point;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.mipmapEnabled = false;
        importer.alphaIsTransparency = character;
        importer.npotScale = TextureImporterNPOTScale.None;
        importer.maxTextureSize = 4096;
        importer.isReadable = character;
        importer.SaveAndReimport();
        if (!character) return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
        // Recorte de metadatos por alfa: no modifica ni vuelve a guardar la imagen.
        var pixels = texture.GetPixels32();
        int minX=texture.width,minY=texture.height,maxX=0,maxY=0,clear=0;
        for(int y=0;y<texture.height;y++) for(int x=0;x<texture.width;x++)
        {
            byte alpha=pixels[y*texture.width+x].a;
            if(alpha==0)clear++;
            if(alpha<200)continue;
            minX=Mathf.Min(minX,x);minY=Mathf.Min(minY,y);maxX=Mathf.Max(maxX,x);maxY=Mathf.Max(maxY,y);
        }
        if(clear==0 || minX>=maxX)throw new Exception("Sprite sin transparencia válida: "+path);
        string spritePath=path.Replace(".png","_Cropped.asset");
        var existing=AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
        if(existing!=null)return existing;
        var sprite=Sprite.Create(texture,new Rect(minX,minY,maxX-minX+1,maxY-minY+1),Vector2.one*0.5f,100,0,SpriteMeshType.FullRect);
        sprite.name=Path.GetFileNameWithoutExtension(path)+"_PixelArt";
        AssetDatabase.CreateAsset(sprite,spritePath);
        return sprite;
    }
    [MenuItem("Underwater/Aplicar arte pixel aprobado")]
    public static void Upgrade()
    {
        if(EditorApplication.isPlaying)return;
        var active=SceneManager.GetActiveScene();
        if(active.isDirty) EditorSceneManager.SaveScene(active,"Assets/Scenes/BeforePixelArt_"+DateTime.Now.ToString("yyyyMMdd_HHmmss")+".unity",true);
        if(active.name!="UnderwaterBoss")EditorSceneManager.OpenScene("Assets/Scenes/UnderwaterBoss.unity");
        Sprite capy=Import(Capy,true),caiman=Import(Caiman,true),background=Import(Background,false);
        var material=AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Generated/UnderwaterUnlit.mat");
        UpdateActor("Assets/Prefabs/Player/Player.prefab",capy,material,false);
        UpdateActor("Assets/Prefabs/Boss/CaimanBoss.prefab",caiman,material,true);
        var env=GameObject.Find("Arena · arrecife sumergido").transform;
        foreach(var sr in env.GetComponentsInChildren<SpriteRenderer>(true))
        {
            if(sr.GetComponentInParent<PolygonCollider2D>()!=null)
            {
                sr.color=sr.name=="Musgo"?new Color(0.38f,0.5f,0.16f):sr.name=="Cara iluminada"?new Color(0.37f,0.34f,0.19f):new Color(0.13f,0.2f,0.15f);
            }
            else sr.enabled=false;
        }
        var root=GameObject.Find("Ruinas submarinas · Pixel Art");
        if(root==null)root=new GameObject("Ruinas submarinas · Pixel Art");
        var renderer=root.GetComponent<SpriteRenderer>();if(renderer==null)renderer=root.AddComponent<SpriteRenderer>();
        renderer.sprite=background;renderer.sharedMaterial=material;renderer.sortingLayerName="Background";renderer.sortingOrder=-20;
        root.transform.position=Vector3.zero;
        root.transform.localScale=new Vector3(29.2f/background.bounds.size.x,16.425f/background.bounds.size.y,1);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        EditorSceneManager.SaveScene(SceneManager.GetActiveScene());AssetDatabase.SaveAssets();
        File.WriteAllText("Temp/UnderwaterWork/pixel-art-complete.txt","Arte aplicado; ambos sprites tienen 3.4 unidades de ancho. "+DateTime.Now);
        Debug.Log("PIXEL ART: fondo y personajes integrados; ancho visual de ambos = 3.4; originales conservados.");
    }
    static void UpdateActor(string path,Sprite sprite,Material material,bool boss)
    {
        var root=PrefabUtility.LoadPrefabContents(path);
        try
        {
            var pose=root.transform.Find("Visual/Pose");
            foreach(Transform child in pose)child.gameObject.SetActive(false);
            var graphic=pose.Find("PixelArt");
            if(graphic==null){graphic=new GameObject("PixelArt").transform;graphic.SetParent(pose,false);}
            graphic.gameObject.SetActive(true);
            var renderer=graphic.GetComponent<SpriteRenderer>();if(renderer==null)renderer=graphic.gameObject.AddComponent<SpriteRenderer>();
            renderer.sprite=sprite;renderer.sharedMaterial=material;renderer.color=Color.white;renderer.sortingLayerName="Characters";renderer.sortingOrder=10;
            graphic.localPosition=Vector3.zero;graphic.localRotation=Quaternion.identity;
            graphic.localScale=Vector3.one*(3.4f/sprite.bounds.size.x);
            var collider=root.GetComponent<CapsuleCollider2D>();
            collider.size=boss?new Vector2(2.8f,0.85f):new Vector2(2.5f,1.45f);
            collider.offset=boss?new Vector2(0.2f,0):new Vector2(-0.25f,0);
            PrefabUtility.SaveAsPrefabAsset(root,path);
        }
        finally {PrefabUtility.UnloadPrefabContents(root);}
    }
}
