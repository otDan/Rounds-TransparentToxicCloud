using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace TransparentToxicCloud
{
    [BepInDependency("com.willis.rounds.unbound")]
    [BepInDependency("pykess.rounds.plugins.moddingutils")]
    [BepInDependency("pykess.rounds.plugins.pickncards")]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("Rounds.exe")]
    public class TransparentToxicCloud : BaseUnityPlugin
    {
        private const string ModId = "ot.dan.rounds.transparenttoxiccloud";
        private const string ModName = "Transparent Toxic Cloud";
        public const string Version = "1.0.1";
        public const string ModInitials = "TTC";
        private const string CompatibilityModName = "TransparentToxicCloud";
        public static TransparentToxicCloud Instance { get; private set; }

        private void Awake()
        {
            UnboundLib.Unbound.RegisterClientSideMod(ModId);
            Instance = this;

            var harmony = new Harmony(ModId);
            harmony.PatchAll();
        }

        private void Start()
        {
            GameObject toxicCloud = ((GameObject) Resources.Load("0 cards/Toxic cloud")).GetComponent<Gun>().objectsToSpawn[0].effect.GetComponent<SpawnObjects>().objectToSpawn[0];
            ParticleSystem particleSystem = toxicCloud.GetComponent<ParticleSystem>();
            var particleSystemEmission = particleSystem.emission;
            particleSystemEmission.rateOverTime = new ParticleSystem.MinMaxCurve(15);

            var particleSystemMain = particleSystem.main;
            var startColor = particleSystemMain.startColor;
            var startColorColorMin = startColor.colorMin;
            var startColorColorMax = startColor.colorMax;

            var newStartColor = new ParticleSystem.MinMaxGradient(
                new Color(startColorColorMin.r, startColorColorMin.g, startColorColorMin.b, 0.10f),
                new Color(startColorColorMax.r, startColorColorMax.g, startColorColorMax.b, 0.05f)
                );
            particleSystemMain.startColor = newStartColor;
            
            var particleSystemRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            ToFadeMode(particleSystemRenderer.material);
        }

        private static readonly int SrcBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int DstBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int ZWrite = Shader.PropertyToID("_ZWrite");
        public static void ToFadeMode(Material material)
        {
            material.SetOverrideTag("RenderType", "Transparent");
            material.SetInt(SrcBlend, (int) UnityEngine.Rendering.BlendMode.SrcAlpha);
            material.SetInt(DstBlend, (int) UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            material.SetInt(ZWrite, 0);
            material.DisableKeyword("_ALPHATEST_ON");
            material.EnableKeyword("_ALPHABLEND_ON");
            material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            material.renderQueue = (int) UnityEngine.Rendering.RenderQueue.Transparent;
        }

        public void Log(string debug)
        {
            UnityEngine.Debug.Log(debug);
        }
    }
}