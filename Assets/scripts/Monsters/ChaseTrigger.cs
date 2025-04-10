using UnityEngine;

public class ChaseTrigger : MonoBehaviour
{
    [SerializeField] private MonsterController monsterController;
    [SerializeField] private bool isStartTrigger = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && monsterController != null)
        {
            if (isStartTrigger)
            {
                monsterController.StartChasing(other.transform);
            }
            else
            {
                monsterController.StopChasing();
            }
        }
    }
}