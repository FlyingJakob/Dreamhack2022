using UnityEngine;

public class Thruster : MonoBehaviour
{
    [Range(0,1)]
    public float currentThrust;
    public ParticleSystem ps;

    public float minLength;
    public float MaxLength;

    public float minSize;
    public float maxSize;

    public float minSimSpeed;
    public float maxSimSpeed;

    public void SetThrust(float thrust)
    {
        this.currentThrust = thrust;
    }
    
    private void Awake()
    {
        ps = GetComponentInChildren<ParticleSystem>();
    }

    public AudioSource audioSource;
    public float maxPitch;
    public float randomPitchAmount;
    void Update()
    {
        var main = ps.main;

        float lifetime = Mathf.Lerp(minLength, MaxLength, currentThrust);
        main.startLifetime = new ParticleSystem.MinMaxCurve(lifetime, lifetime*2f);
        
        float size = Mathf.Lerp(minSize, maxSize, currentThrust);
        main.startSize = new ParticleSystem.MinMaxCurve(size, size+2);
        
        float speed = Mathf.Lerp(minSimSpeed, maxSimSpeed, currentThrust);
        main.simulationSpeed = speed;

        audioSource.pitch = 1 + currentThrust * maxPitch+Random.Range(-1,1)*randomPitchAmount;

        audioSource.volume = 0.1f + currentThrust;

    }
}
