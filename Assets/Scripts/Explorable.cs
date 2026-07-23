using UnityEngine;

public class Explorable : MonoBehaviour
{
    [SerializeField] private string explorationSceneName;
    [SerializeField] private float interactDistance = 10f;
    [SerializeField] private Color highlightColor = Color.white;
    [SerializeField] private float highlightIntensity = 0.3f;

    private Material bodyMaterial;
    private Color originalEmission;

    public string SceneName => explorationSceneName;
    public float InteractDistance => interactDistance;

    void Start()
    {
        bodyMaterial = GetComponent<Renderer>().material;
        originalEmission = bodyMaterial.GetColor("_EmissionColor");
    }

    public void SetHighlight(bool on)
    {
        if (on)
        {
            bodyMaterial.EnableKeyword("_EMISSION");
            bodyMaterial.SetColor("_EmissionColor", highlightColor * highlightIntensity);
        }
        else
        {
            bodyMaterial.SetColor("_EmissionColor", originalEmission);
        }
    }
}