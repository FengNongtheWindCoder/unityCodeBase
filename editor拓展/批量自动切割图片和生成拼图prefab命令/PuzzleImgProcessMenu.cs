/*********************************************************************************
 *Copyright(C) 2015 by FengNongtheWindCoder
 *All rights reserved.
 *FileName:     PuzzleImgProcessMenu.cs
 *Author:       FengNongtheWindCoder
 *Version:      1.0
 *UnityVersion：5.3.0f4
 *Date:         2015-12-26
 *Description:  提供图片切割函数sliceImgByRowColumn，可以利用这些函数进行几行几列的切割
                提供将切分后的图片组合生成prefab的函数generateImgAllPiecesPrefab，生成的prefab记录碎片原来的位置。 
                提供三个菜单项，可将图片切割为4*4等分，并生成prefab，或者单独进行这两步，菜单支持多选。
				如有问题请在 https://git.coding.net/better-start-now/unityCodeBase.git 讨论区留言。
				或 https://github.com/FengNongtheWindCoder/unityCodeBase
 *History:  
**********************************************************************************/
using UnityEngine;
using UnityEditor;
using System.IO;

public class PuzzleImgProcessMenu   {
    /// <summary>
    /// 将图片切割成4*4，同时生成prefab，多选时进行Texture2D过滤。
    /// </summary>
    [MenuItem("Custom Tools/Slice and Generate prefab/4*4")]
    static void SliceAndGen4by4()
    {
        int column = 4;
        int row = 4;
        foreach (var item in Selection.GetFiltered(typeof(Texture2D), SelectionMode.TopLevel))
        {
            //获取导入的图片 
            Texture2D image = item as Texture2D;
            //切割
            sliceImgByRowColumn(image, row, column);
            //生成prefab
            generateImgAllPiecesPrefab(image);
        }
    }
    [MenuItem("Custom Tools/Slice and Generate prefab/4*4",true)]
    static bool canSliceAndGen() {
        return Selection.activeObject is Texture2D;
    }
    /// <summary>
    /// 将图片切割成4*4，多选时进行Texture2D过滤。
    /// </summary>
    [MenuItem("Custom Tools/Slice Image/4*4")]
    static void SliceImg4by4()
    {
        int column = 4;
        int row = 4;
        foreach (var item in Selection.GetFiltered(typeof(Texture2D), SelectionMode.TopLevel))
        {
            //获取导入的图片 
            Texture2D image = item as Texture2D;
            //切割
            sliceImgByRowColumn(image, row, column);
        }
    }
    [MenuItem("Custom Tools/Slice Image/4*4", true)]
    static bool canSlice()
    {
        return Selection.activeObject is Texture2D;
    }
    /// <summary>
    /// 用图片的sprite生成prefab，多选时进行Texture2D过滤。
    /// </summary>
    [MenuItem("Custom Tools/Generate all pieces prefab")]
    static void GenerateAllPiecesPrefab()
    {
        foreach (var item in Selection.GetFiltered(typeof(Texture2D), SelectionMode.TopLevel))
        {
            //获取导入的图片 
            Texture2D image = item as Texture2D;
            //利用所有碎片组成一个prefab，带有位置信息
            generateImgAllPiecesPrefab(image);
        }
    }
    [MenuItem("Custom Tools/Generate all pieces prefab", true)]
    static bool canGenerate()
    {
        return Selection.activeObject is Texture2D;
    }

    /// <summary>
    /// Slices the img by row column.
    /// </summary>
    /// <param name="image">The image.</param>
    /// <param name="row">The row.</param>
    /// <param name="column">The column.</param>
    static void sliceImgByRowColumn(Texture2D image, int row, int column)
    {
        if (row <= 0 || column <= 0)
        {
            Debug.Log("row and column must have a num > 0");
            return;
        }
        if (row == 1 && column == 1)
        {
            return;
        }
        //获取导入的图片，及importer
        string path = AssetDatabase.GetAssetPath(image);
        var teximporter = AssetImporter.GetAtPath(path) as TextureImporter;
        //修改importer的参数，设置为sprite，multiple
        teximporter.textureType = TextureImporterType.Sprite;
        teximporter.spriteImportMode = SpriteImportMode.Multiple;
        //图片在游戏内显示为定长的，这里设置为图片宽度pixel/显示长度unit，假设图是方的
        teximporter.spritePixelsPerUnit = image.width / column;
        //计算出每个sprite所占的位置，由左下的xy和width，height表示
        float sliceWidth = image.width / column; //每个rect块的宽和高
        float sliceHeight = image.height / row;
        SpriteMetaData[] slicedata = new SpriteMetaData[row * column];
        for (int i = 0; i < slicedata.Length; i++)
        {
            slicedata[i] = new SpriteMetaData();
            slicedata[i].alignment = (int)SpriteAlignment.Center;//获取的精灵pivot设置为中心
            slicedata[i].name = image.name + '_' + i;
            //获取当前i代表图中的几行几列
            int currentRow = i / column;
            int currentColumn = i % column;
            //这里是左下为原点，从左往右，从下往上进行分割,编辑器自动切割时从左往右，从上往下的顺序。也就名字顺序不一样。
            slicedata[i].rect = new Rect(currentColumn * sliceWidth, currentRow * sliceHeight, sliceWidth, sliceHeight);
        }
        //保存对importer的修改，并重新导入资源
        teximporter.spritesheet = slicedata;
        teximporter.SaveAndReimport();
    }
    /// <summary>
    /// 利用碎片的rect，计算出在场景中相对于父节点的坐标，最终组合在一起形成原图prefab
    /// </summary>
    /// <param name="image">The image.</param>
    static void generateImgAllPiecesPrefab(Texture2D image)
    {
        string path = AssetDatabase.GetAssetPath(image);
        string dirPath = Path.GetDirectoryName(path) + '/';
        var teximporter = AssetImporter.GetAtPath(path) as TextureImporter;
        //生成父节点
        GameObject allpieces = new GameObject();
        allpieces.name = image.name + "_allpieces";
        allpieces.transform.position = Vector3.zero;
        //将所有sprite添加到父节点，并设置其坐标，因为相对父节点坐标，这里的设置会使父节点在左下角作为原点。
        foreach (var item in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (!(item is Sprite))
            {
                continue;
            }
            Sprite sprite = item as Sprite;
            GameObject piecesobject = new GameObject();
            piecesobject.name = sprite.name;
            SpriteRenderer sRender = piecesobject.AddComponent<SpriteRenderer>();
            sRender.sprite = sprite;
            piecesobject.transform.position = new Vector2(sprite.rect.center.x / teximporter.spritePixelsPerUnit, sprite.rect.center.y / teximporter.spritePixelsPerUnit);
            piecesobject.transform.SetParent(allpieces.transform);
        }
        //将gameobject保存到图片同文件夹
        string prefabname = dirPath + allpieces.name + ".prefab";
        PrefabUtility.CreatePrefab(prefabname, allpieces);
        Object.DestroyImmediate(allpieces);
    }
}
