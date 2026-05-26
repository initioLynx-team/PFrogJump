using UnityEngine;

public class SIndicatorLook : MonoBehaviour
{

    public GameObject indicator;
    private ParticleSystem indicatorParticles;

    public float lookDistance = 2f;
    public float maxDistance = 4f;
    private float actualDist = 1f;
    public float appearSpeed = 2.5f;

    [Range(0f, 1f)]
    public float deadzone = 0.15f;

    public Gradient strengthGradient;
    private ParticleSystem.Particle[] particles;
    private void Awake()
    {
        if (indicator != null)
        {
            indicatorParticles = indicator.GetComponent<ParticleSystem>();
        }
    }
    public void SetVisible(bool visible)
    {
        indicator.SetActive(visible);
    }
    public void UpdateLookDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude < deadzone)
        {
            ResetIndicator();
            return;
        }

        SetVisible(true);
        actualDist = Mathf.MoveTowards(actualDist, lookDistance, appearSpeed * Time.deltaTime);
        PositionAndRotateIndicator(direction);
        UpdateParticleColor(0f);
    }

    public void UpdateThrowDirection(Vector2 direction, float strength)
    {
        if (direction.sqrMagnitude < deadzone)
        {
            ResetIndicator();
            return;
        }

        SetVisible(true);
        strength = Mathf.Clamp01(strength);
        float targetDist = Mathf.Lerp(lookDistance, maxDistance, strength);
        actualDist = Mathf.MoveTowards(actualDist, targetDist, appearSpeed * Time.deltaTime);
        PositionAndRotateIndicator(direction);
        UpdateParticleColor(strength);
    }


    private void PositionAndRotateIndicator(Vector2 direction)
    {
        Vector2 normDir = direction.normalized;
        indicator.transform.localPosition = (Vector3)normDir * actualDist;

        float angle = Mathf.Atan2(normDir.y, normDir.x) * Mathf.Rad2Deg;
        indicator.transform.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    private void UpdateParticleColor(float strength)
    {
if (indicatorParticles != null)
    {
        Color strengthColor = strengthGradient.Evaluate(strength);
        strengthColor.a = 1f; 

        var mainModule = indicatorParticles.main;
        mainModule.startColor = strengthColor;

        int maxParticles = mainModule.maxParticles;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        int numParticlesAlive = indicatorParticles.GetParticles(particles);
        for (int i = 0; i < numParticlesAlive; i++)
        {
            particles[i].startColor = strengthColor;
        }
        indicatorParticles.SetParticles(particles, numParticlesAlive);
    }
    }

    private void ResetIndicator()
    {
        actualDist = 0f;
        SetVisible(false);
    }

}
