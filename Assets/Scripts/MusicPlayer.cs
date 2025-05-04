using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public AudioSource musicSource;
    public float pulseMultiplier = 5f;
    public float baseEmission = 0.2f;
    public Material[] pathMaterials;

    private float[] spectrumData = new float[64];

    void Update()
    {
        AudioListener.GetSpectrumData(spectrumData, 0, FFTWindow.Blackman);
        float intensity = spectrumData[1] * pulseMultiplier + baseEmission;

        foreach (Material mat in pathMaterials)
        {
            if (mat.HasProperty("_EmissionColor"))
            {
                Color baseColor = mat.color;
                Color emissionColor = baseColor * intensity;
                mat.SetColor("_EmissionColor", emissionColor);
            }
        }
    }
}
