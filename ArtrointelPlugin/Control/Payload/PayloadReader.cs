﻿using System;
using System.Collections;
using System.Drawing;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using ArtrointelPlugin.Control.Model;

namespace ArtrointelPlugin.Control.Payload
{
    public class PayloadReader
    {
        // constants from property inspector written in javascript.
        public const String PAYLOAD_IMAGE_UPDATE_KEY = "payload_updateImage";
        public const String PAYLOAD_EFFECT_KEY = "payload_updateEffects";
        public const String PAYLOAD_FUNCTIONS_KEY = "payload_updateFunctions";

        public const String KEY_EFFECT_TRIGGER = "sEffectTrigger";
        public const String KEY_EFFECT_TYPE = "sEffectType";
        public const String KEY_EFFECT_RGB = "iEffectRGB";
        public const String KEY_EFFECT_ALPHA = "iEffectAlpha";
        public const String KEY_EFFECT_DELAY = "iEffectDelay";
        public const String KEY_EFFECT_DURATION = "iEffectDuration";

        public const String KEY_FUNCTION_TRIGGER = "sFunctionTrigger";
        public const String KEY_FUNCTION_TYPE = "sFunctionType";
        public const String KEY_FUNCTION_DELAY = "iFunctionDelay";
        public const String KEY_FUNCTION_INTERVAL = "iFunctionInterval"; // in millisecond
        public const String KEY_FUNCTION_DURATION = "iFunctionDuration";
        public const String KEY_FUNCTION_METADATA = "iFunctionMetadata"; // handled by the type

        private PayloadReader()
        {

        }

        public static int isEffectPayload(JObject payload)
        {
            int effectCount = payload.Value<int>(PAYLOAD_EFFECT_KEY);
            return effectCount;
        }

        public static int isFunctionPayload(JObject payload)
        {
            int functionCount = payload.Value<int>(PAYLOAD_FUNCTIONS_KEY);
            return functionCount;
        }

        public static String isImageUpdatePayload(JObject payload)
        {
            String base64ImageString = payload.Value<String>(PAYLOAD_IMAGE_UPDATE_KEY);
            return base64ImageString;
        }

        public static ArrayList LoadEffectDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newEffectList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    String trigger = payload.Value<String>(KEY_EFFECT_TRIGGER + i);
                    String type = payload.Value<String>(KEY_EFFECT_TYPE + i);
                    String hexrgb = payload.Value<String>(KEY_EFFECT_RGB + i);
                    String alpha = payload.Value<String>(KEY_EFFECT_ALPHA + i);
                    double delay = payload.Value<double>(KEY_EFFECT_DELAY + i);
                    double duration = payload.Value<double>(KEY_EFFECT_DURATION + i);
                    newEffectList.Add(EffectConfig.Load(
                        trigger, type,
                        Color.FromArgb(Int32.Parse(alpha), ColorTranslator.FromHtml(hexrgb)),
                        delay, duration));
                }
                return newEffectList;
            } catch(Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "input effect data is wrong: " + e.ToString());
            }
            return null;
        }

        public static ArrayList LoadFunctionDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newFunctionList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    string trigger = payload.Value<string>(KEY_FUNCTION_TRIGGER + i);
                    string type = payload.Value<string>(KEY_FUNCTION_TYPE + i);
                    double delay = payload.Value<double>(KEY_FUNCTION_DELAY + i);
                    double interval = payload.Value<double>(KEY_FUNCTION_INTERVAL + i);
                    double duration = payload.Value<double>(KEY_FUNCTION_DURATION + i);
                    string metadata = payload.Value<string>(KEY_FUNCTION_METADATA + i);
                    newFunctionList.Add(FunctionConfig.Load(
                        trigger, type, delay, interval, duration, metadata));
                }
                Logger.Instance.LogMessage(TracingLevel.DEBUG, "Loaded Payload, count is " + newFunctionList.Count);
                return newFunctionList;
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "input effect data is wrong: " + e.ToString());
            }
            return null;
        }
    }
}