using MelonLoader;
using UnityEngine;
using UnityEngine.Rendering;

[assembly: MelonInfo(typeof(LowQualityMod.LowQualityModMain), "Ultra Low Graphic", "1.0.2", "AESMSIX")]
[assembly: MelonGame("Radian Simulations LLC", "GHPC")]

namespace LowQualityMod
{
    public class LowQualityModMain : MelonMod
    {
        private bool lowQualityApplied = false;

        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Low Quality Mod loaded successfully. Setting graphics settings and removing non-essential objects..."); // Prints a log message that the mod loaded successfully
            ApplyLowQualitySettings();
            RemoveNonEssentialObjects();
            lowQualityApplied = true;
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.F9))
            {
                MelonLogger.Msg("F9 pressed! Re-running graphics settings,and removing environmental objects..."); // Prints a log message when F9 is pressed
                ApplyLowQualitySettings();
                RemoveNonEssentialObjects();
                lowQualityApplied = true;
            }
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            MelonLogger.Msg($"New scene loaded: {sceneName} (Index: {buildIndex}). Applying low quality settings.");
            ApplyLowQualitySettings();
            lowQualityApplied = true;
        }

        private void ApplyLowQualitySettings()
        {
            QualitySettings.masterTextureLimit = 3; // Limits the maximum texture resolution (higher is better, low value means worse graphics)
            QualitySettings.pixelLightCount = 0; // Sets the number of pixel lights to 0 (higher is better, 0 means none, worse graphics)
            QualitySettings.antiAliasing = 0; // Disables anti-aliasing (higher values smooth out edges, 0 means none, worse graphics)
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable; // Disables anisotropic filtering (improves texture sharpness at oblique angles, Disable means worse graphics)
            QualitySettings.lodBias = 0.5f; // Reduces the level of detail bias (lower values make objects appear more detailed up close, low value means worse graphics)
            QualitySettings.shadows = ShadowQuality.Disable; // Disables shadows (Disable means no shadows, worse graphics)
            QualitySettings.shadowDistance = 0f; // Sets the shadow render distance to 0 (higher is further shadows are visible, 0 means none, worse graphics)
            QualitySettings.realtimeReflectionProbes = false; // Disables real-time reflection probes (Disable means no real-time reflections, worse graphics)
            QualitySettings.vSyncCount = 0; // Disables VSync (prevents screen tearing, but disabling it can make graphics look rougher)
            Shader.globalMaximumLOD = 100; // Limits the maximum shader level of detail (lower values reduce shader detail, low value means worse graphics)
            QualitySettings.softParticles = false; // Disables soft particles (Disable means particles look harsher, worse graphics)
            QualitySettings.softVegetation = false; // Disables soft vegetation (Disable means vegetation looks harsher, worse graphics)
            QualitySettings.shadowResolution = ShadowResolution.Low; // Sets the shadow resolution to low (lower resolution means worse shadow quality, worse graphics)
            QualitySettings.shadowCascades = 0; // Sets the number of shadow cascades to 0 (higher values improve distant shadow quality, 0 means worse graphics)
            ScalableBufferManager.ResizeBuffers(0.75f, 0.75f); // Reduces the render buffer resolution (lower resolution means worse graphics)
            Application.targetFrameRate = 60; // Sets the target frame rate to 60 FPS (doesn't directly impact graphics quality, but performance)
            RenderSettings.fog = false; // Disables fog (Disable means no fog effect, graphics might look less realistic)
            RenderSettings.skybox = null; // Removes the skybox (null means no background sky, graphics might look empty)
            ScalableBufferManager.ResizeBuffers(0.5f, 0.5f); // Further reduces the render buffer resolution (lower resolution means worse graphics)
            Terrain[] terrains = GameObject.FindObjectsOfType<Terrain>(); // Gets all Terrain objects in the scene
            foreach (Terrain terrain in terrains) // Loops through each Terrain object
            {
                terrain.detailObjectDensity = 0.1f; // Reduces the density of detail objects (like grass) (lower density means fewer details, worse graphics)
                terrain.treeBillboardDistance = 10f; // Reduces the billboard distance for trees (lower distance means trees turn into sprites sooner, worse graphics)
                terrain.detailObjectDistance = 20f; // Reduces the render distance for detail objects (lower distance means detail objects are not visible from far away, worse graphics)
                terrain.heightmapPixelError = 15; // Increases the heightmap pixel error tolerance (higher error means less detailed terrain, worse graphics)
                if (terrain.materialTemplate != null && terrain.materialTemplate.HasProperty("_MainTex")) // Checks if the terrain material has a main texture
                {
                    terrain.materialTemplate.SetTexture("_MainTex", null); // Removes the main texture of the terrain (null means the terrain will look plain, worse graphics)
                }
            }
            UnityEngine.Rendering.PostProcessing.PostProcessVolume[] ppVolumes = GameObject.FindObjectsOfType<UnityEngine.Rendering.PostProcessing.PostProcessVolume>(); // Gets all post-processing volumes
            foreach (var volume in ppVolumes) // Loops through each post-processing volume
            {
                volume.enabled = false; // Disables post-processing (Disable means no effects like bloom, color correction, etc., worse graphics)
            }

            RenderPipelineAsset lowQualityPipeline = Resources.Load<RenderPipelineAsset>("LowQualityRenderPipeline"); // Tries to load a low quality render pipeline asset
            if (lowQualityPipeline != null && GraphicsSettings.renderPipelineAsset != lowQualityPipeline) // Checks if the asset was loaded and is not already in use
            {
                GraphicsSettings.renderPipelineAsset = lowQualityPipeline; // Switches the render pipeline asset to the low quality one (render pipeline significantly impacts overall graphics quality)
            }

            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                mainCamera.useOcclusionCulling = false;
            }
            MelonLogger.Msg("Graphics quality settings have been applied with additional parameters for higher FPS.");
        }

        private void RemoveNonEssentialObjects() // Method to remove non-essential game objects
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
                        if (obj.name != null && obj.name.Contains(keyword))
                        {
                            Object.Destroy(obj);
                            removedCount++;
                            break;
                        }
                    }
                }
            }
            MelonLogger.Msg($"{removedCount} non-essential objects have been removed."); // Prints a log message with the number of removed objects
        }
    }
} //CAN YOU FIX THIS????