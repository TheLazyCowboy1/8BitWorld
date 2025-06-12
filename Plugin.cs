using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using BepInEx;
using Unity.Mathematics;
using RWCustom;
using Watcher;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Video;
using System.IO;
using System.Threading.Tasks;
using System.Drawing;
using Graphics = UnityEngine.Graphics;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Color = UnityEngine.Color;

#pragma warning disable CS0618

[module: UnverifiableCode]
[assembly: SecurityPermission(System.Security.Permissions.SecurityAction.RequestMinimum, SkipVerification = true)]

namespace LowBitWorld;

[BepInPlugin(MOD_ID, MOD_NAME, MOD_VERSION)]
public partial class Plugin : BaseUnityPlugin
{
    public const string MOD_ID = "LazyCowboy.8BitWorld",
        MOD_NAME = "8-Bit World",
        MOD_VERSION = "0.0.1";


    public static ConfigOptions Options;

    public Plugin()
    {
        try
        {
            Options = new ConfigOptions();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }
    private void OnEnable()
    {
        On.RainWorld.OnModsInit += RainWorldOnOnModsInit;
    }
    private void OnDisable()
    {
        On.RainWorld.OnModsInit -= RainWorldOnOnModsInit;
        if (IsInit)
        {

            IsInit = false;
        }
    }

    public static Shader EffectShader;
    public static Material EffectMaterial;

    public static int ShaderBitMaskRef;
    public static int ShaderBrightnessRef;

    private bool IsInit;
    private void RainWorldOnOnModsInit(On.RainWorld.orig_OnModsInit orig, RainWorld self)
    {
        orig(self);
        try
        {
            if (IsInit) return;

            //load shader
            try
            {
                AssetBundle assetBundle = AssetBundle.LoadFromFile(AssetManager.ResolveFilePath("AssetBundles\\LazyCowboy\\8BitWorld.assets"));

                EffectShader = assetBundle.LoadAsset<Shader>("8BitEffect.shader");
                if (EffectShader == null)
                    Logger.LogError("Could not find shader 8BitEffect.shader");
                EffectMaterial = new(EffectShader);

                ShaderBitMaskRef = Shader.PropertyToID("TheLazyCowboy1_BitMask");
                ShaderBrightnessRef = Shader.PropertyToID("TheLazyCowboy1_Brightness");

                Futile.instance.camera.gameObject.AddComponent<LowBitEffect>();
            }
            catch (Exception ex) { Logger.LogError(ex); }
            
            MachineConnector.SetRegisteredOI(MOD_ID, Options);
            IsInit = true;

            Logger.LogDebug("Applied hooks");
        }
        catch (Exception ex)
        {
            Logger.LogError(ex);
            throw;
        }
    }

    public class LowBitEffect : MonoBehaviour
    {
        private void Update()
        {
            Shader.SetGlobalInt(ShaderBitMaskRef, Int32.MaxValue ^ (255 >> Options.Bits.Value)); //0b11111...111 EXCEPT (0b11111111 shifted down)
            Shader.SetGlobalFloat(ShaderBrightnessRef, Options.Brightness.Value);
        }

        private void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, EffectMaterial);
        }
    }

}
