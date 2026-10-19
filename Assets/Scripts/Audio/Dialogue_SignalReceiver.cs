using UnityEngine;

public class Dialogue_SignalReceiver : MonoBehaviour
{
    [SerializeField] private Transform sourceTransform;
    [SerializeField] private SoundEffect[] lines;

    private int iterator = 0;
    public bool fromMainCamera = false;
    private GameObject mainCam;
    
    private void Start()
    {
        if (fromMainCamera)
        {
            mainCam = Camera.main.gameObject;
            sourceTransform = mainCam.transform;
        }
    }

    public void PlayNextLine()
    {
        AudioManager.Instance.PlaySFXFollow(lines[iterator], sourceTransform);
        iterator = ++iterator % lines.Length;
    }
}