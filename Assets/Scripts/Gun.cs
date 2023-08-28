using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class Gun : MonoBehaviour
{
    public float damage;
    public int magazineSize;
    public bool automatic;
    public float firerate, reloadTime;
    public GameObject bullet, casing, muzzleFlash;
    public Transform bulletExit, casingExit;
    public TMP_Text ammoText;
    public AudioClip shootSound, reloadSound, emptySound;
    public float destroyTimer = 5f;
    public float shotPower;
    private float ejectPower = 40f;

    private Animator animator;
    private AudioSource audioSource;

    public int bullets;
    private bool firing, shooting, reloading;
    public bool canUse;
    public bool infiniteAmmo;

    private int ammo;

    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (automatic) firing = Input.GetButton("Fire1");
        else firing = Input.GetButtonDown("Fire1");

        foreach (GameObject obj in MousePickUp.savedObjects)
        {
            if (obj.TryGetComponent(out AmmoCrate ammoCrate)) ammo = ammoCrate.ammo;
        }

        // Shoot
        if (canUse && firing && !shooting && (bullets > 0 || infiniteAmmo)) Shoot();

        // Reaload
        if (canUse && Input.GetKeyDown(KeyCode.R) && !reloading) Reload();

        // Ammo Text
        if (infiniteAmmo) ammoText.text = "\u25A1";
        else ammoText.text = bullets + " / " + ammo;
    }

    void Shoot()
    {
        shooting = true;

        animator.Play("Fire");
        audioSource.PlayOneShot(shootSound);

        // Muzzle Flash
        if (muzzleFlash != null)
        {
            GameObject f_temp = Instantiate(muzzleFlash, bulletExit.position, bulletExit.rotation);
            Destroy(f_temp, 0.05f);
        }

        // Bullet
        if (bullet != null)
        {
            GameObject b_temp = Instantiate(bullet, bulletExit.position, bulletExit.rotation);
            if (b_temp.TryGetComponent(out Rigidbody rb)) rb.AddForce(bulletExit.forward * shotPower);
            if (b_temp.TryGetComponent(out Bullet bs)) damage = bs.damage;
            Destroy(b_temp, destroyTimer);
        }

        // Casing
        if (casing != null)
        {
            GameObject c_temp = Instantiate(casing, casingExit.position, casingExit.rotation);
            Rigidbody c_rb = c_temp.GetComponent<Rigidbody>();
            c_rb.AddExplosionForce(Random.Range(ejectPower * 0.7f, ejectPower), (casingExit.position - casingExit.right * 0.3f - casingExit.up * 0.6f), 1f);
            c_rb.AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(100f, 1000f)), ForceMode.Impulse);
            Destroy(c_temp, destroyTimer);
        }

        bullets -= 1;

        Invoke("FinishShoot", firerate);
    }

    void FinishShoot()
    {
        shooting = false;
    }

    void Reload()
    {
        reloading = true;

        animator.Play("Reload");
        audioSource.PlayOneShot(reloadSound);

        Invoke("FinishReload", reloadTime);
    }

    void FinishReload()
    {
        reloading = false;

        int bulletsToReload = 0;
        if (ammo > 0)
        {
            bulletsToReload = ammo % magazineSize;
            bullets = bulletsToReload;
        }
        print(bulletsToReload);
        ammo -= bulletsToReload;
        TryToGetAmmoFromInventory();
    }

    void TryToGetAmmoFromInventory()
    {
        foreach (GameObject obj in MousePickUp.savedObjects)
        {
            // If player has ammo add it to current gun
            if (obj.TryGetComponent(out AmmoCrate ammoCrate))
            {
                ammoCrate.ammo -= ammo;
            }
        }
    }
}