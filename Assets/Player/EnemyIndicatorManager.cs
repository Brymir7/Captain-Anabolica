using UnityEngine;
using System.Collections.Generic;

public class EnemyIndicatorManager : MonoBehaviour
{
    public GameObject player;
    public Camera mainCamera;
    public List<Transform> enemyTransforms;
    public EnemyManager enemyManager;
    public List<GameObject> arrows = new List<GameObject>();
    public GameObject arrowPrefab;
    public float borderX = 30f;
    public float minAngle = -60f;
    public float maxAngle = 60f;

    private void Start()
    {
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager not found!");
        }

        if (arrowPrefab == null)
        {
            Debug.LogError("Arrow prefab not assigned!");
        }
    }

    private void FixedUpdate()
    {
        enemyTransforms = enemyManager.GetTransforms();
        List<int> notVisibleEnemies = new List<int>();
        for (int i = 0; i < enemyTransforms.Count; i++)
        {
            Transform t = enemyTransforms[i];
            if (!IsEnemyVisible(t))
            {
                notVisibleEnemies.Add(i);
            }
        }

        UpdateArrows(notVisibleEnemies);
        PrintNonVisibleEnemyRotations();
    }

    private void UpdateArrows(List<int> notVisibleEnemies)
    {
        while (arrows.Count < notVisibleEnemies.Count)
        {
            GameObject newArrow = Instantiate(arrowPrefab, transform);
            arrows.Add(newArrow);
        }

        while (arrows.Count > notVisibleEnemies.Count)
        {
            GameObject arrowToRemove = arrows[arrows.Count - 1];
            arrows.RemoveAt(arrows.Count - 1);
            Destroy(arrowToRemove);
        }

        notVisibleEnemies.ForEach(delegate(int visibleEnemy)
        {
            UpdateArrowTransform(arrows[visibleEnemy], enemyTransforms[visibleEnemy]);
        });
    }

    private void UpdateArrowTransform(GameObject arrow, Transform enemyTransform)
    {
        Vector3 directionToEnemy = enemyTransform.position - player.transform.position;
        float angle = Vector3.SignedAngle(player.transform.forward, directionToEnemy, Vector3.up);

        float screenWidth = Screen.width;
        float arrowX;
        float arrowY;
        if (angle < -45f)
        {
            arrowX = Mathf.Lerp(borderX, Screen.width / 2f - borderX, Mathf.InverseLerp(-45, -180, angle));
            arrowY = Mathf.Lerp(Screen.height / 2f, 300, Mathf.InverseLerp(-45, -90, angle));
        }
        else
        {
            arrowX = Mathf.Lerp(Screen.width / 2f, Screen.width - borderX , Mathf.InverseLerp(180, 45, angle));
            arrowY = Mathf.Lerp(300, Screen.height / 2f, Mathf.InverseLerp(90, 45, angle));
        }

        arrow.transform.position = new Vector3(arrowX, arrowY, arrow.transform.position.z);
        arrow.transform.rotation = Quaternion.Euler(0f, 0f, -angle);
    }


    private void PrintNonVisibleEnemyRotations()
    {
        foreach (Transform enemy in enemyTransforms)
        {
            if (!IsEnemyVisible(enemy))
            {
                Vector3 directionToEnemy = enemy.position - player.transform.position;
                float angle = Vector3.SignedAngle(player.transform.forward, directionToEnemy, Vector3.up);
                Debug.Log($"Non-visible enemy rotation: {angle} degrees");
            }
        }
    }

    private bool IsEnemyVisible(Transform enemy)
    {
        Vector3 screenPoint = mainCamera.WorldToViewportPoint(enemy.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}