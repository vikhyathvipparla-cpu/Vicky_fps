using System.Collections;
using UnityEngine;

public class HitscanWeapon : MonoBehaviour
{
    [Header("Weapon")]
    public Camera playerCamera;
    public float damage = 25f;
    public float range = 120f;
    public float fireRate = 9f;
    public int magazineSize = 12;
    public float reloadTime = 1.1f;

    [Header("Feedback")]
    public LayerMask hitMask = ~0;
    public Transform muzzlePoint;
    public LineRenderer tracer;
    public float tracerDuration = 0.04f;

    public int AmmoInMagazine { get; private set; }
    public bool IsReloading { get; private set; }

    private float nextFireTime;

    private void Awake()
    {
        AmmoInMagazine = magazineSize;

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (tracer != null)
        {
            tracer.enabled = false;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            TryFire();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartReload();
        }
    }

    public void TryFire()
    {
        if (IsReloading || Time.time < nextFireTime)
        {
            return;
        }

        if (AmmoInMagazine <= 0)
        {
            StartReload();
            return;
        }

        nextFireTime = Time.time + 1f / fireRate;
        AmmoInMagazine--;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 endPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore))
        {
            endPoint = hit.point;

            if (hit.collider.TryGetComponent(out DamageableTarget target))
            {
                target.TakeDamage(damage);
            }
        }

        if (tracer != null)
        {
            StartCoroutine(ShowTracer(endPoint));
        }
    }

    public void StartReload()
    {
        if (!IsReloading && AmmoInMagazine < magazineSize)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    private IEnumerator ReloadRoutine()
    {
        IsReloading = true;
        yield return new WaitForSeconds(reloadTime);
        AmmoInMagazine = magazineSize;
        IsReloading = false;
    }

    private IEnumerator ShowTracer(Vector3 endPoint)
    {
        Vector3 startPoint = muzzlePoint != null ? muzzlePoint.position : playerCamera.transform.position;

        tracer.enabled = true;
        tracer.positionCount = 2;
        tracer.SetPosition(0, startPoint);
        tracer.SetPosition(1, endPoint);

        yield return new WaitForSeconds(tracerDuration);

        tracer.enabled = false;
    }
}
