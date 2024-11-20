using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableRealGravityWhileTouching : MonoBehaviour
{
    [SerializeField] private float gravity;
    [SerializeField] private List<string> tags;

    private struct GravityData
    {
        public int NumColliders { get; set; }
        public float OriginalGravity { get; set; }

        public GravityData(int numColliders, float originalGravity)
        {
            NumColliders = numColliders;
            OriginalGravity = originalGravity;
        }
    }

    private static Dictionary<GameObject, GravityData> orignialGravityStorage;

    private void Awake() => orignialGravityStorage = new Dictionary<GameObject, GravityData>();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject gameObject = collision.gameObject;

        if (!HasTag(gameObject))
        {
            return;
        }

        Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();

        if (rigidbody == null)
        {
            return;
        }

        if (!orignialGravityStorage.ContainsKey(gameObject))
        {
            orignialGravityStorage.Add(gameObject, new GravityData(1, rigidbody.gravityScale));
        }
        else
        {
            GravityData gravityData = orignialGravityStorage[gameObject];
            gravityData.NumColliders++;
        }

        rigidbody.gravityScale = gravity;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        GameObject gameObject = collision.gameObject;

        if (orignialGravityStorage.TryGetValue(gameObject, out var gravityData))
        {
            gravityData.NumColliders--;

            if (gravityData.NumColliders > 0)
            {
                return;
            }

            Rigidbody2D rigidbody = gameObject.GetComponent<Rigidbody2D>();

            if (rigidbody != null)
            {
                rigidbody.gravityScale = gravityData.OriginalGravity;
            }

            orignialGravityStorage.Remove(gameObject);
        }
    }

    private bool HasTag(GameObject gameObject)
    {
        foreach (string tag in tags)
        {
            if (gameObject.CompareTag(tag))
            {
                return true;
            }
        }

        return false;
    }
}
