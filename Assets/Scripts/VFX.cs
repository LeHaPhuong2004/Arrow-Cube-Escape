using UnityEngine;

public class VFXManager : MonoBehaviour
{
    public static VFXManager Instance;

    public GameObject fireworkPrefab;

    void Awake()
    {
        Instance = this;
    }

    public void PlayFirework(Vector3 pos)
    {
        GameObject fx = Instantiate(fireworkPrefab, pos, Quaternion.identity);
        Destroy(fx, 3f);
    }

    public void PlayWinEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = new Vector3(
                Random.Range(-2.5f, 2.5f),
                Random.Range(1f, 3.5f),
                0
            );

            PlayFirework(pos);
        }
    }
}