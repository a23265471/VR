﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRStandardAssets.Utils;
using UnityEngine.VR;
public class ShootingGunController : MonoBehaviour
{
    public VRInput vrInput;//using VRStandardAssets.Utils;
    public AudioSource audiosource;
    public ParticleSystem flareParticle;
    public LineRenderer gunFlare;
    public Transform gunEnd;

    public Transform cameraTransform;
    public Reticle reticle;//using VRStandardAssets.Utils;
    public Transform gunContainer;
    public float damping = 0.5f;
    public float dampingCoef=-20f;
    public float gunContainerSmooth = 10f;

    public float defaulLineLength;
    public float gunFlareVisibleSeconds = 0.07f;

    private void OnEnable()
    {
        vrInput.OnDown += HandleDown;


    }

    private void OnDisable()
    {
        vrInput.OnDown -= HandleDown;
    }

    private void HandleDown()
    {
        StartCoroutine(Fire());
    }

    private IEnumerator Fire()
    {
        audiosource.Play();
        float lineLength = defaulLineLength;
        //TODO判斷有無射到東西
        flareParticle.Play();
        gunFlare.enabled = true;
        yield return StartCoroutine(MoveLineRender(lineLength));
        gunFlare.enabled = false;

    }
    private IEnumerator MoveLineRender(float lineLength)
    {
        float timer = 0f;
        while (timer < gunFlareVisibleSeconds)
        {
            gunFlare.SetPosition(0, gunEnd.position);//0為射線初始
            gunFlare.SetPosition(1, gunEnd.position + gunEnd.forward * lineLength);//1為射線終點
            yield return null;
            timer += Time.deltaTime;
        }
    }
    private void Update()
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, InputTracking.GetLocalRotation(VRNode.Head), damping * (1 - Mathf.Exp(dampingCoef * Time.deltaTime)));
        transform.position = cameraTransform.position;
        Quaternion lookATRotation = Quaternion.LookRotation(reticle.ReticleTransform.position - gunContainer.position);
        gunContainer.rotation = Quaternion.Slerp(gunContainer.rotation, lookATRotation, gunContainerSmooth * Time.deltaTime);
    }
}
