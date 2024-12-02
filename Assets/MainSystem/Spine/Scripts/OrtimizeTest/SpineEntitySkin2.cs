using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Spine.Unity.AttachmentTools;
using UnityEngine.Rendering;
using System.IO;
using UnityEditor;

public class SpineEntitySkin2 : MonoBehaviour
{
    [SerializeField] Image imgTexture;
    [SerializeField] TMPro.TextMeshProUGUI fileSizeTX;
    SkeletonAnimation skeletonAnimation;
    Skin characterSkin;

    // for repacking the skin to a new atlas texture preset_hero/King Narai_Ayutthaya_1
    public Material runtimeMaterial;
    public Texture2D runtimeAtlas;

    private void Awake()
    {
        skeletonAnimation = this.GetComponent<SkeletonAnimation>();

    }
    void Start()
    {
        UpdateCharacterSkin();
        OptimizeSkin();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OptimizeSkin();
        }
    }

    void UpdateCharacterSkin()
    {
        Skeleton skeleton = skeletonAnimation.Skeleton;
        SkeletonData skeletonData = skeleton.Data;
        characterSkin = new Skin("character-base");
    }

    public void OptimizeSkinBackup()
    {
        // Create a repacked skin.
        var previousSkin = skeletonAnimation.Skeleton.Skin;

        // Get Texture size
        Material material = skeletonAnimation.SkeletonDataAsset.atlasAssets[0].PrimaryMaterial;
        Texture2D texture = material.mainTexture as Texture2D;

        Debug.Log("Repacked Texture Width: " + name + texture.width);
        Debug.Log("Repacked Texture Height: " + name + texture.height);

        if (fileSizeTX != null) fileSizeTX.text = name + ": "+texture.width+"/"+ texture.height;

        // Calculate file size
        Texture2D texture2D = texture;
        byte[] textureBytes = texture.EncodeToPNG(); // Or use EncodeToJPG() depending on format
        float fileSizeInKB = textureBytes.Length / 1024f;
        float fileSizeInMB = fileSizeInKB / 1024f;

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();

        //Save the encoded PNG to the file path
        string filePath = Application.dataPath + "/repackedSkin.png";
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Skin saved as PNG at: " + filePath);

        Debug.Log("Repacked Texture File Size: " + fileSizeInKB + " KB (" + fileSizeInMB + " MB)" + name);
        if (fileSizeTX != null) fileSizeTX.text = name + " :" + fileSizeInKB + " KB (" + fileSizeInMB + " MB)";
    }

    public void OptimizeSkinBackup_()
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

        // Calculate file size
        var texture = runtimeAtlas;
        byte[] textureBytes = texture.EncodeToPNG(); // Or use EncodeToJPG() depending on format
        float fileSizeInKB = textureBytes.Length / 1024f;
        float fileSizeInMB = fileSizeInKB / 1024f;
        if (fileSizeTX != null) fileSizeTX.text = name + " :" + fileSizeInKB + " KB (" + fileSizeInMB + " MB)";

        //Save the encoded PNG to the file path
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Application.dataPath + "/repackedSkin.png";
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Skin saved as PNG at: " + filePath);

        AtlasUtilities.ClearCache();
        Resources.UnloadUnusedAssets();
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

        // Calculate file size
        var texture = runtimeAtlas;
        byte[] textureBytes = texture.EncodeToPNG(); // Or use EncodeToJPG() depending on format
        float fileSizeInKB = textureBytes.Length / 1024f;
        float fileSizeInMB = fileSizeInKB / 1024f;
        //if (fileSizeTX != null) fileSizeTX.text = name + " :" + fileSizeInKB + " KB (" + fileSizeInMB + " MB)";

        //Debug.log 
        if (fileSizeTX != null) fileSizeTX.text = "runtimeMaterial:" + runtimeMaterial.name +
                "\n runtimeAtlas:" + runtimeAtlas.name +
                "\n"+ "texture: " + texture.width + "/" + texture.height+
                "\n"+fileSizeInKB + " KB (" + fileSizeInMB + " MB)";

        //Texture
        if (imgTexture != null && runtimeAtlas != null)
        {
            Rect rect = new Rect(0, 0, runtimeAtlas.width, runtimeAtlas.height);
            Vector2 pivot = new Vector2(0.5f, 0.5f); // Center pivot

            Sprite newSprite = Sprite.Create(runtimeAtlas, rect, pivot);

            imgTexture.sprite = newSprite;
            imgTexture.preserveAspect = true;
        }

        AtlasUtilities.ClearCache();
        Resources.UnloadUnusedAssets();

        /*
         //Save texture PNG//
        //Save the encoded PNG to the file path
        byte[] bytes = texture.EncodeToPNG();
        string filePath = Application.dataPath + "/repackedSkin.png";
        //File.WriteAllBytes(filePath, bytes);
        Debug.Log("Skin saved as PNG at: " + filePath);*/

        /*
         * //Save material//
        string materialPath = "Assets/SavedMaterials/runtimeMaterial.mat";
        SaveMaterialAsAsset(runtimeMaterial, materialPath);
        */

        /*
         * //Save atlas//
        string atlasPath = "Assets/SavedAtlases/runtimeAtlas.asset";
        SaveAtlasAsAsset(runtimeAtlas, atlasPath);
        */
    }

   
}
