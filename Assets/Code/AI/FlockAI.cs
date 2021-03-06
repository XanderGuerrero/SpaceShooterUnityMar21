using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockAI : MonoBehaviour
{
    private Vector3 direction;
    private Vector3 PreviousPosition;
    private Vector3 position;
    private float speed;

    [SerializeField]
    private Transform EnemyTransform;

    private Coroutine EnemyRotationCoroutine;


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
        //public Vector3 Direction
        //{
        //    set
        //    {
        //        direction = value;
        //    }
        //}

        //public Vector3 Position
        //{
        //    set
        //    {
        //        position = value;
        //        //PreviousPosition = position;
        //    }
        //}

        //public float Speed
        //{
        //    set
        //    {
        //        speed = value;
        //    }
        //}


        // Update is called once per frame
        //void Update()
        //{
        //    Debug.Log("hey Hi");
        //    //calculate the objects rotation
        //    float rot = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        //    float pitch = -Mathf.Asin(direction.y) * Mathf.Rad2Deg;
        //    Vector3 currentRotation = new Vector3(pitch, rot, 0);
        //    transform.rotation = Quaternion.Euler(currentRotation);
        //    //Vector3 pos = direction * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;
        //    transform.position += new Vector3(position.x, position.y, position.z) * speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime;
        //    //transform.position += Vector3.Lerp(position, position, speed * NetworkClient.SERVER_UPDATE_TIME * Time.deltaTime);
        //}

    }
}
