using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] private VariableLevelController levelController;

    private void OnTriggerEnter(Collider other)
    {
        TryComplete(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null)
        {
            return;
        }

        TryComplete(collision.collider);
    }

    private void TryComplete(Collider other)
    {
        if (levelController == null || other == null)
        {
            return;
        }

        if (levelController.IsLevelCompleted || !levelController.IsExitUnlocked)
        {
            return;
        }

        if (other.GetComponentInParent<CharacterController>() != null)
        {
            levelController.CompleteLevel();
        }
    }
}
