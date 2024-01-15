using BepInEx.Configuration;
using BepInEx.Logging;
using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AoTGiants.Patches
{
    [HarmonyPatch(typeof(ForestGiantAI))]
    internal class ForestGiantAIPatch
    {
        private static ManualLogSource logger = BepInEx.Logging.Logger.CreateLogSource("Flowprojects.AoTGiants.ForestGiantAIPatch");
        private static AudioSource aotSFX;
        private static string sfxVolume = AotGiantsBase.configVolume.Value;

        [HarmonyPatch("GiantSeePlayerEffect")]
        [HarmonyPrefix]
        private static void forestGiantAoTAudioPatch(ForestGiantAI __instance)
        {
            aotSFX = GameNetworkManager.Instance.localPlayerController.itemAudio;
            aotSFX.volume = AotGiantsBase.getVolume(sfxVolume);

            bool __lostPlayerInChase = false;
            if (__lostPlayerInChase == false)
            {
                __lostPlayerInChase = (bool)Traverse.Create(__instance).Field("lostPlayerInChase").GetValue();
            }
            
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead || GameNetworkManager.Instance.localPlayerController.isInsideFactory)
            {
                return;
            }

            if (__instance.currentBehaviourStateIndex == 1 && __instance.chasingPlayer == GameNetworkManager.Instance.localPlayerController && !__lostPlayerInChase)
            {
                GameNetworkManager.Instance.localPlayerController.IncreaseFearLevelOverTime(1.4f);
                
                if(!aotSFX.isPlaying)
                {
                    playRandomTitanAggroClip(aotSFX);
                }
                return;
            }

            bool flag = false;
            if (!GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom && __instance.HasLineOfSightToPosition(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, 45f, 70))
            {
                if (Vector3.Distance(((Component)(object)__instance).transform.position, ((Component)(object)GameNetworkManager.Instance.localPlayerController).transform.position) < 15f)
                {
                    GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(0.7f);
                }
                else
                {
                    GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(0.4f);
                }
            }

            //if player is lost the sfx stops
            if(__lostPlayerInChase == true)
            {
                aotSFX.Stop();
                logger.LogInfo("Stopped audio clip");
            }
        }


        private static void playRandomTitanAggroClip(AudioSource __audioSource)
        {
            AudioSource audioSource = __audioSource;
            int audioClipIndex = UnityEngine.Random.Range(0, AotGiantsBase.SoundFX.Count);
            audioSource.PlayOneShot(AotGiantsBase.SoundFX[audioClipIndex]);
            logger.LogInfo("Played audio clip " + audioClipIndex + " with volume " + aotSFX.volume);
        }

        //Original method code (v47 / v49)
        //Plugin might need adjusting
        /* 
        private void GiantSeePlayerEffect()
        {
            if (GameNetworkManager.Instance.localPlayerController.isPlayerDead || GameNetworkManager.Instance.localPlayerController.isInsideFactory)
            {
                return;
            }

            if (currentBehaviourStateIndex == 1 && (Object)(object)chasingPlayer == (Object)(object)GameNetworkManager.Instance.localPlayerController && !lostPlayerInChase)
            {
                GameNetworkManager.Instance.localPlayerController.IncreaseFearLevelOverTime(1.4f);
                return;
            }

            bool flag = false;
            if (!GameNetworkManager.Instance.localPlayerController.isInHangarShipRoom && HasLineOfSightToPosition(GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.position, 45f, 70))
            {
                if (Vector3.Distance(((Component)(object)this).transform.position, ((Component)(object)GameNetworkManager.Instance.localPlayerController).transform.position) < 15f)
                {
                    GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(0.7f);
                }
                else
                {
                    GameNetworkManager.Instance.localPlayerController.JumpToFearLevel(0.4f);
                }
            }
        }
        */
    }
}
