using UnityEngine;

public class DamageableTarget : MonoBehaviour
{
    public float maxHealth = 100f;
    public bool respawn = true;
    public float respawnDelay = 2f;

    private float health;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private Renderer[] renderers;
    private Collider[] colliders;

    private void Awake()
    {
        health = maxHealth;
        startPosition = transform.position;
        startRotation = transform.rotation;
        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Defeat();
        }
    }

    private void Defeat()
    {
        GameManager.Instance?.AddScore(1);

        if (!respawn)
        {
            Destroy(gameObject);
            return;
        }

        SetVisible(false);
        Invoke(nameof(Respawn), respawnDelay);
    }

    private void Respawn()
    {
        health = maxHealth;
        transform.SetPositionAndRotation(startPosition, startRotation);
        SetVisible(true);
    }

    private void SetVisible(bool visible)
    {
        foreach (Renderer targetRenderer in renderers)
        {
            targetRenderer.enabled = visible;
        }

        foreach (Collider targetCollider in colliders)
        {
            targetCollider.enabled = visible;
        }
    }
}
