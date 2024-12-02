using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using System;
using UnityEditor;

public class OptimizeTest : Singletons<OptimizeTest>
{
    public TMPro.TextMeshProUGUI displayName;
    public TMPro.TextMeshProUGUI textureDataTx;
    public Image textureImg;

    public SkeletonAnimation skeletonAnimation;
    public string path;
    public SpineSkinModelSO spineSkinModelSO;
    public GameObject prefab;
    public GameObject characterParent;

    [Header("SelectChar")]
    public int index = 0;
    public SpineSkinModelSO currentSkinModelSO;
    public SpineSkinModelSO[] spineSkinModelSOs;

    void Start()
    {
        index = 0;
        currentSkinModelSO = spineSkinModelSOs[index];
        CreateHero(currentSkinModelSO);
    }

    void Update()
    {
    }

    public void CreateHero(SpineSkinModelSO _spineSkinModelSO)
    {
        UiController.Instance.DestorySlot(characterParent);
        GameObject charPrefab = Instantiate(_spineSkinModelSO.prefab);
        charPrefab.transform.SetParent(characterParent.transform);
        charPrefab.transform.localPosition = new Vector3(0, 0f);
        SpineEntitySkin spineEntitySkin = charPrefab.GetComponent<SpineEntitySkin>();
        spineEntitySkin.ChangeSkin(_spineSkinModelSO);
        displayName.text = spineEntitySkin.spineSkinModelSO.displayName;
        textureDataTx.text = spineEntitySkin.GetTextureData();
        textureImg.sprite = spineEntitySkin.GetTexture();
        textureImg.preserveAspect = true;
    }

    public void NextCharButton()
    {
        index += 1;
        if (index > spineSkinModelSOs.Length - 1) index = 0;
        if (currentSkinModelSO == spineSkinModelSOs[index]) return;

        currentSkinModelSO = spineSkinModelSOs[index];
        CreateHero(currentSkinModelSO);
    }

    public void BackCharButton()
    {
        index -= 1;
        if (index < 0) index = spineSkinModelSOs.Length - 1;
        if (currentSkinModelSO == spineSkinModelSOs[index]) return;

        currentSkinModelSO = spineSkinModelSOs[index];
        CreateHero(currentSkinModelSO);
    }
    /*
    public void SetImporter()
    {
        // หาค่า path ของ mainTexture
        string path = AssetDatabase.GetAssetPath(skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial.mainTexture);

        // ดึง TextureImporter ของ mainTexture
        TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer != null)
        {
            // ตั้งค่าการตั้งค่าสำหรับ WebGL
            importer.SetPlatformTextureSettings(new TextureImporterPlatformSettings
            {
                name = "WebGL",
                overridden = true,
                maxTextureSize = 512,
                format = TextureImporterFormat.RGBA32,
                resizeAlgorithm = TextureResizeAlgorithm.Mitchell
            });

            // บันทึกการเปลี่ยนแปลง
            //AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);

            Debug.Log("WebGL Texture settings applied to: " + path);
        }
    }
    */
}
