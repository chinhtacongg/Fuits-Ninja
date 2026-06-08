using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blade : MonoBehaviour
{
    private Camera mainCamera;
    private bool slicing;
    private Collider bladeCollider;
    private TrailRenderer bladeTrail;

    public Vector3 direction { get; private set; }

    [SerializeField] private float minSlicingVelocity = 0.01f;
    [SerializeField] private float sliceRadius = 0.4f;

    private Vector3 prevPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
        bladeCollider = GetComponent<Collider>();
        bladeTrail = GetComponentInChildren<TrailRenderer>();
    }

    private void OnEnable() => StopSlicing();
    private void OnDisable() => StopSlicing();

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            StartSlicing();
        else if (Input.GetMouseButtonUp(0))
            StopSlicing();
        else if (slicing)
            ContinueSlicing();
    }

    private void StartSlicing()
    {
        Vector3 pos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        pos.z = 0f;

        prevPosition = pos;
        transform.position = pos;

        slicing = true;
        bladeCollider.enabled = true;
        bladeTrail.enabled = true;
        bladeTrail.Clear();
    }

    private void StopSlicing()
    {
        slicing = false;
        bladeCollider.enabled = false;
        bladeTrail.enabled = false;
    }

    private void ContinueSlicing()
    {
        Vector3 newPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        newPos.z = 0f;

        direction = newPos - transform.position;
        float velocity = direction.magnitude / Time.deltaTime;

        if (velocity > minSlicingVelocity)
        {

            Collider[] hits = Physics.OverlapCapsule(prevPosition, newPos, sliceRadius);

            foreach (Collider hit in hits)
            {

                if (hit.CompareTag("Limit")) continue;
                if (hit.gameObject == this.gameObject) continue;

                TossFruits fruit = hit.GetComponent<TossFruits>();
                if (fruit != null)
                    fruit.SliceFromBlade();
            }
        }

        prevPosition = transform.position;
        transform.position = newPos;
    }

}
