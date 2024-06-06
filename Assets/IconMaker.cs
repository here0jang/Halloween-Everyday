//using UnityEditor;
//using UnityEngine;
//using UnityEngine.UI;
////using static System.Net.Mime.MediaTypeNames;
//using System.IO;
//using System.Threading;
//using static UnityEditor.Progress;

//public class IconMaker : MonoBehaviour
//{
//    public ItemDatabase mItemBody;
//    public ItemDatabase mItemEyebrow;
//    public ItemDatabase mItemGlasses;
//    public ItemDatabase mItemMustache;

//    public ItemDatabase mItemFullBody;
//    public ItemDatabase mItemPants;
//    public ItemDatabase mItemShoe;
//    public ItemDatabase mItemGlove;
//    public ItemDatabase mItemBackpack;
//    public ItemDatabase mItemOuterwear;


//    string meshPath = "Assets/Characters_7/Meshes";
//    string prefabPath = "Assets/Characters_7/Prefabs";
//    string itemSpritePath = "Assets/Characters_7/ItemSprites";

 



//    private void Start()
//    {
//        //int i = TryGetUnityObjectsOfTypeFromPath<Sprite>("Assets/Characters_7/ItemSprites/Body", mItemBody);
//        //int j = TryGetUnityObjectsOfTypeFromPath<Sprite>("Assets/Characters_7/ItemSprites/Eyebrow", mItemEyebrow);
//        //int k = TryGetUnityObjectsOfTypeFromPath<Sprite>("Assets/Characters_7/ItemSprites/Glasses", mItemGlasses);
//        //int l = TryGetUnityObjectsOfTypeFromPath<Sprite>("Assets/Characters_7/ItemSprites/Mustache", mItemMustache);
//        //int i = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Body", "Assets/Characters_7/Prefabs/Body", "Assets/Characters_7/ItemSprites/Body", mItemBody);
//        //int j = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Eyebrow", "Assets/Characters_7/Prefabs/Eyebrow", "Assets/Characters_7/ItemSprites/Eyebrow", mItemEyebrow);
//        //int k = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Glasses", "Assets/Characters_7/Prefabs/Glasses", "Assets/Characters_7/ItemSprites/Glasses", mItemGlasses);
//        //int l = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Mustache", "Assets/Characters_7/Prefabs/Mustache", "Assets/Characters_7/ItemSprites/Mustache", mItemMustache);
//        //int a = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/FullBody", "Assets/Characters_7/Prefabs/FullBody", mItemFullBody);
//        //int b = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Pants", "Assets/Characters_7/Prefabs/Pants", mItemPants);
//        //int c = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Shoe", "Assets/Characters_7/Prefabs/Shoe", mItemShoe);
//        //int d = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Glove", "Assets/Characters_7/Prefabs/Glove", mItemGlove);
//        //int e = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>("Assets/Characters_7/Meshes/Backpack", "Assets/Characters_7/Prefabs/Backpack", mItemBackpack);
//        //int f = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Pants", prefabPath+"/Pants", itemSpritePath + "/Pants", mItemPants);
//        //int f = TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Outerwear", prefabPath+ "/Outerwear", itemSpritePath + "/Outerwear", mItemOuterwear);
//        //TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Mustache", prefabPath+ "/Mustache", itemSpritePath + "/Mustache", mItemMustache);
//        //TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Eyebrow", prefabPath+ "/Eyebrow", itemSpritePath + "/Eyebrow", mItemEyebrow);
//        //TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Body", prefabPath+ "/Body", itemSpritePath + "/Body", mItemBody);
//        TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/FullBody", prefabPath+ "/FullBody", itemSpritePath + "/FullBody", mItemFullBody);
//        TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Pants", prefabPath+ "/Pants", itemSpritePath + "/Pants", mItemPants);
//        TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Shoe", prefabPath+ "/Shoe", itemSpritePath + "/Shoe", mItemShoe);
//        TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Glove", prefabPath+ "/Glove", itemSpritePath + "/Glove", mItemGlove);
//        TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Backpack", prefabPath + "/Backpack", itemSpritePath + "/Backpack", mItemBackpack);
//        TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Outerwear", prefabPath + "/Outerwear", itemSpritePath + "/Outerwear", mItemOuterwear);
//        //TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Mustache", prefabPath+ "/Mustache", itemSpritePath + "/Mustache", mItemMustache);
//        //TryGetUnityObjectsOfTypeFromPath<UnityEngine.Object>(meshPath + "/Mustache", prefabPath+ "/Mustache", itemSpritePath + "/Mustache", mItemMustache);

//        //int a = TryGetUnityObjectsOfTypeFromPath<Sprite>($"{itemSpritePath}/Backpack", mItemBackpack);
//        //int b = TryGetUnityObjectsOfTypeFromPath<Sprite>($"{itemSpritePath}/Outerwear", mItemOuterwear);
//        //int c = TryGetUnityObjectsOfTypeFromPath<Sprite>($"{itemSpritePath}/Glove", mItemGlove);
//        //int d = TryGetUnityObjectsOfTypeFromPath<Sprite>($"{itemSpritePath}/Shoe", mItemShoe);
//        //int e = TryGetUnityObjectsOfTypeFromPath<Sprite>($"{itemSpritePath}/Pants", mItemPants);
//        //int f = TryGetUnityObjectsOfTypeFromPath<Sprite>($"{itemSpritePath}/FullBody", mItemFullBody);

//        //CreateIcon("Body", mItemBody);
//        //CreateIcon("Eyebrow", mItemEyebrow);
//        //CreateIcon("Glasses", mItemGlasses);
//        //CreateIcon("Mustache", mItemMustache);

//        //CreateIcon("Backpack", mItemBackpack);
//        //CreateIcon("Outerwear", mItemOuterwear);
//        //CreateIcon("Glove", mItemGlove);
//        //CreateIcon("Shoe", mItemShoe);
//        //CreateIcon("Pants", mItemPants);
//        //CreateIcon("FullBody", mItemFullBody);
//    }

//    private void CreateIcon(string fileName, ItemDatabase items)
//    {
//        for (int i = 0; i < items.mDatas.Count; i++)
//        {
//            Texture2D test = null;
//            while (test == null)
//            {
//                test = AssetPreview.GetAssetPreview(items.mDatas[i].mPrefab);
//                Thread.Sleep(80);
//            }
//            Sprite sp = Sprite.Create(test, new Rect(0, 0, test.width, test.height), new Vector2(0.5f, 0.5f));
//            byte[] bytes = ImageConversion.EncodeToPNG(sp.texture);
//            string dirPath = UnityEngine.Application.dataPath + $"/../{fileName}/{items.mDatas[i].mPrefab.name}.png";
//            File.WriteAllBytes(dirPath, bytes);
//        }

//        Debug.Log("Finished : " + fileName);
//    }

//    public int TryGetUnityObjectsOfTypeFromPath<T>(string meshPath, string prefabPath,string spritePath, ItemDatabase item) where T : UnityEngine.Object
//    {
//        string[] filePaths = System.IO.Directory.GetFiles(meshPath);
//        string[] filePaths2 = System.IO.Directory.GetFiles(prefabPath);
//        string[] filePaths3 = System.IO.Directory.GetFiles(spritePath);

//        int countFound = 0;

//        Debug.Log(filePaths.Length);

//        if (filePaths != null && filePaths.Length > 0)
//        {
//            for (int i = 0; i < filePaths.Length; i++)
//            {
//                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(Mesh));
//                UnityEngine.Object obj2 = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths2[i], typeof(GameObject));
//                UnityEngine.Object obj3 = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths3[i], typeof(Sprite));
//                if (obj is Mesh asset && obj2 is GameObject asset2 && obj3 is Sprite asset3)
//                {
//                    countFound++;
//                    item.mDatas.Add(new Data { mMesh = asset, mPrefab = asset2, mSprite = asset3});
//                }
//            }
//        }

//        EditorUtility.SetDirty(item);

//        return countFound;
//    }

//    public int TryGetUnityObjectsOfTypeFromPath<T>(string path, string path2, ItemDatabase item) where T : UnityEngine.Object
//    {
//        string[] filePaths = System.IO.Directory.GetFiles(path);
//        string[] filePaths2 = System.IO.Directory.GetFiles(path2);

//        int countFound = 0;

//        Debug.Log(filePaths.Length);

//        if (filePaths != null && filePaths.Length > 0)
//        {
//            for (int i = 0; i < filePaths.Length; i++)
//            {
//                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(Mesh));
//                UnityEngine.Object obj2 = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(GameObject));
//                if (obj is Mesh asset && obj2 is GameObject asset2)
//                {
//                    countFound++;
//                    item.mDatas.Add(new Data { mMesh = asset, mPrefab = asset2 });
//                }
//            }
//        }

//        return countFound;
//    }

//    public int TryGetUnityObjectsOfTypeFromPath<T>(string path, ItemDatabase item) where T : UnityEngine.Object
//    {
//        string[] filePaths = System.IO.Directory.GetFiles(path);

//        int countFound = 0;

//        Debug.Log(filePaths.Length);

//        if (filePaths != null && filePaths.Length > 0)
//        {
//            int j = 0;
//            for (int i = 0; i < filePaths.Length; i++)
//            {
//                UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(filePaths[i], typeof(Sprite));
//                if (obj is Sprite asset && i % 2 == 0)
//                {
//                    countFound++;
//                    //mItem.mDatas.Add(new Data { mMesh = asset });
//                    item.mDatas[j].mSprite = asset;
//                    j++;
//                }
//            }
//        }

//        return countFound;
//    }
//}
