using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    public GameObject prefab; // ใส่ prefab ที่ต้องการใช้ที่นี่
    public int rows = 10; // จำนวนแถว
    public int columns = 10; // จำนวนคอลัมน์
    public float cellWidth = 1.0f; // ความกว้างของแต่ละ cell
    public float cellHeight = 1.0f; // ความสูงของแต่ละ cell

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(column * cellWidth, row * cellHeight, 0);
                Instantiate(prefab, position, Quaternion.identity, transform);
            }
        }
    }
}
