using System.Collections;
using UnityEngine;

public class TossFruits : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool isBomb = false;
    [SerializeField] private ParticlePool particlePool;
    public ParticleSystem particleSys;

    private SoundManger soundManger;
    private float minSpeed = 10f;
    private float maxSpeed = 13.5f;
    private float rotateForce = 10f;
    private Rigidbody rb;
    private bool isSliced = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        soundManger = FindAnyObjectByType<SoundManger>();
    }

    private void OnEnable()
    {
        isSliced = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(Vector3.up * Random.Range(minSpeed, maxSpeed), ForceMode.Impulse);
        rb.AddTorque(
            Random.Range(-rotateForce, rotateForce),
            Random.Range(-rotateForce, rotateForce),
            Random.Range(-rotateForce, rotateForce),
            ForceMode.Impulse
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Limit"))
        {
            if (isBomb)
                GameManager.instance.BombMissed();
            else
                GameManager.instance.FruitMissed();

            gameObject.SetActive(false);
        }

    }


    public void SliceFromBlade()
    {
        Slice();
    }

    private void Slice()
    {
        if (isSliced) return;
        isSliced = true;

        if (isBomb)
        {
            soundManger?.PlayBombClip();
            particlePool?.PlayGameOBPool(transform.position, particlePool.particlePrefabs);
            GameManager.instance.HitBomb();
        }
        else
        {
            particlePool?.PlayGameOBPool(transform.position, particlePool.particlePrefabs);
            particlePool?.PlayGameOBPool(transform.position + new Vector3(0.5f, 0, 0), particlePool.floatPointPrefabs);
            soundManger?.PlaySliceClip();
            GameManager.instance.AddScore(10);
        }

        gameObject.SetActive(false);
    }


    private void OnMouseDown() => Slice();
}
