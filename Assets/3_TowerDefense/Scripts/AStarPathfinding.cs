using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarPathfinding : MonoBehaviour
{
    public Transform startNode; // ตำแหน่งเริ่มต้น
    public Transform targetNode; // ตำแหน่งเป้าหมาย
    public Transform[] allNodes; // โหนดทั้งหมดในเส้นทาง

    private List<Transform> FindPath(Transform start, Transform target)
    {
        List<Transform> openList = new List<Transform>(); // โหนดที่ต้องตรวจสอบ
        List<Transform> closedList = new List<Transform>(); // โหนดที่ตรวจสอบแล้ว

        Dictionary<Transform, Transform> cameFrom = new Dictionary<Transform, Transform>(); // เก็บเส้นทางย้อนกลับ
        Dictionary<Transform, float> gScore = new Dictionary<Transform, float>(); // เก็บค่า G ของแต่ละโหนด
        Dictionary<Transform, float> fScore = new Dictionary<Transform, float>(); // เก็บค่า F ของแต่ละโหนด

        foreach (Transform node in allNodes)
        {
            gScore[node] = Mathf.Infinity; // ตั้งค่าเริ่มต้นของ G เป็น Infinity
            fScore[node] = Mathf.Infinity; // ตั้งค่าเริ่มต้นของ F เป็น Infinity
        }

        gScore[start] = 0; // G ของจุดเริ่มต้นคือ 0
        fScore[start] = HeuristicCostEstimate(start, target); // F คือค่า H (จาก start ไป target)

        openList.Add(start);

        while (openList.Count > 0)
        {
            Transform current = GetNodeWithLowestF(openList, fScore); // หาโหนดที่มีค่า F ต่ำที่สุด

            if (current == target) // ถ้าถึงโหนดเป้าหมายแล้ว
            {
                return ReconstructPath(cameFrom, current); // คืนค่าเส้นทางที่พบ
            }

            openList.Remove(current); // เอา current ออกจาก openList
            closedList.Add(current); // เพิ่ม current เข้า closedList

            foreach (Transform neighbor in GetNeighbors(current)) // หาทุกโหนดที่อยู่ใกล้เคียง
            {
                if (closedList.Contains(neighbor)) // ถ้าอยู่ใน closedList แล้ว ข้ามไป
                    continue;

                float tentative_gScore = gScore[current] + Vector3.Distance(current.position, neighbor.position); // คำนวณ G ใหม่

                if (!openList.Contains(neighbor))
                    openList.Add(neighbor); // เพิ่ม neighbor เข้า openList ถ้ายังไม่ได้อยู่ในนี้

                if (tentative_gScore >= gScore[neighbor])
                    continue; // ถ้า G ที่ได้มากกว่าหรือเท่ากับ G ที่มีอยู่ ข้ามไป

                // บันทึกเส้นทางและอัปเดตค่า G/F
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentative_gScore;
                fScore[neighbor] = gScore[neighbor] + HeuristicCostEstimate(neighbor, target); // F = G + H
            }
        }

        return null; // ไม่มีเส้นทาง
    }

    // ฟังก์ชันที่ใช้คำนวณค่า H (heuristic) เป็นระยะห่างแบบตรงจากโหนดปัจจุบันไปยังเป้าหมาย
    private float HeuristicCostEstimate(Transform from, Transform to)
    {
        return Vector3.Distance(from.position, to.position); // ใช้ระยะห่าง Euclidean
    }

    // หาค่า F ต่ำที่สุดจาก openList
    private Transform GetNodeWithLowestF(List<Transform> openList, Dictionary<Transform, float> fScore)
    {
        Transform lowest = openList[0];
        foreach (Transform node in openList)
        {
            if (fScore[node] < fScore[lowest])
            {
                lowest = node;
            }
        }
        return lowest;
    }

    // สร้างเส้นทางจาก cameFrom
    private List<Transform> ReconstructPath(Dictionary<Transform, Transform> cameFrom, Transform current)
    {
        List<Transform> totalPath = new List<Transform>();
        totalPath.Add(current);
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Add(current);
        }
        totalPath.Reverse();
        return totalPath;
    }

    // ฟังก์ชันตัวอย่างที่จะดึง neighbors ของโหนด (ในที่นี้ควรเป็นระบบที่คุณสร้างขึ้นเอง)
    private List<Transform> GetNeighbors(Transform node)
    {
        List<Transform> neighbors = new List<Transform>();
        foreach (Transform t in allNodes)
        {
            if (t != node && Vector3.Distance(node.position, t.position) < 5.0f) // หาโหนดที่อยู่ในระยะ 5 หน่วย
            {
                neighbors.Add(t);
            }
        }
        return neighbors;
    }

    // ใช้ทดสอบใน Start
    void Start()
    {
        List<Transform> path = FindPath(startNode, targetNode);

        if (path != null)
        {
            foreach (Transform node in path)
            {
                Debug.Log("Path node: " + node.name);
            }
        }
        else
        {
            Debug.Log("No path found");
        }
    }
}
