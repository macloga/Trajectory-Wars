using Complete;
using UnityEngine;

public class PlasmaShell : MonoBehaviour
{
    public float speed = 10f;
    public float damage = 40f;
    public float splashRadius = 5f;
    public float maxPushForce = 0.7f;       // 💥 Maximum force if very close
    public float upwardModifier = 0.1f;
    public GameObject impactEffect;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * speed; // ✅ correct property
    }

    void OnCollisionEnter(Collision other)
    {
        // 🌪️ Get all objects within the blast radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, splashRadius);
        foreach (Collider hit in colliders)
        {
            // 🔻 Damage tanks
            TankHealth tank = hit.GetComponent<TankHealth>();
            if (tank != null)
            {
                tank.TakeDamage(damage);
            }

            // 💣 Custom explosion knockback
            Rigidbody hitRb = hit.GetComponent<Rigidbody>();
            if (hitRb != null)
            {
                float distance = Vector3.Distance(hit.transform.position, transform.position);
                float forcePercent = 1f - Mathf.Clamp01(distance / splashRadius); // 1 = close, 0 = far
                float finalForce = maxPushForce * forcePercent;

                hitRb.AddExplosionForce(finalForce, transform.position, splashRadius, upwardModifier, ForceMode.Impulse);
            }
        }

        // 🎥 Camera shake
        if (CameraShake.instance != null)
        {
            StartCoroutine(CameraShake.instance.Shake(0.25f, 0.35f));
        }

        // ✨ Impact effect
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // 🧼 Clean up
        Destroy(gameObject);
    }
}
