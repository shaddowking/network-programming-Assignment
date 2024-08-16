using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gun : NetworkBehaviour
{

    private GameObject AmmoCountHolder;
    private TextMeshProUGUI AmmoCountDisplay;
    private GameObject AmmoCountDisplayHolder;
    private GameObject RelodingDisplay;
    
    
    [SerializeField] private GameObject bullet;
    
    [SerializeField] private float ShootForce, upwardForce;

    [SerializeField] private float timeBetweenShooting, spread, realodeTime, timeBetweenShots;
    [SerializeField] private int MagazineSize, bulletsPerTap;
    [SerializeField] private bool allowButtonHold;

    private int bulletsLeft, bulletsShot;

    private bool shooting, readyToShoot, reloding;

    [SerializeField] private Camera playerCam;
    [SerializeField] private  Transform attackPoint;

    [SerializeField] private bool allowInvoke = true;

    private void Start()
    {
        
        AmmoCountHolder = GameObject.FindWithTag("AmmoCountHolder");
        AmmoCountDisplayHolder = AmmoCountHolder.transform.GetChild(0).gameObject;
        AmmoCountDisplay = AmmoCountDisplayHolder.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        RelodingDisplay = AmmoCountHolder.transform.GetChild(1).gameObject;
        RelodingDisplay.SetActive(false);
        AmmoCountDisplay.text = bulletsLeft.ToString();


    }

    private void Awake()
    {
        bulletsLeft = MagazineSize;
        readyToShoot = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsLocalPlayer && gameObject.GetComponent<player>().CanMove)
        {
            MyInput();
        }
    }

    private void MyInput()
    {
        if (allowButtonHold)
        {
            shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if(Input.GetKeyDown(KeyCode.R) && bulletsLeft < MagazineSize && !reloding) Reload();
        
        if(readyToShoot && shooting && !reloding && bulletsLeft <= 0) Reload();
        
        if (readyToShoot && shooting && !reloding && bulletsLeft > 0)
        {
            bulletsShot = 0;
            
            Shoot();
        }
    }

    private void Shoot()
    {
        readyToShoot = false;

        

        Ray ray = playerCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
            targetPoint = hit.point;
        else
        {
            targetPoint = ray.GetPoint(80);
        }

        Vector3 directionWithoutSpread = targetPoint - attackPoint.position;

        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        Vector3 directionWithSpread = directionWithoutSpread + new Vector3(x, y, 0);
        
        SpawnBulletRPC(directionWithoutSpread);
        
        bulletsLeft--;
        bulletsShot++;
        AmmoCountDisplay.text = bulletsLeft.ToString();

        if (allowInvoke)
        {
            Invoke("resetShot",timeBetweenShooting);
            allowInvoke = false;
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnBulletRPC(Vector3 directionWithSpread)
    {
        GameObject curentBullet = Instantiate(bullet, attackPoint.position, quaternion.identity);
        curentBullet.transform.forward = directionWithSpread.normalized;
        NetworkObject bulletnetworkOBJ = curentBullet.GetComponent<NetworkObject>();
        bulletnetworkOBJ.Spawn();
        curentBullet.GetComponent<Rigidbody>().AddForce(directionWithSpread.normalized * ShootForce, ForceMode.Impulse);
        curentBullet.GetComponent<bullet>().AddForceRPC(directionWithSpread,ShootForce);

    }

    private void resetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }

    private void Reload()
    {
        RelodingDisplay.SetActive(true);
        reloding = true;
        Invoke("ReloadFinished",realodeTime);
    }

    private void ReloadFinished()
    {
        bulletsLeft = MagazineSize;
        reloding = false;
        RelodingDisplay.SetActive(false);
        AmmoCountDisplay.text = bulletsLeft.ToString();

    }
}
