using UnityEngine;

public class GlowPulse : MonoBehaviour
{
    private Material mat;

    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        float intensity = Mathf.PingPong(Time.time * 2f, 2f) + 2f;
        mat.SetColor("_EmissionColor", Color.cyan * intensity);
    }
} 