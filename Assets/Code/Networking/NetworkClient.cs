using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;
using System;
//using PE2D;


public class NetworkClient : SocketIOComponent
{
    public const float SERVER_UPDATE_TIME = 10;
    public static Action<SocketIOEvent> OnGameStateChange = (E) => { };

    [Header("Network Client")]
    [SerializeField]
    private Transform networkContainer;
    [SerializeField]
    private GameObject playerPrefab;
    [SerializeField]
    private ServerObjects serverSpawnables;
    //can get the id anywhere but cannot set it anywhere except this class
    public static string ClientID { get; private set; }
    string[] AsteroidPrefabNames = { "ASTEROID_AI", "ASTEROID_AI2", "ASTEROID_AI3" };
    public Dictionary<string, NetworkIdentity> serverObjects;


    // Start is called before the first frame update
    public override void Start()
    {
        //this calls the start method in the SocketIOComponent to connect
        base.Start();
        //used to clean up start
        initialize();
        setupEvents();
    }

    private void initialize()
    {
        //initialize the dictionary
        serverObjects = new Dictionary<string, NetworkIdentity>();
    }


    // Update is called once per frame
    public override void Update()
    {
        //this calls the Update method in the SocketIOComponent
        base.Update();

    }

    private void setupEvents()
    {
        On("open", (E) =>
        {
            Debug.Log("Connection made to the server");
        });


        On("register", (E) =>
        {
            ClientID = E.data["id"].ToString();
            ClientID = ClientID.Trim('"');
        });

        //the event 'spawn' or any event name must match what the node server
        //event name is*************
        On("spawn", (E) =>
        {
            //handles all spawning all players
            //passed data
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            GameObject go = Instantiate(playerPrefab, networkContainer);
            go.name = string.Format("Player({0})", id);
            //everything we spawn will have a network identity
            NetworkIdentity ni = go.GetComponent<NetworkIdentity>();
            ni.SetControllerID(id);
            if (ni.IsControlling() == false)
            {
                ni.GetComponentInChildren<Canvas>().enabled = false;
                ni.GetComponentInChildren<Light>().intensity = .5f;
            }
            ni.SetScoketReference(this);

            serverObjects.Add(id, ni);
            //Debug.Log("Connection Made! PLAYER: " + id + "has joined the game");
        });

        On("serverSpawn", (E) =>
        {

            //Debug.Log("about to fire1");
            string name = E.data["name"].str;
            name = name.Trim('"');
            //Debug.Log("name: "+ name);
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            //Debug.Log("about to fire2");
            float x = E.data["position"]["x"].f;
            //Debug.Log("X :" + x);
            float y = E.data["position"]["y"].f;
            //Debug.Log("Y :" + y);
            float z = E.data["position"]["z"].f;
            //Debug.Log("Z :" + z);
            //Debug.Log("server wants us to spawn a " + name);
            //Debug.Log("ID : " + id);

            if (!serverObjects.ContainsKey(id))
            {
                ServerObjectData sod = null;
                //Debug.Log(AsteroidPrefabNames[0]);
                if (name == AsteroidPrefabNames[0])
                {
                    System.Random r = new System.Random();
                    var rInt = r.Next(0, 3);
                    //Debug.Log(rInt);
                    sod = serverSpawnables.GetObjectByName(AsteroidPrefabNames[rInt]);
                }
                else
                {
                    sod = serverSpawnables.GetObjectByName(name);

                }

                var spawnedObject = Instantiate(sod.Prefab, networkContainer);
                //Debug.Log("about to fire4");
                //    //objectPooler.SpawnFromPool(name);
                //Debug.Log("Spawned: " + spawnedObject.name);
                //    //Debug.Log("Position " + new Vector3(x, y, z).ToString());
                spawnedObject.transform.position = new Vector3(x, y, z);
                //Debug.Log("Position " + spawnedObject.transform.position.ToString());
                var ni = spawnedObject.GetComponent<NetworkIdentity>();
                //Debug.Log("id " + id);
                ni.SetControllerID(id);
                //Debug.Log("Position " + spawnedObject.transform.position.ToString());
                ni.SetScoketReference(this);

                //Debug.Log("If NAME: " + name);
                //if bullet, apply directions as well
                if (name == "Bullet")
                //if (name == "Bullet(Clone)")
                {
                    spawnedObject.name = string.Format("{0}({1})", name, id);
                    //Debug.Log("about to fire5");
                    float directionX = E.data["direction"]["x"].f;
                    //Debug.Log("directionX: " + directionX);
                    float directionY = E.data["direction"]["y"].f;
                    //Debug.Log("directionY: " + directionY);
                    float directionZ = E.data["direction"]["z"].f;
                    //Debug.Log("directionZ: " + directionZ);
                    string activator = E.data["activator"].ToString();

                    activator = activator.Trim('"');
                    //Debug.Log("activator: " + activator);
                    float speed = E.data["speed"].f;
                    // Debug.Log("server spawn bullet speed " + speed);


                    //calculate rotation
                    float rot = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
                    //Debug.Log("float rot " + rot);
                    //Vector3 currentRotation = new Vector3(0, rot, 0);
                    //Debug.Log("currentRotation " + currentRotation);
                    float pitch = -Mathf.Asin(directionY) * Mathf.Rad2Deg;
                    Vector3 currentRotation = new Vector3(pitch, rot, 0);
                    spawnedObject.transform.rotation = Quaternion.Euler(currentRotation);
                    //Debug.Log("spawnedObject.transform.rotation " + spawnedObject.transform.rotation);

                    WhoActivateMe whoActivateMe = spawnedObject.GetComponent<WhoActivateMe>();
                    whoActivateMe.SetActivator(activator);

                    Projectile projectile = spawnedObject.GetComponent<Projectile>();
                    projectile.Direction = new Vector3(directionX, directionY, directionZ);
                    projectile.Speed = speed;
                }
                //    //if Asteroid1, apply tumble and  as well
                if (name == AsteroidPrefabNames[0] || name == AsteroidPrefabNames[1] || name == AsteroidPrefabNames[2])
                {
                    //Debug.Log("If NAME entered!!!!!!: " + name);
                    //Debug.Log("tumble!!!!!!: " + E.data.ToString());
                    //string activator = E.data["activator"].ToString();
                    float tumble = E.data["tumble"].f;
                    //Random.InitState(10);
                    //Debug.Log("activator: " + activator);
                    spawnedObject.name = string.Format("{0}({1})", AsteroidPrefabNames[0], id);
                    ni.gameObject.name = string.Format("{0}({1})", AsteroidPrefabNames[0], id);
                    //Debug.Log("spawnedObject.name: " + spawnedObject.name);
                    //Debug.Log("ni.gameObject.name: " + ni.gameObject.name);
                    //spawnedObject.name = string.Format("{0}({1})", name, id);
                    spawnedObject.GetComponent<AsteroidAI>().tumble = tumble;
                    
                    //spawnedObject.transform.rotation = Quaternion.Euler(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                    spawnedObject.transform.localScale = new Vector3(E.data["scale"]["x"].f.TwoDecimals(), E.data["scale"]["y"].f.TwoDecimals(), E.data["scale"]["z"].f.TwoDecimals());
                    //WhoActivateMe whoActivateMe = spawnedObject.GetComponent<WhoActivateMe>();
                    //whoActivateMe.SetActivator(activator);
                    //spawnedObject.AddComponent<BeltObject>().SetupAsteroidBeltObject(Random.Range(1, 3f), Random.Range(1, 1), Planet, true);

                }               
                if (name == "AI_Base" || name == "ENEMY_AI")
                {
                    spawnedObject.name = string.Format("{0}({1})", name, id);
                    //Debug.Log("spawnedObject.name: " + spawnedObject.name);
                    spawnedObject.GetComponent<AIManager>();
                }
                if (name == "FLOCK_AI")
                {
                    spawnedObject.name = string.Format("{0}({1})", name, id);
                    spawnedObject.GetComponent<FlockAI>();
                    //Debug.Log(spawnedObject.GetType().ToString());
                }
                if (name == "Missile")
                {
                    spawnedObject.name = string.Format("{0}({1})", name, id);
                    //Debug.Log("about to fire5");
                    float targetx = E.data["target"]["x"].f;
                    //Debug.Log("X :" + x);
                    float targety = E.data["target"]["y"].f;
                    //Debug.Log("Y :" + y);
                    float targetz = E.data["target"]["z"].f;

                    float directionX = E.data["direction"]["x"].f;
                    //Debug.Log("directionX: " + directionX);
                    float directionY = E.data["direction"]["y"].f;
                    //Debug.Log("directionY: " + directionY);
                    float directionZ = E.data["direction"]["z"].f;
                    //Debug.Log("directionZ: " + directionZ);
                    string activator = E.data["activator"].ToString();

                    activator = activator.Trim('"');
                    //Debug.Log("missile activator: " + activator);
                    float speed = E.data["speed"].f;
                    string targetId = E.data["targetId"].ToString();
                    targetId = targetId.Trim('"');
                    //Debug.Log("server spawn missile target " + targetId);


                    //calculate rotation
                    float rot = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
                    //Debug.Log("float rot " + rot);
                    //Vector3 currentRotation = new Vector3(0, rot, 0);
                    //Debug.Log("currentRotation " + currentRotation);
                    float pitch = -Mathf.Asin(directionY) * Mathf.Rad2Deg;
                    Vector3 currentRotation = new Vector3(pitch, rot, 0);
                    spawnedObject.transform.rotation = Quaternion.Euler(currentRotation);
                    //Debug.Log("spawnedObject.transform.rotation " + spawnedObject.transform.rotation);

                    WhoActivateMe whoActivateMe = spawnedObject.GetComponent<WhoActivateMe>();
                    whoActivateMe.SetActivator(activator);
                    //spawnedObject.AddComponent<AIManager>();
                    //spawnedObject.AddComponent<Missile>();
                    //Missile missile = spawnedObject.GetComponent<Missile>();
                    //missile.targetId = targetId;
                    Missile missile = ni.GetComponent<Missile>();
                    missile.targetId = targetId;
                    missile.Speed = speed;
                    missile.missilePosition = new Vector3(x, y, z);
                    missile.rocketTarget = new Vector3(targetx, targety, targetz);
                    //missile.Direction = new Vector3(directionX, directionY, directionZ);
                    //missile.Speed = speed;
                    //if (serverObjects.ContainsKey(targetId))
                    //{
                    //    //Debug.Log("this Enemy target DOES exist");
                    //    missile.RocketTarget = serverObjects[targetId].transform;
                    //    missile.RocketTarget.position = new Vector3(targetx, targety, targetz); ;
                    //}
                    //else
                    //{
                    //    missile.RocketTarget = serverObjects[targetId].transform;
                    //    missile.RocketTarget.position = new Vector3(targetx, targety, targetz); ;
                    //    //Debug.Log("this missile target doesnt exist");
                    //    // }
                    //    //Projectile projectile = spawnedObject.GetComponent<Projectile>();
                    //    //projectile.Direction = new Vector3(directionX, directionY, directionZ);
                    //    //projectile.Speed = speed;
                    //}


                }
                serverObjects.Add(id, ni);
            }
           
        });

        On("serverSpawnExplosion", (E) =>
        {
            //Debug.Log("serverSpawnExplosion - explosion TIME ");
            //Debug.Log("about to fire1");
            string name = E.data["name"].str;
            name = name.Trim('"');
            //Debug.Log("NAME :" + name);
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            //Debug.Log("about to fire2");
            float x = E.data["position"]["x"].f;
            //Debug.Log("X :" + x);
            float y = E.data["position"]["y"].f;
            //Debug.Log("Y :" + y);
            float z = E.data["position"]["z"].f;
            //Debug.Log("Z :" + z);
            //Debug.Log("server wants us to spawn a " + name);
            //float speed = E.data["speed"].f;
            //Debug.Log("server spawn bullet speed " + speed);

            //make sure object is not already spawned into the game
            if (!serverObjects.ContainsKey(id))
            {

                 //                float speedOffset = .01f;
                 //float lengthMultiplier = 40f;
                 //int numToSpawn = 200;
                 //WrapAroundType wrapAround;
                //Debug.Log("about to explode!!!!!!!!!!!!!!!!");
                //SpawnExplosion(Camera.main.ScreenToWorldPoint(Input.mousePosition));
                ServerObjectData sod = serverSpawnables.GetObjectByName(name);

                //float hue1 = UnityEngine.Random.Range(0, 6);
                //float hue2 = (hue1 + UnityEngine.Random.Range(0, 2)) % 6f;
                //Color colour1 = StaticExtensions.Color.FromHSV(hue1, 0.5f, 1);
                //Color colour2 = StaticExtensions.Color.FromHSV(hue2, 0.5f, 1);

                //for (int i = 0; i < numToSpawn; i++)
                //{
                //    float speed = (18f * (1f - 1 / UnityEngine.Random.Range(1f, 10f))) * speedOffset;

                //    var state = new ParticleBuilder()
                //    {
                //        velocity = StaticExtensions.Random.RandomVector2(speed, speed),
                //        wrapAroundType = WrapAroundType.None,
                //        lengthMultiplier = lengthMultiplier,
                //        velocityDampModifier = 0.94f,
                //        removeWhenAlphaReachesThreshold = true
                //    };

                //    var colour = Color.Lerp(colour1, colour2, UnityEngine.Random.Range(0, 1));

                //    float duration = 320f;
                //    var initialScale = new Vector3(2f, 1f, 1f);


                //    sod.Prefab = ParticleFactory.instance.CreateParticle(new Vector3(x, y, z), colour, duration, initialScale, state);
                //}


                var spawnedObject = Instantiate(sod.Prefab, networkContainer);
                spawnedObject.transform.position = new Vector3(x, y, z);
                var ni = spawnedObject.GetComponent<NetworkIdentity>();
                ni.SetControllerID(id);
                ni.SetScoketReference(this);
                //add the obj to the dictinary of objs
                serverObjects.Add(id, ni);

                //DemoMouseController Explosion = spawnedObject.GetComponent<DemoMouseController>();
                //Explosion.Position = new Vector3(x, y, z);
                //Debug.Log("serverSpawnExplosion - explosion TIME ");
                //Explosion.SpawnExplosion(new Vector3(x, y, z));
            }
        });


        On("serverUnspawn", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            NetworkIdentity ni = serverObjects[id];
            serverObjects.Remove(id);
            //Debug.Log("serverUnspawn - explosion  ");
            DestroyImmediate(ni.gameObject);
        });

        On("disconnected", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            //Debug.LogFormat("player ({0})", id);
            GameObject go = serverObjects[id].gameObject;

            //Debug.LogFormat("player ({0}) has left the game", id);
            Destroy(go);//remove from game
            serverObjects.Remove(id);//remove from dictionary "memory"
        });


        On("updatePosition", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            //Debug.LogFormat("Data back to the client ID values: ({0}) ", id);
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float z = E.data["position"]["z"].f;
            //Debug.LogFormat("Data back to the client X values: ({0}) ", x);
            //Debug.LogFormat("Data back to the client Y values: ({0}) ", y);
            //Debug.LogFormat("Data back to the client Z values: ({0}) ", z);
            // Debug.LogFormat("Data back to the client X values: ({0}) ", x);

            //search the doctionary for the id we got from server when we registered a connection
            //and assign the id value to the network
            //identity of that gameobject
            NetworkIdentity ni = serverObjects[id];
            ni.transform.position = new Vector3(x, y, z);
        });

        On("updateRotation", (E) =>
        {
            //Debug.Log("Got Data back, ROTATION : ({0}) " + E.data);

            string id = E.data["id"].ToString();
            id = id.Trim('"');

            float barrelRotation = E.data["barrelRotation"].f;
            // Debug.Log("barrel rotation value: " + barrelRotation);
            float ShipTilt = E.data["shipTiltRotation"].f;
            float shipTiltX = E.data["shipTiltRotationX"].f;
            float shipTiltY = E.data["shipTiltRotationY"].f;

            //Debug.LogFormat("Data back to the client rotation values: ({0}) ", barrelRotation);
            NetworkIdentity ni = serverObjects[id];
            ni.GetComponent<Rigidbody>().rotation = Quaternion.Euler(shipTiltX, shipTiltY, ShipTilt);
            //ni.GetComponent<Rigidbody>().rotation = Quaternion.Euler(shipTiltX, shipTiltY, ShipTilt);
            //ni.transform.localEulerAngles = new Vector3(shipTiltX, shipTiltY, shipTilt);
            //ni.GetComponent<PlayerManager>().SetRotation(barrelRotation);


        });

        //On("updateShipTilt", (E) =>
        //{
        //    //Debug.Log("Got Data back, Tilt : {0} " + E.data);

        //    string id = E.data["id"].ToString();
        //    id = id.Trim('"');

        //    float ShipTilt = E.data["zTiltValue"].f;


        //    //Debug.LogFormat(id + " Data back to the client rotation values: {0} ", ShipTilt);
        //    NetworkIdentity ni = serverObjects[id];

        //    //calls the set method in player manager class to set its z rotation angle
        //    //ni.transform.localEulerAngles = new Vector3(0.0f, 0.0f, ShipTilt);
        //    ni.GetComponent<Rigidbody>().rotation = Quaternion.Euler(0f, 0f, ShipTilt);
        //    // ni.GetComponent<GameObject>();
        //});


        On("updateAI_Rotation", (E) =>
        {
            //Debug.Log("Got AI Data back, ROTATION : ({0}) " + E.data);

            string id = E.data["id"].ToString();
            id = id.Trim('"');

            //float barrelRotation = E.data["barrelRotation"].f;
            // Debug.Log("barrel rotation value: " + barrelRotation);
            float shipTilt = E.data["shipTiltRotation"].f;
            float shipTiltX = E.data["shipTiltRotationX"].f;
            float shipTiltY = E.data["shipTiltRotationY"].f;

            //Debug.LogFormat("Data back to the client rotation values: ({0}) ", barrelRotation);
            NetworkIdentity ni = serverObjects[id];
            if (ni.gameObject.activeInHierarchy)
            {
                //ni.transform.position = new Vector3(x, y, z);
                //Debug.Log("ABOUT TO UPDATE THE AI POSITION, DATA : ({0}) " + E.data);
                //StartCoroutine(AIPositionSmoothing(ni.transform, new Vector3(x, y, z)));

                //ni.transform.position = Vector3.Lerp(ni.transform.position, new Vector3(shipTiltX, y, z), 10f * Time.deltaTime);
                //ni.transform.localEulerAngles = new Vector3(shipTiltX, shipTiltY, shipTilt);
                //ni.GetComponent<AIManager>().SetBarrelRotation(barrelRotation);
                //ni.GetComponent<FlockAI>().SetEnemyShipRotation(shipTiltX, shipTiltY, shipTilt);
                //FlockAI AI = ni.GetComponent<FlockAI>();
                //AI.SetEnemyShipRotation(shipTiltX, shipTiltY, shipTilt);

                ni.transform.localEulerAngles = new Vector3(shipTiltX, shipTiltY, shipTilt);

            }
            //ni.transform.localEulerAngles = new Vector3(shipTiltX, shipTiltY, shipTilt);
            //ni.GetComponent<PlayerManager>().SetRotation(barrelRotation);


        });

        On("UpdateAI", (E) =>
        {
           //Debug.Log("Got Data back, ROTATION :  " + E.data);
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float z = E.data["position"]["z"].f;

            //Debug.Log("about to fire5");
            float directionX = E.data["direction"]["x"].f;
            float directionY = E.data["direction"]["y"].f;
            float directionZ = E.data["direction"]["z"].f;

            float speed = E.data["speed"].f;
            //Debug.Log("server spawn bullet speed " + speed);         
            NetworkIdentity ni = serverObjects[id];
          
            if (ni.gameObject.activeInHierarchy)
            {
                //calculate rotation
                float rot = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
                float pitch = -Mathf.Asin(directionY) * Mathf.Rad2Deg;
                //Vector3 currentRotation = new Vector3(pitch, rot, 0);
                //ni.transform.rotation = Quaternion.Euler(currentRotation);
                //Debug.Log(ni.gameObject.name);
                StartCoroutine(AIPositionSmoothing(ni.transform, new Vector3(x, y, z)));
                if (ni.gameObject.name.Contains("FLOCK_AI"))
                {
                    FlockAI Flock_AI = ni.GetComponent<FlockAI>();
                    Flock_AI.SetEnemyShipRotation(rot, pitch);
                    //Debug.Log("AI direction is: " + AI.Direction + " speed: " + speed);
                }
                if (ni.gameObject.name.Contains("ASTEROID_AI"))
                {
                    AsteroidAI asteroid = ni.GetComponent<AsteroidAI>();
                }
                else
                {

                }
            }
        });
        

        On("UpdatePlayerCrosshair", (E) =>
        {
            //Debug.Log("Got Data back, ROTATION :  " + E.data);
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float z = E.data["position"]["z"].f;

            //Debug.Log("about to fire5");
            float directionX = E.data["direction"]["x"].f;
            float directionY = E.data["direction"]["y"].f;
            float directionZ = E.data["direction"]["z"].f;

            //float speed = E.data["speed"].f;
            //Debug.Log("server spawn bullet speed " + speed);         
            NetworkIdentity ni = serverObjects[id];

            if (ni.gameObject.activeInHierarchy)
            {
                //calculate rotation
                float rot = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
                float pitch = -Mathf.Asin(directionY) * Mathf.Rad2Deg;

                StartCoroutine(AIPositionSmoothing(ni.transform, new Vector3(x, y, z)));
                if (ni.gameObject.name.Contains("FLOCK_AI"))
                {
                    Crosshair Corsshair = ni.GetComponent<Crosshair>();
                    Corsshair.SetUp(new Vector3(x, y, z), new Vector3(directionX, directionY, directionZ));
                    //Debug.Log("AI direction is: " + AI.Direction + " speed: " + speed);
                }          
            }
        });

        On("UpdateMissileAI", (E) =>
        {
            //Debug.Log("Got Data back, ROTATION : ({0}) " + E.data);

            string id = E.data["id"].ToString();
            id = id.Trim('"');
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float z = E.data["position"]["z"].f;
            //spawnedObject.name = string.Format("{0}({1})", name, id);
            //Debug.Log("about to fire5");
            float targetPositionx = E.data["targetPosition"]["x"].f;
            float targetPositiony = E.data["targetPosition"]["y"].f;
            float targetPositionz = E.data["targetPosition"]["z"].f;
            float directionX = E.data["direction"]["x"].f;
            float directionY = E.data["direction"]["y"].f;
            float directionZ = E.data["direction"]["z"].f;
            //string activator = E.data["activator"].ToString();
            //activator = activator.Trim('"');
            float speed = E.data["speed"].f;
            string targetId = E.data["targetId"].ToString();
            targetId = targetId.Trim('"');
            //Debug.Log("server spawn bullet speed " + speed);
            //float speed = E.data["speed"].f;
            //float barrelRotation = E.data["barrelRotation"].f;
            //Debug.Log("barrel rotation value: " + barrelRotation);
            //float shipTilt = E.data["shipTiltRotation"].f;
            //float shipTiltX = E.data["shipTiltRotationX"].f;
            //float shipTiltY = E.data["shipTiltRotationY"].f;
            NetworkIdentity ni = serverObjects[id];
            if (ni.gameObject.activeInHierarchy)
            {
                //Debug.Log("ABOUT TO UPDATE THE AI POSITION, DATA : ({0}) " + E.data);
                //ni.GetComponent<FlockAI>().Position = new Vector3(x, y, z);
                //ni.GetComponent<FlockAI>().Speed = speed;
                //ni.transform.position = new Vector3(x, y, z);
                //StartCoroutine(AIPositionSmoothing(ni.transform, new Vector3(x, y, z)));
                //ni.transform.position = Vector3.Lerp(ni.transform.position, new Vector3(x, y, z), 10f * Time.deltaTime);
                //ni.transform.localEulerAngles = new Vector3(shipTiltX, shipTiltY, shipTilt);
                //ni.GetComponent<AIManager>().SetBarrelRotation(barrelRotation);
                //ni.GetComponent<AIManager>().SetEnemyShipRotation(shipTiltX, shipTiltY, shipTilt);
                //calculate rotation
                //float rot = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
                //float pitch = -Mathf.Asin(directionY) * Mathf.Rad2Deg;
                //Vector3 currentRotation = new Vector3(pitch, rot, 0);
                //ni.transform.rotation = Quaternion.Euler(currentRotation);

                Missile missile = ni.GetComponent<Missile>();
                missile.targetId = targetId;
                missile.Speed = speed;
                missile.missilePosition = new Vector3(x, y, z);
                missile.rocketTarget = new Vector3(targetPositionx, targetPositiony, targetPositionz);
                //Projectile projectile = ni.GetComponent<Projectile>();
                //projectile.Direction = new Vector3(directionX, directionY, directionZ);
                //projectile.Speed = speed;

                //Debug.Log("missile.rocketTarget position is: " + missile.rocketTarget + " speed: " + speed);
            }
        });


        On("playerDied", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            NetworkIdentity ni = serverObjects[id];
            if (ni.GetComponent<AIManager>())
            {
                ni.GetComponent<AIManager>().StopCoroutines();
            }
            ni.gameObject.SetActive(false);
        });

        On("playerRespawn", (E) =>
        {
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            float x = E.data["position"]["x"].f;
            float y = E.data["position"]["y"].f;
            float z = E.data["position"]["z"].f;
            NetworkIdentity ni = serverObjects[id];
            ni.transform.position = new Vector3(x, y, z);
            ni.gameObject.SetActive(true);
        });

        On("updatePlayerHealth", (E) =>
        {
            //Debug.Log("Got health data back: " + E.data);
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            float health = E.data["health"].f;
            
            NetworkIdentity ni = serverObjects[id];
            ni.GetComponent<HealthBar>().SetHealth(health);
         
        });

        On("updatePlayerScore", (E) =>
        {
            //Debug.Log("Got score data back: " + E.data);
            string id = E.data["id"].ToString();
            id = id.Trim('"');
            float score = E.data["score"].f;
            NetworkIdentity ni = serverObjects[id];
            ni.GetComponent<PlayerScore>().SetPlayerScore(score); 
            //ni.GetComponent<DamagePopUp>().SetUpPopUp(score);


        });

        On("displayPlayerPointsOnScreen", (E) =>
        {
            //Debug.Log("Got points data back: " + E.data);
            string id = E.data["id"].ToString();
         
            id = id.Trim('"');
         
            float score = E.data["score"].f;
       
            float x = E.data["position"]["x"].f;
          
            float y = E.data["position"]["y"].f;
      
            float z = E.data["position"]["z"].f;

            //NetworkIdentity ni = serverObjects[id];
            var canvas = networkContainer.transform.Find("Canvas");
            ServerObjectData sod = serverSpawnables.GetObjectByName("DamagePointsPopUp");
            
            var spawnedObject = Instantiate(sod.Prefab, networkContainer.GetComponentInChildren<Canvas>().transform);
            //spawnedObject.transform.parent = ;
            //spawnedObject.transform.SetParent(canvas);
            //spawnedObject.transform.position = new Vector3(x, y, z);

            //spawnedObject.GetComponent<DamagePopUp>().SetUpPopUp(score, new Vector3(x, y, z));
            DamagePopUp points = spawnedObject.GetComponent<DamagePopUp>();
            points.SetUpPopUp(score, new Vector3(x, y, z));
            //Debug.Log("HI ");

        });

        On("loadGame", (E) =>
        {
            Debug.Log(message: "Switching to game");
            SceneManagementManager.Instance.LoadLevel(SceneList.LEVEL, onLevelLoaded: (levelName) => {
                SceneManagementManager.Instance.UnLoadLevel(SceneList.MAIN_MENU);
            });
        });

        On("lobbyUpdate", (E) =>
        {
            OnGameStateChange.Invoke(E);
        });


        //public override void Awake()
        //{
        //    //this calls the Awake method in the SocketIOComponent
        //    base.Awake();

        //}
    }

    public void AttemptToJoinLobby()
    {
        Emit("joinGame");
    }

    private IEnumerator AIPositionSmoothing(Transform AiTransform, Vector3 goalPosition)
    {
        float count = 0.1f;//make sure to sync this with the server ai_base.speed
        float currentTime = 0.0f;
        Vector3 startPositiion = AiTransform.position;

        while (currentTime < count)
        {
            currentTime += Time.deltaTime;

            if (currentTime < count)
            {
                AiTransform.position = Vector3.Lerp(startPositiion, goalPosition, currentTime / count);
            }
            yield return new WaitForEndOfFrame();

            if (AiTransform == null)
            {
                currentTime = count;
                yield return null;
            }

        }
        yield return null;
    }
}

//its serilizable so I can send as a data over socket
[Serializable]
public class Player
{
    public string id;
    public Position position;
    //public PlayerRotation rotation;
    //public PlayerRotation shipTilt;
}

//to send asteroid data back to the server to use to update the other players screens


[Serializable]
public class Position
{
    public float x;
    public float y;
    public float z;
}

[Serializable]
public class PlayerTilt
{
    public float zValueForTilt;

}

[Serializable]
public class BulletData
{
    public string id;
    public string activator;
    public Position position;
    public Position direction;
}

[Serializable]
public class MissileData
{
    public string id;
    public string targetId;
    public string activator;
    public Position target;
    public Position position;
    public Position direction;
}

[Serializable]
public class IdData
{
    public string id;
    public string activator;
    public string ObjCollidedWith;
    public float x;
    public float y;
    public float z;
    public Position direction;
}

[Serializable]
public class ExplosionData
{
    public string id;
    public string activator;
    public Position position;
    public string bulletActivatorID;
}

[Serializable]
public class PlayerRotation
{
    public float barrelRotation;
    public float shipTiltRotation;
    public float shipTiltRotationX;
    public float shipTiltRotationY;
}