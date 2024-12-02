using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    public string filePath;

    void Start()
    {
        List<string[]> data = ReadCSV(filePath);
        foreach (var row in data)
        {
            Debug.Log(string.Join(",", row));
        }
    }

    List<string[]> ReadCSV(string path)
    {
        List<string[]> data = new List<string[]>();

        try
        {
            using (StreamReader sr = new StreamReader(path))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    string[] values = line.Split(',');
                    data.Add(values);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("The file could not be read:");
            Debug.LogError(e.Message);
        }

        return data;
    }
}
