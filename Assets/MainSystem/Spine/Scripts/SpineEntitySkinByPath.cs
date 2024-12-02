using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEditor;
using Spine.Unity.Examples;

public class SpineEntitySkinByPath : MonoBehaviour
{
    public string[] paths;
    
    SkeletonAnimation skeletonAnimation;
    Skin characterSkin;

    // for repacking the skin to a new atlas texture
    public Material runtimeMaterial;
    public Texture2D runtimeAtlas;

    [SerializeField] private string textuerData;
    private Texture2D texture;

    private void Awake()
    {
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();
    }

    public void Start() {}
    public void ChangeSkinFormPath(string[] _paths)
    {
        paths = _paths;
        //ReloadSkeletonDataAsset();
        UpdateCharacterSkin();
        UpdateCombinedSkin();
        OptimizeSkin();
    }

    void UpdateCharacterSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        characterSkin = new Skin("character-base");

        int index = 0;
        paths.ToList().ForEach(o => {
            //Debug.Log("Path:" + o);
            if (!string.IsNullOrEmpty(o)) characterSkin.AddSkin(skeletonData.FindSkin(o));
            index += 1;
        });
    }

    void UpdateCombinedSkin()
    {
        var skeleton = skeletonAnimation.Skeleton;
        var resultCombinedSkin = new Skin("character-combined");

        resultCombinedSkin.AddSkin(characterSkin);
        _AddEquipmentSkinsTo(resultCombinedSkin);

        skeleton.SetSkin(resultCombinedSkin);
        skeleton.SetSlotsToSetupPose();

        void _AddEquipmentSkinsTo(Skin combinedSkin)
        {
            Skeleton skeleton = skeletonAnimation.Skeleton;
            SkeletonData skeletonData = skeleton.Data;
            /*
            int index = 0;
            paths.ToList().ForEach(o => {
                Debug.Log("Path:" + o);
                if (!string.IsNullOrEmpty(o)) combinedSkin.AddSkin(skeletonData.FindSkin(o));
                index += 1;
            });
            */
        }
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
