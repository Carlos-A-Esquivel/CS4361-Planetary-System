using UnityEngine;

public class OrbitalMotion : MonoBehaviour
{
    [SerializeField] private Transform orbitCenter;        // the body this one orbits
    [SerializeField] private float orbitalRadius = 15f;
    [SerializeField] private float orbitalPeriodDays = 365f;
    [SerializeField] private float startAngleDegrees = 0f;

    private float currentAngle;

    void Start()
    {
        currentAngle = startAngleDegrees * Mathf.Deg2Rad;
        UpdatePosition();
    }

    void LateUpdate()
    {
        // Advance the angle: a full 2*PI sweep over one orbital period
        float radiansPerSecond = (2f * Mathf.PI) / orbitalPeriodDays;
        currentAngle += radiansPerSecond * SimulationTime.DeltaTime;

        UpdatePosition();
    }

    void UpdatePosition()
    {
        if (orbitCenter == null) return;

        // Circular orbit in the XZ plane, offset from whatever the center body's position is right now
        float x = Mathf.Cos(currentAngle) * orbitalRadius;
        float z = Mathf.Sin(currentAngle) * orbitalRadius;

        transform.position = orbitCenter.position + new Vector3(x, 0f, z);
    }
}