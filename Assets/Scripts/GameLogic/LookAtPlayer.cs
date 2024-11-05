using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform player; 

    private void Update()
    {
        if (player != null)
        {
            Vector3 direction = player.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }
}
