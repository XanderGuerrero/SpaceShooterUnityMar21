using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidAI : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 PreviousPosition;
    private Vector3 position;
    private float speed;
    public float tumble;
    [SerializeField]
    private Transform EnemyTransform;
    private Coroutine EnemyRotationCoroutine;
    private Rigidbody rb;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.angularVelocity = Random.insideUnitSphere * tumble * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;
    }

    //void Update()
    //{
    //    rb.angularVelocity = Random.insideUnitSphere * tumble * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;

    //}

    public void SetEnemyShipRotation(float rotation, float pitch)
    {
        //Debug.Log("rotate the anitmation");
        //StartCoroutine(AnimateEnemyShipTurn(EnemyTransform.localEulerAngles.x, valueX, EnemyTransform.localEulerAngles.y, valueY, EnemyTransform.localEulerAngles.z, valueZ));
        EnemyRotationCoroutine = StartCoroutine(AnimateFlockShipTurn(EnemyTransform.localEulerAngles, rotation, pitch));
        //EnemyTransform.localEulerAngles = new Vector3(x: valueX, y: valueY, z: valueZ);
    }

    public void StopCoroutines()
    {
        if (EnemyRotationCoroutine != null)
        {
            StopCoroutine(EnemyRotationCoroutine);
        }
    }

    private IEnumerator AnimateFlockShipTurn(Vector3 startRotation, float goalRotationOnY, float goalRotationOnX)
    {
        {

            float count = 0.1f; //In sync with server update
            float currentTime = 0.0f;

            while (currentTime < count)
            {
                currentTime += Time.deltaTime;

                if (currentTime < count)
                {
                    EnemyTransform.localEulerAngles = new Vector3(Mathf.LerpAngle(startRotation.x, goalRotationOnX, currentTime / count), Mathf.LerpAngle(startRotation.y, goalRotationOnY, currentTime / count), 0);
                }

                yield return new WaitForEndOfFrame();

                if (EnemyTransform == null)
                {
                    currentTime = count;
                    yield return null;
                }
            }

            yield return null;
        }
    }
}
