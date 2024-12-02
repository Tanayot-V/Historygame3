using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEditor;

[System.Serializable]
public class EntitySkin
{
    public string id;
    public string spineSkin_id_Path;
}

public class SpineEntitySkin : MonoBehaviour
{
    public SpineSkinModelSO spineSkinModelSO;

    SkeletonAnimation skeletonAnimation;
    Skin characterSkin;

    // for repacking the skin to a new atlas texture
    public Material runtimeMaterial;
    public Texture2D runtimeAtlas;

    private string textuerData;
    private Texture2D texture;

    private void Awake()
    {
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();
    }
    void Start()
    {
        //ChangeSkinFormPath("hero/Costume/hero_B");
        ChangeSkin(spineSkinModelSO);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Debug.Log("UpdateCharacterSkin");
            //ChangeSkin(spineSkinModelSO);
            //Debug.Log(skeletonAnimation.initialSkinName);
        }
    }

    public void ChangeSkinFormPath(string _path)
    {
        UpdateCharacterSkin();
        UpdateCombinedSkinFormPath(_path);
        OptimizeSkin();
    }

    public void ChangeSkin(SpineSkinModelSO _spineSkinModelSO)
    {
        spineSkinModelSO = _spineSkinModelSO;
        ReloadSkeletonDataAsset();
        UpdateCharacterSkin();
        UpdateCombinedSkin();
        OptimizeSkin();
    }

    void UpdateCharacterSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        characterSkin = new Skin("character-base");
    }

    void AddEquipmentSkinsTo(Skin combinedSkin)
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        /*
        spineSkinModelSO.mainPath.ToList().ForEach(o => {
            string _path = o + spineSkinModelSO.nameID[index];
            Debug.Log("Path:" +_path);
            if (!string.IsNullOrEmpty(_path)) combinedSkin.AddSkin(skeletonData.FindSkin(_path));
            index += 1;
        });
        /*
        if (!string.IsNullOrEmpty(head_Target)) combinedSkin.AddSkin(skeletonData.FindSkin(head_Target));
        if (!string.IsNullOrEmpty(weapon_Target)) combinedSkin.AddSkin(skeletonData.FindSkin(weapon_Target));
        if (!string.IsNullOrEmpty(body_Target)) combinedSkin.AddSkin(skeletonData.FindSkin(body_Target));
        if (!string.IsNullOrEmpty(hat_Target)) combinedSkin.AddSkin(skeletonData.FindSkin(hat_Target));
        if (!string.IsNullOrEmpty(costume_Target)) combinedSkin.AddSkin(skeletonData.FindSkin(costume_Target));*/
    }

    void UpdateCombinedSkinFormPath(string _path)
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        Skin resultCombinedSkin = new Skin("character-combined");

        resultCombinedSkin.AddSkin(characterSkin);
        AddEquipmentSkinsToPath(resultCombinedSkin);

        skeleton.SetSkin(resultCombinedSkin);
        skeleton.SetSlotsToSetupPose();

        void AddEquipmentSkinsToPath(Skin combinedSkin)
        {
            Skeleton skeleton = skeletonAnimation.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
            if (!string.IsNullOrEmpty(_path)) combinedSkin.AddSkin(skeletonData.FindSkin(_path));
            Debug.Log(_path);
        }
    }

    void UpdateCombinedSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        Skin resultCombinedSkin = new Skin("character-combined");

        resultCombinedSkin.AddSkin(characterSkin);
        AddEquipmentSkinsTo(resultCombinedSkin);

        skeleton.SetSkin(resultCombinedSkin);
        skeleton.SetSlotsToSetupPose();
    }

    public void OptimizeSkin()
    {
        // Create a repacked skin.
        var previousSkin = skeletonAnimation.Skeleton.Skin;
        // Note: materials and textures returned by GetRepackedSkin() behave like 'new Texture2D()' and need to be destroyed
        if (runtimeMaterial)
            Destroy(runtimeMaterial);
        if (runtimeAtlas)
            Destroy(runtimeAtlas);

        Skin repackedSkin = previousSkin.GetRepackedSkin("Repacked skin", skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial, out runtimeMaterial, out runtimeAtlas);
        previousSkin.Clear();

        // Use the repacked skin.
        skeletonAnimation.Skeleton.Skin = repackedSkin;
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        skeletonAnimation.AnimationState.Apply(skeletonAnimation.Skeleton);

        // `GetRepackedSkin()` and each call to `GetRemappedClone()` with parameter `premultiplyAlpha` set to `true`
        // cache necessarily created Texture copies which can be cleared by calling AtlasUtilities.ClearCache().
        // You can optionally clear the textures cache after multiple repack operations.
        // Just be aware that while this cleanup frees up memory, it is also a costly operation
        // and will likely cause a spike in the framerate.
        AtlasUtilities.ClearCache();
        Resources.UnloadUnusedAssets();

        Debug.Log("OptimizeSkin");

        // Calculate file size
        var texture = runtimeAtlas;
        byte[] textureBytes = texture.EncodeToPNG(); // Or use EncodeToJPG() depending on format
        float fileSizeInKB = textureBytes.Length / 1024f;
        float fileSizeInMB = fileSizeInKB / 1024f;

        textuerData = "runtimeMaterial:" + runtimeMaterial.name +
        "\n runtimeAtlas:" + runtimeAtlas.name +
        "\n" + "Texture: " + texture.width + "x" + texture.height +
        "\n" + "FileSize:" + fileSizeInKB + " KB (" + fileSizeInMB + " MB)";

       
    }

    public void ReloadSkeletonDataAsset()
    {
        if (skeletonAnimation != null && skeletonAnimation.skeletonDataAsset != null)
        {
            // สร้าง SkeletonDataAsset ใหม่จากเดิม
            var skeletonDataAsset = skeletonAnimation.skeletonDataAsset;

            // เรียกใช้ฟังก์ชัน Reload ใน SkeletonDataAsset
            skeletonDataAsset.Clear();
            skeletonDataAsset.GetSkeletonData(true); // true for immediate load

            // รีโหลด Skeleton Animation
            skeletonAnimation.Initialize(true);
        }
    }

    public string GetTextureData()
    {
        return textuerData;
    }

    public Sprite GetTexture()
    {
        Rect rect = new Rect(0, 0, runtimeAtlas.width, runtimeAtlas.height);
        Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot

        Sprite newSprite = Sprite.Create(runtimeAtlas, rect, pivot);

        return Sprite.Create(runtimeAtlas, rect, pivot);
    }
}
