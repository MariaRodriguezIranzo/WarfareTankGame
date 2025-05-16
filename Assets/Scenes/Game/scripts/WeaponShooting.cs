using System.Collections;
using UnityEngine;
using TMPro;

public class WeaponShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 20f;
    public int maxAmmo = 10;
    private int currentAmmo;
    public float reloadTime = 14; 
    private bool isReloading = false;
    public TextMeshProUGUI normalText;
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip reloadSound;

    void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoText();
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && currentAmmo > 0 && !isReloading)
        {
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < maxAmmo)
        {
            StartCoroutine(Reload());
        }

        UpdateAmmoText();
    }

    void Shoot()
    {
        currentAmmo--;

        if (audioSource && shootSound)
        {
            audioSource.PlayOneShot(shootSound);
        }

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = firePoint.up * bulletSpeed;
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;

        // Reproducimos el sonido de recarga una sola vez
        if (audioSource && reloadSound)
        {
            audioSource.loop = false; // Asegurar que no está en loop
            audioSource.PlayOneShot(reloadSound);
            reloadTime = reloadSound.length;
        }

        // Esperamos el tiempo de recarga
        yield return new WaitForSeconds(reloadTime);

        // Restaura la munición y termina la recarga
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void UpdateAmmoText()
    {
        if (normalText != null)
        {
            normalText.text = currentAmmo + "/" + maxAmmo;
        }
    }
}
