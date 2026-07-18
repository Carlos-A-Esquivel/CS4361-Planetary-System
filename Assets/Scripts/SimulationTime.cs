using UnityEngine;

public class SimulationTime : MonoBehaviour
{
    public static float TimeScale = 1f;

    [SerializeField] private float initialTimeScale = 1f;

    // All simulation scripts use this instead of Time.deltaTime
    public static float DeltaTime => Time.deltaTime * TimeScale;

    void Awake()
    {
        TimeScale = initialTimeScale;
    }
}