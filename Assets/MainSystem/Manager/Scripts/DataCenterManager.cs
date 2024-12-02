using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using Unity.Mathematics;
using System.Security.Cryptography;

public class DataCenterManager : MonoBehaviour
{
    private static DataCenterManager _Instance;
    public static DataCenterManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                GameObject obj = new GameObject("[Script]: -- DataCenterManager Instance --");
                _Instance = obj.AddComponent<DataCenterManager>();
                DontDestroyOnLoad(obj);
            }
            return _Instance;
        }
    }

    public static T GetData<T>(ref T _data, string _nameObj) where T : Component
    {
        if (_data == null)
        {
            GameObject obj = GameObject.Find(_nameObj);
            if (obj != null)
            {
                _data = obj.GetComponent<T>();
            }
            else
            {
                Debug.LogWarning($"GameObject named {_nameObj} not found.");
            }
        }
        return _data;
    }

    public void LoadSceneByName(string sceneName)
    {
        // โหลดซีนตามชื่อที่ระบุ
        SceneManager.LoadScene(sceneName);
    }

    public static string GenerateID()
    {
        int length = 10;
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        StringBuilder result = new StringBuilder(length);

        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] buffer = new byte[1];
            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(buffer);
                int randomIndex = buffer[0] % chars.Length; // ใช้ค่า buffer[0] เพื่อสุ่มตำแหน่งใน chars
                result.Append(chars[randomIndex]);
            }
        }

        return result.ToString();
    }
}
