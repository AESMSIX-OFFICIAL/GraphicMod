using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.PostProcessing; 

[assembly: MelonInfo(typeof(LowQualityMod.LowQualityModMain), "Ultra Low Graphic", "1.0.2", "AESMSIX")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace LowQualityMod
{
    public class LowQualityModMain : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Low Quality Mod loaded successfully. Setting graphics settings and removing non-essential objects...");
            ApplyLowQualitySettings();
            RemoveNonEssentialObjects();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                MelonLogger.Msg("F9 pressed! Re-running graphics settings and removing environmental objects...");
                ApplyLowQualitySettings();
                RemoveNonEssentialObjects();
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg($"New scene loaded: {sceneName} (Index: {buildIndex}). Applying low quality settings.");
            ApplyLowQualitySettings();
        }

        private void ApplyLowQualitySettings()
        {
            QualitySettings.masterTextureLimit = 3;
            QualitySettings.pixelLightCount = 0;
            QualitySettings.antiAliasing = 0;
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
            QualitySettings.lodBias = 0.5f;
            QualitySettings.shadows = ShadowQuality.Disable;
            QualitySettings.shadowDistance = 0f;
            QualitySettings.realtimeReflectionProbes = false;
            QualitySettings.vSyncCount = 0;
            Shader.globalMaximumLOD = 100;
            QualitySettings.softParticles = false;
            QualitySettings.softVegetation = false;
            QualitySettings.shadowResolution = ShadowResolution.Low;
            QualitySettings.shadowCascades = 0;
            ScalableBufferManager.ResizeBuffers(0.5f, 0.5f);
            Application.targetFrameRate = 60;
            RenderSettings.fog = false;
            RenderSettings.skybox = null;

            Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>();
            foreach (Terrain terrain in terrains)
            {
                terrain.detailObjectDensity = 0.1f;
                terrain.treeBillboardDistance = 10f;
                terrain.detailObjectDistance = 20f;
                terrain.heightmapPixelError = 15;
                if (terrain.materialTemplate != null && terrain.materialTemplate.HasProperty("_MainTex"))
                {
                    terrain.materialTemplate.SetTexture("_MainTex", null);
                }
            }

            PostProcessVolume[] ppVolumes = GameObject.FindObjectsOfType<PostProcessVolume>();
            foreach (var volume in ppVolumes)
            {
                volume.enabled = false;
            }

            RenderPipelineAsset lowQualityPipeline = Resources.Load<RenderPipelineAsset>("LowQualityRenderPipeline");
            if (lowQualityPipeline != null && GraphicsSettings.renderPipelineAsset != lowQualityPipeline)
            {
                GraphicsSettings.renderPipelineAsset = lowQualityPipeline;
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.useOcclusionCulling = false;
            }
            MelonLogger.Msg("Graphics quality settings have been applied with additional parameters for higher FPS.");
        }

        private void RemoveNonEssentialObjects()
        {
            string[] keywords = new string[] { "Smoke", "Tree", "Effect", "Fog", "Grass", "Bush" };
            int removedCount = 0;

            GameObject[] allObjects = GameObject.FindObjectsOfType<GameObject>();

            foreach (GameObject obj in allObjects)
            {
                if (obj != null && obj.activeInHierarchy)
                {
                    foreach (string keyword in keywords)
                    {
                        if (!string.IsNullOrEmpty(obj.name) && obj.name.Contains(keyword))
                        {
                            Object.Destroy(obj);
                            removedCount++;
                            break;
                        }
                    }
                }
            }
            MelonLogger.Msg($"{removedCount} non-essential objects have been removed.");
        }
    }
}
