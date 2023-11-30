using System.Collections.Generic;
using BrutalEvent.Models;
using Unity.Netcode;
using UnityEngine;

namespace BrutalEvent.MonoBehaviours
{
    // TODO Review this file and update to your own requirements, or remove it altogether if not required
    /// <summary>
    /// Template MonoBehaviour class. Use this to add new functionality and behaviours to
    /// the game.
    /// </summary>
    public class QuotaAjuster : MonoBehaviour
    {
        public TimeOfDay TOD;
        public float messageTimer = 10f;
        public static int surpriseLandmines;
        public static GameObject landmine;
        public static GameObject turret;
        public static List<GameObject> objectsToCleanUp = new List<GameObject>();
        public static float slSpawnTimer;
        public static bool shouldSpawnTurret;

        /// <summary>
        /// Unity Awake method.
        /// </summary>
        public void Awake()
        {
            Configuration.mls.LogWarning("Changing quota variables");
        }

        /// <summary>
        /// Unity Start method
        /// </summary>
        public void Start()
        {

        }

        /// <summary>
        /// Unity Awake method. Runs every frame so remove this if not required.
        /// Runs frequently, so remove if not required.
        /// </summary>
        public void Update()
        {
            if (TOD == null)
            {
                TOD = FindFirstObjectByType<TimeOfDay>();
            }
            else
            {
                TOD.quotaVariables.baseIncrease = 275f;
            }
            if (StartOfRound.Instance != null && StartOfRound.Instance.shipIsLeaving)
            {
                surpriseLandmines = -1;
            }
            if (surpriseLandmines > 0)
            {
                //if (slSpawnTimer > 0f)
                //{
                //    slSpawnTimer = UnityEngine.Random.Range(-4, -1);

                //    PlayerControllerB[] allPlayerScripts = StartOfRound.Instance.allPlayerScripts;
                //    PlayerControllerB playerControllerB = allPlayerScripts[
                //        UnityEngine.Random.Range(0, allPlayerScripts.Length)];

                //    if (playerControllerB != null)
                //    {
                //        if (Vector3.Distance(playerControllerB.transform.position, 
                //                new Vector3(9.33f, 5.2f, 1021f)) < 1f)
                //        {
                //            slSpawnTimer = 0.9f;
                //            return;
                //        }

                //        var gameObject = Instantiate(landmine, playerControllerB.transform.position, Quaternion.identity);

                //        gameObject.transform.position = playerControllerB.transform.position;
                //        EventConfiguration.mls.LogWarning(playerControllerB.transform.position);
                //        gameObject.GetComponent<NetworkObject>().Spawn(true);
                //        objectsToCleanUp.Add(gameObject);
                //    }
                //}
                //else
                //{
                //    slSpawnTimer += Time.deltaTime;
                //}
            }
            if (shouldSpawnTurret & turret != null)
            {
                shouldSpawnTurret = false;
                //GameObject gameObject2 = Instantiate(turret, new Vector3(3.87f, 0.84f, -14.23f), Quaternion.identity);

                //gameObject2.transform.position = new Vector3(3.87f, 0.84f, -14.23f);
                //gameObject2.transform.forward = new Vector3(1f, 0f, 0f);
                //gameObject2.GetComponent<NetworkObject>().Spawn(true);
                //objectsToCleanUp.Add(gameObject2);
            }
        }

        public static void CleanupAllSpawns()
        {
            foreach (GameObject gameObject in objectsToCleanUp)
            {
                if (gameObject != null)
                {
                    gameObject.GetComponent<NetworkObject>().Despawn(true);
                }
            }
            objectsToCleanUp.Clear();
        }

        /// <summary>
        /// Unity Physics Update (LateUpdate) method.
        /// Runs frequently, so remove if not required.
        /// </summary>
        public void LateUpdate()
        {

        }
    }
}
