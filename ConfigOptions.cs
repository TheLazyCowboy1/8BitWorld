using BepInEx.Logging;
using Menu.Remix;
using Menu.Remix.MixedUI;
using Menu.Remix.MixedUI.ValueTypes;
using RWCustom;
using System.Collections.Generic;
using UnityEngine;

namespace LowBitWorld;

public class ConfigOptions : OptionInterface
{
    public ConfigOptions()
    {
        Bits = this.config.Bind<int>("Bits", 2, new ConfigAcceptableRange<int>(1, 8));
        Brightness = this.config.Bind<float>("Brightness", 1.3f, new ConfigAcceptableRange<float>(0.5f, 10));
    }

    public readonly Configurable<int> Bits;
    public readonly Configurable<float> Brightness;

    private OpSliderTick slider;
    private OpUpdown updown;

    public override void Initialize()
    {
        OpTab tab = new(this, "Options");
        this.Tabs = new[]
        {
            tab
        };

        tab.AddItems(
            new OpLabel(20, 500, "Color Bits:"),
            slider = new OpSliderTick(Bits, new(20, 450), 350) { Increment = 1, min = 1, max = 8 },
            new OpLabel(20, 400, "4"),
            new OpLabel(70, 400, "8"),
            new OpLabel(120, 400, "12"),
            new OpLabel(170, 400, "16"),
            new OpLabel(220, 400, "20"),
            new OpLabel(270, 400, "24"),
            new OpLabel(320, 400, "28"),
            new OpLabel(370, 400, "32"),
            new OpLabel(90, 350, "Brightness Multiplier"),
            updown = new OpUpdown(Brightness, new(20, 350), 60, 2) { description = "Multiplies the brightness of the screen, to make things black less often." }
            );
    }

    public override void Update()
    {
        base.Update();

        if (slider != null)
        {
            int val = slider.GetValueInt();
            slider.description = $"{val * 4}-Bit Colors";
            Bits.Value = val; //a cheat to make the changes appear actively
            Brightness.Value = Mathf.Clamp(updown.GetValueFloat(), 0.6f, 10);
        }
    }
}