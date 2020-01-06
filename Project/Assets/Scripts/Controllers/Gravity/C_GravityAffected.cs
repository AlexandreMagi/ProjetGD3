using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_GravityAffected : MonoBehaviour
{
    public bool isAirbone = false;
    float fTimePropel = .5f;
    float fElapsedTime = 0;

    Vector3 v3InitPos = Vector3.zero;

    float fSpinSpeed = 20;

    Rigidbody rbBody = null;

    // Start is called before the first frame update
    void Start()
    {
        fSpinSpeed += Random.Range(-5f, 5f);
        rbBody = this.GetComponent<Rigidbody>();
        fElapsedTime = 0;
        v3InitPos = this.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (isAirbone)
        {
            fElapsedTime += Time.deltaTime;

            if (fElapsedTime >= fTimePropel)
            {
                Spin();

                //Check si touche le sol
                fElapsedTime = 0;
                if (Physics.Raycast(this.transform.position, new Vector3(0, -1, 0), 1f))
                {
                    isAirbone = false;
                }
            }
        }

    }

    public void OnFloatActivation(float fGForce, float timeBeforeActivation, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {

        //Debug.Log("Woosh");
        rbBody.AddForce(new Vector3(0, fGForce, 0));
        this.isAirbone = true;

        StartCoroutine(Float(timeBeforeActivation, isSlowedDownOnFloat, tFloatTime, bIndependantFromTimeScale));

        Spin();
    }

    void DelockBody()
    {
        rbBody.constraints = RigidbodyConstraints.None;
    }

    void LockBody()
    {
        rbBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rbBody.useGravity = true;
    }

    public void RespawnItem()
    {
        StopCoroutine("Float");

        transform.position = v3InitPos;
        rbBody.angularVelocity = new Vector3();
        rbBody.velocity = new Vector3();
    }

    public void OnGravityBulletPull(Vector3 hitPoint, float pullForce)
    {
        StopCoroutine("Float");

        DelockBody();

        Vector3 v3DirectionToGo = (hitPoint - transform.position).normalized;
        Vector3 v3UpperAngle = isAirbone ? new Vector3(0, -.2f, 0) : new Vector3(0, .2f, 0);
        float fDistance = Vector3.Distance(hitPoint, transform.position);

        //Ça marche, c'est moche, mais on aime

        //En gros, ça attire en fonction de la distance par rapport au sol. Si la distance est inférieure à un seuil, on applique ce seuil au lieu de la distance. Pareil pour la distance max
        rbBody.AddForce(new Vector3(v3DirectionToGo.x, v3UpperAngle.y + (v3DirectionToGo.y / 2), v3DirectionToGo.z) * pullForce * (fDistance < .2f ? Mathf.Pow(2, 1.8f) : fDistance > 10 ? Mathf.Pow(5, 1.8f) : Mathf.Pow(3, 1.8f)));

    }

    void Spin()
    {
        Vector3 v3SpinRandom = new Vector3(
                        Random.Range(-1f, 1f) * fSpinSpeed,
                        Random.Range(-1f, 1f) * fSpinSpeed,
                        Random.Range(-1f, 1f) * fSpinSpeed
                    );

        rbBody.AddTorque(v3SpinRandom);
    }

    IEnumerator Float(float tTimeBeforeFloat, bool isSlowedDownOnFloat, float tFloatTime, bool bIndependantFromTimeScale)
    {

        yield return new WaitForSecondsRealtime(tTimeBeforeFloat);

        float tETime = 0;

        //Reset vélo avant de float
        if (isSlowedDownOnFloat)
            rbBody.velocity /= 50;

        while (true)
        {
            yield return new WaitForFixedUpdate();


            if (!bIndependantFromTimeScale)
                tETime += Time.fixedDeltaTime;
            else
                tETime += Time.fixedDeltaTime/Time.timeScale;

            //rbBody.AddForce(new Vector3(0, -rbBody.mass * (Physics.gravity.y*Time.timeScale), 0));
            rbBody.useGravity = false;

            if (tETime >= tFloatTime)
            {
                rbBody.useGravity = true;
                rbBody.AddForce(new Vector3(0, -2000, 0));
                if(GetComponent<C_Enemy>() != null) StartCoroutine(GetBackUp());
                yield break;
            }
        }

    }

    IEnumerator GetBackUp()
    {
        yield return new WaitForSecondsRealtime(.05f);

        rbBody.useGravity = false;

        float cumultime = 0f;

        while (cumultime < 1)
        {
            cumultime += 1f;

            yield return new WaitForSecondsRealtime(.05f);

            //transform.rotation = Quaternion.Lerp(this.transform.rotation, Quaternion.Euler(0, this.transform.rotation.y, 0), Time.time * getUpSpeed);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        LockBody();
        yield break;

    }

}
