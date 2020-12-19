using System;
using System.Collections.Generic;
using System.ComponentModel;
using CSCore;
using CSCore.DSP;
using CSCore.SoundIn;
using CSCore.Streams;
using TTController.Common;
using TTController.Common.Plugin;

namespace TTController.Plugin.SoundEffect
{
    public class SoundEffectConfigBase : EffectConfigBase { }

    public abstract class SoundEffectBase<T> : EffectBase<T> where T : SoundEffectConfigBase
    {
        protected WasapiLoopbackCapture SoundIn { get; private set; }

        private bool _initialized;
        private int _lastInitializeTry;

        public SoundEffectBase(T config) : base(config)
        {
            _initialized = false;
            TryInitialize();

            _lastInitializeTry = 0;
        }

        public override string EffectType => "PerLed";
        public override bool IsEnabled(ICacheProvider cache) => _initialized ? base.IsEnabled(cache) : TryInitialize();

        protected abstract void Initialize();
        private bool TryInitialize()
        {
            if (_initialized)
                return true;

            var currentTicks = Environment.TickCount;
            if (currentTicks - _lastInitializeTry < 1000)
                return false;
            _lastInitializeTry = currentTicks;

            try
            {
                SoundIn = new WasapiLoopbackCapture();
                SoundIn.Initialize();
            }
            catch (Exception e)
            {
                Logger.Debug(e, "Failed to initialize WasapiLoopbackCapture!");
                return false;
            }

            Logger.Debug($"Initialized WasapiLoopbackCapture on \"{SoundIn.Device.FriendlyName}\"");
            Initialize();
            SoundIn.Start();

            _initialized = true;
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            SoundIn?.Stop();
            SoundIn?.Dispose();

            base.Dispose(disposing);
        }
    }


}
