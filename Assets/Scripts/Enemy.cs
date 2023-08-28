using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float health = 100f;
    public float moveSpeed = 2f;
    public float damage = 10f;
    public float cooldown = 1f;
    public float attackRange = 2f;
    public Collider visionCollider;
    public float spoilTime = 10f;

    private float targetDistance;
    private float cooldownTimer;

    void Update()
    {
        if (health <= 0)
        {
            visionCollider.enabled = false;
            Destroy(gameObject, spoilTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            targetDistance = Vector3.Distance(transform.position, other.transform.position);
            if (targetDistance <= attackRange)
            {
                if (cooldownTimer <= 0)
                {
                    other.gameObject.GetComponent<FPSController>().health -= damage;
                    cooldownTimer = cooldown;
                }
                else
                {
                    cooldownTimer -= Time.deltaTime;
                }
            }
            else
            {
                moveTowardsPlayer(other.transform.position);
            }
        }
    }

    private void moveTowardsPlayer(Vector3 playerPosition)
    {
        Vector3 direction = playerPosition - transform.position;
        transform.Translate(direction.normalized * moveSpeed * Time.deltaTime);
    }
}