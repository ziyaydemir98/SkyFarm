using UnityEngine;

public class StarManager : MonoBehaviour
{
    private CameraParticle cameraParticle;

    private void Start()
    {
        cameraParticle = Camera.main.GetComponent<CameraParticle>();
    }

    public void EnableParticle()
    {
        cameraParticle.PlayWinParticle();
    }
}
