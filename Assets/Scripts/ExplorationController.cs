using UnityEngine;
using UnityEngine.SceneManagement;

public class ExplorationController : MonoBehaviour
{
    private Explorable currentTarget;

    void Update()
    {
        Explorable hitTarget = null;

        // Fire a ray from the camera through the center of the screen
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Explorable candidate = hit.collider.GetComponent<Explorable>();

            // Only counts if it's explorable AND we're close enough
            if (candidate != null && hit.distance <= candidate.InteractDistance)
            {
                hitTarget = candidate;
            }
        }

        // Highlight changed — turn the old one off, the new one on
        if (hitTarget != currentTarget)
        {
            if (currentTarget != null) currentTarget.SetHighlight(false);
            if (hitTarget != null) hitTarget.SetHighlight(true);
            currentTarget = hitTarget;
        }

        // Click to enter
        if (currentTarget != null && Input.GetMouseButtonDown(0))
        {
            if (!string.IsNullOrEmpty(currentTarget.SceneName))
            {
                SceneManager.LoadScene(currentTarget.SceneName);
            }
        }
    }
}