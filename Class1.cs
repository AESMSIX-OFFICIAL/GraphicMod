using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[assembly: MelonInfo(typeof(LowQualityMod.LowQualityModMain), "Ultra Low Graphic", "1.1.2", "AESMSIX")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace LowQualityMod
{
    public class LowQualityModMain : MelonMod
    {
        Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();
        GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

        public override void OnInitializeMelon()
        {
            ApplyLowQualitySettings();
            ConfigureMaterials(); // Materialkonfig() -> ConfigureMaterials()
            MelonLogger.Msg("Low Quality Mod loaded successfully. Setting graphics settings and removing non-essential objects...");
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {

                ApplyLowQualitySettings();
                ConfigureMaterials(); // Materialkonfig() -> ConfigureMaterials()
                DeleteNonEssentialObjectsDuringGameplay();
                MelonLogger.Msg("F9 pressed! Re-running graphics settings and removing environmental objects...");
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            ApplyLowQualitySettings();
            MelonLogger.Msg($"New scene loaded: {sceneName} (Index: {buildIndex}). Applying low quality settings.");
        }

        private void ApplyLowQualitySettings()
        {
            QualitySettings.globalTextureMipmapLimit = 15;
            QualitySettings.pixelLightCount = 0;
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.lodBias = 0.01f;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0f;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.maximumLODLevel = 0;
            QualitySettings.softParticles = false;
            QualitySettings.softVegetation = false;
            QualitySettings.shadowCascades = 0;
            ScalableBufferManager.ResizeBuffers(0.01f, 0.01f);
            RenderSettings.fog = false;
            RenderSettings.skybox = null;
            Time.timeScale = 1f;
            Time.maximumDeltaTime = 0.1f;
            MelonLogger.Msg("Graphics quality settings have been applied.");
        }

        private void ConfigureMaterials() 
        {
            foreach (Terrain terrain in terrains)
            {
                terrain.detailObjectDensity = 0.1f;
                terrain.treeBillboardDistance = 0.1f;
                terrain.detailObjectDistance = 10f;
                terrain.heightmapPixelError = 55;
                if (terrain.materialTemplate != null && terrain.materialTemplate.HasProperty("_MainTex"))
                {
                    terrain.materialTemplate.SetTexture("_MainTex", null);
                }
            }
            foreach (Light light in GameObject.FindObjectsOfType<Light>())
            {
                if (light.shadows != LightShadows.None)
                    light.shadows = LightShadows.None;
            }
            foreach (Material mat in Resources.FindObjectsOfTypeAll<Material>())
            {
                if (mat != null)
                {
                    mat.enableInstancing = true;
                }
            }
            foreach (Camera camera in cameras)
            {
                var postProcessingLayer = camera.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessLayer>();
                if (postProcessingLayer != null)
                {
                    postProcessingLayer.enabled = false;
                }

                var postProcessingVolume = camera.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
                if (postProcessingVolume != null)
                {
                    postProcessingVolume.enabled = false;
                }
                var volumes = GameObject.FindObjectsOfType<PostProcessVolume>();
                foreach (var volume in volumes)
                {
                    volume.enabled = false;
                }
            }
        }
        private void DeleteNonEssentialObjectsDuringGameplay()
        {
            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();
            int deletedCount = 0;

            foreach (GameObject obj in allObjects)
            {
                if (obj == null)
                    continue;

                string objName = obj.name.ToLower();

                if (objName.Contains("tree") || objName.Contains("leaves") ||
                    objName.Contains("dust") || objName.Contains("smoke") ||
                    objName.Contains("skybox") || objName.Contains("sun") ||
                    objName.Contains("cloud") ||
                    objName.Contains("fog") || 
                    objName.Contains("grass"))
                {
                    try
                    {
                        Renderer renderer = obj.GetComponent<Renderer>();
                        if (renderer != null)
                            renderer.enabled = false;

                        Renderer[] childRenderers = obj.GetComponentsInChildren<Renderer>();
                        foreach (Renderer childRenderer in childRenderers)
                        {
                            childRenderer.enabled = false;
                        }

                        obj.SetActive(false);
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Msg("Failed to delete object " + obj.name + ": " + ex.Message); // Gagal menghapus objek -> Failed to delete object
                    }
                }
            }
            MelonLogger.Msg("Number of objects deleted and forced not to render: " + deletedCount); // Jumlah objek yang dihapus dan dipaksa agar tidak dirender -> Number of objects deleted and forced not to render
        }
    }
}
