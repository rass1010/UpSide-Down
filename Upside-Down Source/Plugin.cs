using BepInEx;
using System;
using UnityEngine;
using Utilla;
using Photon.Pun;
using System.IO;
using System.Reflection;
using GravitySwitch;
using Bepinject;
using System.ComponentModel;

namespace GravitySwitch
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")] // Make sure to add Utilla 1.5.0 as a dependency!
    [ModdedGamemode]
    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;

        private string fileLocation = string.Format("{0}/SaveDataUpsidedown", Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
        private SaveDataUpsidedown saveData;

        public bool inAllowedRoom = false;

        public bool modEnabled = false;
        public float normalGravity;
        public Quaternion normalPlayerRotation;
        public Quaternion normalOnlinePlayerRotation;
        public GameObject onlineRig;
        public GameObject leftHand;
        public GameObject rightHand;

        void Awake()
        {
            HarmonyPatches.ApplyHarmonyPatches();

            instance = this;
            Zenjector.Install<ModInstaller>().OnProject();

            Utilla.Events.GameInitialized += GameInitialized;

            try
            {
                if (File.Exists(fileLocation))
                {
                    saveData = JsonUtility.FromJson<SaveDataUpsidedown>(File.ReadAllText(fileLocation));
                    modEnabled = saveData.enabled;
                }
                else
                {
                    saveData.enabled = modEnabled;
                }
            }
            catch
            {
                modEnabled = saveData.enabled;
            }

            
        }

        void Update()
        {
        }

        void FixedUpdate()
        {



            if (PhotonNetwork.InRoom && inAllowedRoom && modEnabled)
            {
                leftHand.transform.position = GorillaLocomotion.Player.Instance.leftHandTransform.position;
                rightHand.transform.position = GorillaLocomotion.Player.Instance.rightHandTransform.position;

                if (GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/gorilla") != null)
                {
                    HideOnlineRig();
                    HideOfflineRig();
                }
            }
        }


        public void ChangeGravity()
        {
            leftHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightHand = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            rightHand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            leftHand.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            Destroy(leftHand.GetComponent<SphereCollider>());
            Destroy(rightHand.GetComponent<SphereCollider>());
            Destroy(leftHand.GetComponent<Rigidbody>());
            Destroy(rightHand.GetComponent<Rigidbody>());
            Physics.gravity = new Vector3(0, Mathf.Abs(normalGravity), 0);
            GorillaLocomotion.Player.Instance.gameObject.transform.rotation = new Quaternion(normalPlayerRotation.x, normalPlayerRotation.y, normalPlayerRotation.z - 180, normalPlayerRotation.w);
        }

        public void RevertGravity()
        {
            Destroy(leftHand);
            Destroy(rightHand);
            Physics.gravity = new Vector3(0, normalGravity, 0);
            GorillaLocomotion.Player.Instance.gameObject.transform.rotation = normalPlayerRotation;
        }


        [ModdedGamemodeJoin]
        private void RoomJoined(string gamemode)
        {
            // The room is modded. Enable mod stuff.
            if (modEnabled)
            {
                ChangeGravity();
            }
            inAllowedRoom = true;
        }

        [ModdedGamemodeLeave]
        private void RoomLeft(string gamemode)
        {
            // The room was left. Disable mod stuff.
            RevertGravity();
            ShowOfflineRig();
            inAllowedRoom = false;
        }

        private void GameInitialized(object sender, EventArgs e)
        {
            normalPlayerRotation = new Quaternion(GorillaLocomotion.Player.Instance.gameObject.transform.rotation.x, GorillaLocomotion.Player.Instance.gameObject.transform.rotation.y, GorillaLocomotion.Player.Instance.gameObject.transform.rotation.z, GorillaLocomotion.Player.Instance.gameObject.transform.rotation.w);
            normalGravity = Physics.gravity.y;
        }

        public void HideOnlineRig()
        {
            GameObject online_face = GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/rig/body/head/gorillaface");
            GameObject online_body = GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/gorilla");
            GameObject online_chest = GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/rig/body/gorillachest");

            online_face.GetComponent<MeshRenderer>().forceRenderingOff = true;
            online_body.GetComponent<SkinnedMeshRenderer>().forceRenderingOff = true;
            online_chest.GetComponent<MeshRenderer>().forceRenderingOff = true;
        }

        public void HideOfflineRig()
        {
            GameObject offline_face = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/head/gorillaface");
            offline_face.GetComponent<Renderer>().forceRenderingOff = true;

            GameObject offline_body = GameObject.Find("OfflineVRRig/Actual Gorilla/gorilla");
            offline_body.GetComponent<SkinnedMeshRenderer>().forceRenderingOff = true;


            GameObject offline_chest = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/gorillachest");
            offline_chest.GetComponent<Renderer>().forceRenderingOff = true;

        }

        public void ShowOfflineRig()
        {
            GameObject offline_face = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/head/gorillaface");
            offline_face.GetComponent<Renderer>().forceRenderingOff = false;

            GameObject offline_body = GameObject.Find("OfflineVRRig/Actual Gorilla/gorilla");
            offline_body.GetComponent<SkinnedMeshRenderer>().forceRenderingOff = false;


            GameObject offline_chest = GameObject.Find("OfflineVRRig/Actual Gorilla/rig/body/gorillachest");
            offline_chest.GetComponent<Renderer>().forceRenderingOff = false;

        }

        public void ShowOnlineRig()
        {
            GameObject online_face = GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/rig/body/head/gorillaface");
            GameObject online_body = GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/gorilla");
            GameObject online_chest = GameObject.Find("GorillaParent/GorillaVRRigs/Gorilla Player Networked(Clone)/rig/body/gorillachest");

            online_face.GetComponent<MeshRenderer>().forceRenderingOff = false;
            online_body.GetComponent<SkinnedMeshRenderer>().forceRenderingOff = false;
            online_chest.GetComponent<MeshRenderer>().forceRenderingOff = false;
        }

        public void ToggleMod()
        {
            modEnabled = !modEnabled;

            saveData.enabled = modEnabled;
            File.WriteAllText(fileLocation, JsonUtility.ToJson(saveData));
        }

    }
}