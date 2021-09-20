﻿using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ArtrointelPlugin.Control;
using ArtrointelPlugin.Utils;

namespace ArtrointelPlugin
{
    [PluginActionId("com.artrointel.avengerskey")]
    public class PluginAction : PluginBase
    {
        private static Dictionary<string, AvengersKeyController> sControllerInstances
            = new Dictionary<string, AvengersKeyController>();

        private AvengersKeyController mController;
        private DelayedTask mLongPressTask;

        private const int LONG_PRESS_TIME = 1000;
        
        public PluginAction(ISDConnection connection, InitialPayload payload) : base(connection, payload)
        {
            if (!sControllerInstances.ContainsKey(connection.ContextId))
            {
                sControllerInstances.Add(connection.ContextId,
                    new AvengersKeyController(payload.Settings, async (image) =>
                    {
                        await Connection.SetImageAsync(image);
                    }));
            }

            mController = sControllerInstances[connection.ContextId];
            mController.onKeyAppeared();

            Connection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin += Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
        }

        private void Connection_OnTitleParametersDidChange(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.TitleParametersDidChange> e)
        {

        }

        private void Connection_OnSendToPlugin(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.SendToPlugin> e)
        {
            if(mController.handlePayload(e.Event.Payload))
            {
                SaveSettings();
            }
        }

        private void Connection_OnPropertyInspectorDidDisappear(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidDisappear> e)
        {

        }

        private void Connection_OnPropertyInspectorDidAppear(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.PropertyInspectorDidAppear> e)
        {
            Connection.SendToPropertyInspectorAsync(JObject.FromObject(mController.getSettings()));
        }

        private void Connection_OnDeviceDidDisconnect(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidDisconnect> e)
        {

        }

        private void Connection_OnDeviceDidConnect(object sender, SDEventReceivedEventArgs<BarRaider.SdTools.Events.DeviceDidConnect> e)
        {

        }

        private void Connection_OnApplicationDidTerminate(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidTerminate> e)
        {

        }

        private void Connection_OnApplicationDidLaunch(object sender, BarRaider.SdTools.Wrappers.SDEventReceivedEventArgs<BarRaider.SdTools.Events.ApplicationDidLaunch> e)
        {

        }

        public override void Dispose()
        {
            Connection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
            Connection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
            Connection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
            Connection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
            Connection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
            Connection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
            Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
            Connection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
            mController.onKeyDisappeared();
        }

        public async override void KeyPressed(KeyPayload payload)
        {
            await Task.Run(() =>
            {
                mController.onKeyPressed();
            });
            mLongPressTask = new DelayedTask(LONG_PRESS_TIME, () =>
            {
                KeyLongPressed(payload);
            });
        }

        public async override void KeyReleased(KeyPayload payload) 
        {
            // TODO mController.actionOnKeyReleased();
            if(mLongPressTask != null)
                mLongPressTask.cancel();
        }

        public async void KeyLongPressed(KeyPayload payload)
        {
            await Task.Run(() =>
            {
                mController.onKeyLongPressed();
            });
        }

        public override void OnTick() { }

        public override void ReceivedSettings(ReceivedSettingsPayload payload)
        {
            DLogger.LogMessage(TracingLevel.DEBUG, "ReceivedSettings: " + payload.ToString());
        }

        public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }

        #region Private Methods

        private Task SaveSettings()
        {
            return Connection.SetSettingsAsync(JObject.FromObject(mController.getSettings()));
        }

        #endregion
    }
}