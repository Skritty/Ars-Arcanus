using UnityEngine;

public class CandleFlicker : MonoBehaviour
{
    public Light light;
    public Vector2 intensityRange;
    public float directionSwapChance;
    public float intensityRateOfChange;
    private float direction = 1;

    private void Update()
    {
        if(light.intensity <= intensityRange.x) direction = 1;
        else if(light.intensity >= intensityRange.y) direction = -1;
        else if (Random.Range(0f, 1f) < directionSwapChance)
        {
            direction *= -1;
        }
        light.intensity += direction * intensityRateOfChange * Time.deltaTime;
    }
}
