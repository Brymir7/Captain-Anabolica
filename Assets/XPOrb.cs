using System;
using Player;
using UnityEngine;

public class XpOrb : MonoBehaviour
{
    public int xpAmount;
    public float attractionSpeed = 5f; // Speed at which the orb moves towards the player
    public float maxSize = 1f;
    public float maxXPAmount = 100f;
    private bool isAttracting = false;
    private Transform playerTransform;

    void Start()
    {
        var relativeSize = Mathf.Lerp(0.2f, maxSize, xpAmount / maxXPAmount);
        transform.localScale = new Vector3(relativeSize, relativeSize, relativeSize);
    }

    void Update()
    {
        if (isAttracting)
        {
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position,
                attractionSpeed * Time.deltaTime);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerXp>().AddXp(xpAmount);
            Destroy(gameObject);
        }
    }

    public void StartAttraction(Transform player)
    {
        isAttracting = true;
        playerTransform = player;
    }
}