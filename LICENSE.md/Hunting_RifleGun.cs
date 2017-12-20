using System.Collections;
using UnityEngine;
public class Hunting_RifleGun : MonoBehaviour
{
    public float damage = 40f;
    public float impactForce = 1000f;
    public float fireRate = 4f;
    private bool readyToFire = true;
    public float maxBulletSpreadAngle = 5f;
    public float scopedMaxBulletSpreadAngle = 1f;


    public Camera fpsCam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;
    public Animator animator;
    public Camera mainCamera;
    //---forsniperscopes---public GameObject scopeOverlay;
    //---forsniperscopes---public GameObject weaponCamera;
    public Camera weaponCamera;

    public float scopedFOV = 15f;
    private float normalFOV = 60f;
    public float weaponScopedFOV = 15f;
    private float weaponNormalFOV = 60f;

    public int maxAmmo = 30;
    private int currentAmmo;
    public float reloadTime = 1f;
    public bool isReloading = false;

    private float nextTimeToFire = 0f;
    public bool isScoped = false;
    public bool isShootingUnscoped = false;
    public bool isShootingScoped = false;

    void Start()
    {
        currentAmmo = maxAmmo;
    }
    void OnEnable()
    {
        isReloading = false;
        animator.SetBool("Reloading", false);
    }
    void Update()
    {
        //ammo
        if (isReloading)
            return;
        if (currentAmmo <= 0 /*|| Input.GetButtonDown(KeyCode.R)*/)
        {
            isShootingUnscoped = false;
            animator.SetBool("IsShootingUnscoped", isShootingUnscoped);
            if (isScoped == true)
            {
                isShootingScoped = false;
                isScoped = false;
                animator.SetBool("IsShootingScoped", isShootingScoped);
                animator.SetBool("IsScoped", isShootingScoped);
                isScoped = false;
                StartCoroutine(OnUnscoped());
            }
            StartCoroutine(Reload());
            return;
        }
        //fireing
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
        {
            Shoot();
            nextTimeToFire = Time.time + 1f / fireRate;
        }
        //shooting unscoped
        if (Input.GetButton("Fire1") && isScoped == false)
        {
            isShootingUnscoped = true;
            animator.SetBool("IsShootingUnscoped", isShootingUnscoped);
        }
        if (Input.GetButtonUp("Fire1") && isScoped == false)
        {
            isShootingUnscoped = false;
            animator.SetBool("IsShootingUnscoped", isShootingUnscoped);
        }
        //shooting scoped
        if (Input.GetButton("Fire1") && isScoped == true)
        {
            isShootingScoped = true;
            animator.SetBool("IsShootingScoped", isShootingScoped);
        }
        if (Input.GetButtonUp("Fire1") && isScoped == true)
        {
            isShootingScoped = false;
            animator.SetBool("IsShootingScoped", isShootingScoped);
        }
        // unscoping while shooting
        if (Input.GetButtonDown("Fire2") && isShootingScoped == true && isScoped == true)
        {
            isShootingScoped = false;
            isShootingUnscoped = true;
            animator.SetBool("IsShootingUnscoped", isShootingUnscoped);
            animator.SetBool("IsShootingScoped", isShootingScoped);
        }
        //scoping while shooting
        if (Input.GetButtonDown("Fire2") && isShootingUnscoped == true && isScoped == false)
        {
            isShootingScoped = true;
            isShootingUnscoped = false;
            animator.SetBool("IsShootingUnscoped", isShootingUnscoped);
            animator.SetBool("IsShootingScoped", isShootingScoped);
        }
        // aiming down sights
        if (Input.GetButtonDown("Fire2"))
        {
            isScoped = !isScoped;
            animator.SetBool("IsScoped", isScoped);
            // --- forsniperscopes---   scopeOverlay.SetActive(isScoped);
            if (isScoped == true)
                StartCoroutine(OnScoped());
            if (isScoped == false)
                StartCoroutine(OnUnscoped());
        }

    }
    IEnumerator Reload()
    {
        if (isScoped == false)
            yield return new WaitForSeconds(.15f);
        isReloading = true;
        animator.SetBool("IsReloading", isReloading);
        yield return new WaitForSeconds(reloadTime);
        isReloading = false;
        animator.SetBool("IsReloading", isReloading);
        currentAmmo = maxAmmo;
    }
    void Shoot()
    {
        currentAmmo--;
        muzzleFlash.Play();
        RaycastHit hit;
        Vector3 fireDirection = fpsCam.transform.forward;
        Quaternion fireRotation = Quaternion.LookRotation(fireDirection);
        Quaternion randomRotation = Random.rotation;
        if (isScoped == false)
        {
            fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0.0f, maxBulletSpreadAngle));
        }
        if (isScoped == true)
        {
            fireRotation = Quaternion.RotateTowards(fireRotation, randomRotation, Random.Range(0.0f, scopedMaxBulletSpreadAngle));
        }
        if (Physics.Raycast(fpsCam.transform.position, fireRotation * Vector3.forward, out hit))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForce(-hit.normal * impactForce);
            }
            Object impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGO, 2f);
            readyToFire = false;
            Invoke("SetReadyToFire", fireRate);
        }
    }
    void SetReadyToFire()
    {
        readyToFire = true;
    }
    IEnumerator OnUnscoped()
    {
        //---forsniperscopes---scopeOverlay.SetActive(false);
        //---forsniperscopes---weaponCamera.SetActive(true);
        //hard coding ahead
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 51f;
        weaponCamera.fieldOfView = 33f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 52f;
        weaponCamera.fieldOfView = 36f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 53f;
        weaponCamera.fieldOfView = 39f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 54f;
        weaponCamera.fieldOfView = 42f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 55f;
        weaponCamera.fieldOfView = 45f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 56f;
        weaponCamera.fieldOfView = 48f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 57f;
        weaponCamera.fieldOfView = 51f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 58f;
        weaponCamera.fieldOfView = 54f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 59f;
        weaponCamera.fieldOfView = 57f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 60f;
        weaponCamera.fieldOfView = 60f;
    }
    IEnumerator OnScoped()
    {
        //---forsniperscopes---scopeOverlay.SetActive(true);
        //---forsniperscopes---weaponCamera.SetActive(false);
        //---forsniperscopes---normalFOV = mainCamera.fieldOfView;
        //hard coding ahead
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 59f;
        weaponCamera.fieldOfView = 57f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 58f;
        weaponCamera.fieldOfView = 54f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 57f;
        weaponCamera.fieldOfView = 51f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 56f;
        weaponCamera.fieldOfView = 48f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 55f;
        weaponCamera.fieldOfView = 45f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 54f;
        weaponCamera.fieldOfView = 42f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 53f;
        weaponCamera.fieldOfView = 39f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 52f;
        weaponCamera.fieldOfView = 36f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 51f;
        weaponCamera.fieldOfView = 33f;
        yield return new WaitForSeconds(.01f);
        mainCamera.fieldOfView = 50f;
        weaponCamera.fieldOfView = 30f;
    }
}