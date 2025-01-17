﻿using System;
using System.Collections;
using Newtonsoft.Json.Linq;
using BarRaider.SdTools;
using ArtrointelPlugin.Control.Model;

namespace ArtrointelPlugin.Control.Payload
{
    public class PayloadReader
    {
        // constants from property inspector written in javascript.
        public const string PAYLOAD_IMAGE_UPDATE_KEY = "payload_updateImage";
        public const string PAYLOAD_EFFECT_KEY = "payload_updateEffects";
        public const string PAYLOAD_FUNCTION_KEY = "payload_updateFunctions";
        public const string PAYLOAD_COMMAND_KEY = "payload_commands";

        public const string META_DATA_COUNT = "meta_arrayCount";
        public const string META_FILE_PATH = "meta_filePath";

        public const string KEY_EFFECT_TRIGGER = "sEffectTrigger";
        public const string KEY_EFFECT_TYPE = "sEffectType";
        public const string KEY_EFFECT_RGB = "iEffectRGB";
        public const string KEY_EFFECT_ALPHA = "iEffectAlpha";
        public const string KEY_EFFECT_DELAY = "iEffectDelay";
        public const string KEY_EFFECT_DURATION = "iEffectDuration";
        public const string KEY_EFFECT_METADATA = "iEffectMetadata";

        public const string KEY_FUNCTION_TRIGGER = "sFunctionTrigger";
        public const string KEY_FUNCTION_TYPE = "sFunctionType";
        public const string KEY_FUNCTION_DELAY = "iFunctionDelay";
        public const string KEY_FUNCTION_INTERVAL = "iFunctionInterval"; // in millisecond
        public const string KEY_FUNCTION_DURATION = "iFunctionDuration";
        public const string KEY_FUNCTION_METADATA = "iFunctionMetadata"; // handled by the type

        private PayloadReader()
        {

        }

        public static bool IsEffectPayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_EFFECT_KEY);
        }

        public static bool IsFunctionPayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_FUNCTION_KEY);
        }

        public static bool IsCommandPayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_COMMAND_KEY);
        }

        private static bool IdentifyPayload(JObject payload, string key)
        {
            bool ret = false;
            try
            {
                ret = bool.Parse(payload.Value<string>(key));
            }
            catch { }
            return ret;
        }

        public static int GetArrayCount(JObject payload)
        {
            return payload.Value<int>(META_DATA_COUNT);
        }

        public static bool IsImageUpdatePayload(JObject payload)
        {
            return IdentifyPayload(payload, PAYLOAD_IMAGE_UPDATE_KEY);
        }

        public static string GetFilePath(JObject payload)
        {
            string ret = null;
            try
            {
                ret = payload.Value<string>(META_FILE_PATH);
            }
            catch { }
            
            return ret;
        }

        public static ArrayList LoadEffectDataFromPayload(JObject payload, int count)
        {
            try
            {
                ArrayList newEffectList = new ArrayList();
                for (int i = 1; i <= count; i++)
                {
                    string trigger = payload.Value<string>(KEY_EFFECT_TRIGGER + i);
                    string type = payload.Value<string>(KEY_EFFECT_TYPE + i);
                    string hexrgb = payload.Value<string>(KEY_EFFECT_RGB + i);
                    string alpha = payload.Value<string>(KEY_EFFECT_ALPHA + i);
                    double delay = payload.Value<double>(KEY_EFFECT_DELAY + i);
                    double duration = payload.Value<double>(KEY_EFFECT_DURATION + i);
                    string metadata = payload.Value<string>(KEY_EFFECT_METADATA + i);
                    newEffectList.Add(EffectConfig.Create(
                        trigger, type, hexrgb, alpha, delay, duration, metadata));
                }
                return newEffectList;
            } 
            catch(Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Input effect data is wrong: " + e.ToString());
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
                return newFunctionList;
            }
            catch (Exception e)
            {
                Logger.Instance.LogMessage(TracingLevel.ERROR, "Input effect data is wrong: " + e.ToString());
            }
            return null;
        }
    }
}
