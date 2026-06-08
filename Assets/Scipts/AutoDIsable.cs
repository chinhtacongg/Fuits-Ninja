
using UnityEngine;

public class AutoDIsable : MonoBehaviour
{
    ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnEnable()
    {
        ps.Play();
        Invoke(nameof(Disable), 0.3f);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
