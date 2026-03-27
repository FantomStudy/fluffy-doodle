using UnityEngine;

public class ExitZone : MonoBehaviour
{
    [SerializeField] private VariableLevelController levelController;

    private void OnTriggerEnter(Collider other)
    {
        if (levelController == null)
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
