using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class WeaponShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 14f;
    private bool isReloading = false;

    public float shootCooldown = 5f; // ⏳ Tiempo entre disparos (en segundos)
    private float lastShootTime = -Mathf.Infinity;

    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    void Start()
    {
        currentAmmo = maxAmmo;
        AmmoUI.instance?.UpdateAmmoText(currentAmmo, maxAmmo);
    }

    void Update()
    {
        if (GameManager.inputBloqueado || EventSystem.current.IsPointerOverGameObject())
            return;

        bool puedeDisparar = Time.time >= lastShootTime + shootCooldown;

        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading && puedeDisparar)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }
    }

    void Shoot()
    {
        currentAmmo--;
        lastShootTime = Time.time; // ⏲️ Guardamos el tiempo del último disparo

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.forward * bulletSpeed;
        }

        AmmoUI.instance?.UpdateAmmoText(currentAmmo, maxAmmo);
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        if (audioSource && reloadSound)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(reloadSound);
            reloadTime = reloadSound.length;
        }

        yield return new WaitForSeconds(reloadTime);

        currentAmmo = maxAmmo;
        isReloading = false;
        AmmoUI.instance?.UpdateAmmoText(currentAmmo, maxAmmo);
    }
}
