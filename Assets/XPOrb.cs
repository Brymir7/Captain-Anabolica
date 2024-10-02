using System;
using Player;
using UnityEngine;
using UnityEngine.Serialization;

public class XpOrb : MonoBehaviour
{
    public int xpAmount;
    public float attractionSpeed = 5f; // Speed at which the orb moves towards the player
    public float maxSize = 1f;
    [FormerlySerializedAs("maxXPAmount")] public float maxXpAmount = 100f;
    private bool _isAttracting = false;
    private Transform _playerTransform;

    void Start()
    {
        var relativeSize = Mathf.Lerp(0.2f, maxSize, xpAmount / maxXpAmount);
        transform.localScale = new Vector3(relativeSize, relativeSize, relativeSize);
    }

    void Update()
    {
        if (_isAttracting)
        {
            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position,
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
        _isAttracting = true;
        _playerTransform = player;
    }
}