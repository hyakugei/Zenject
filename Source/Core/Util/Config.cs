using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
#if UNITY_EDITOR || UNITY_WEBPLAYER
using UnityEngine;
#endif

namespace ModestTree
{
    public static class Config
    {
        static XmlDocument _xml;
        static XmlDocument _xmlOverride;

        static Config()
        {
            LoadConfigFile();
            Log.Info("Loaded config file");
        }

        static XmlDocument LoadXml(string resourceName)
        {
            try
            {
                var xml = new XmlDocument();

#if UNITY_EDITOR || UNITY_WEBPLAYER
                // Use Resources.Load so it works with web builds
                var obj = Resources.Load(resourceName) as TextAsset;
                xml.LoadXml(obj.text);
#else
                // For non-unity builds just assume it's in the starting directory
                var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, resourceName + ".xml");
                xml.Load(path);
#endif
                return xml;
            }
            catch (Exception)
            {
            }

            return null;
        }

        static void LoadConfigFile()
        {
            Util.Assert(_xml == null);
            Util.Assert(_xmlOverride == null);

            _xml = LoadXml("core_config");

            if (_xml == null)
            {
                Log.Warn("No core_config found.");
            }

            _xmlOverride = LoadXml("core_config_custom");
        }

        static bool TryGet<TValue>(XmlDocument xml, string identifier, ref TValue value)
        {
            if (xml == null)
            {
                return false;
            }

            var child = xml.SelectSingleNode("/Config/" + identifier);

            if (child == null)
            {
                return false;
            }

            try
            {
                value = (TValue)Convert.ChangeType(child.InnerText.Trim(), typeof(TValue));
            }
            catch(Exception ex)
            {
                Util.Assert(false, "Failed during type conversion while loading setting '" + identifier + "': " + ex.Message);
                return false;
            }

            return true;
        }

        public static TValue Get<TValue>(string identifier)
        {
            TValue value = default(TValue);

            if (TryGet(_xmlOverride, identifier, ref value) || TryGet(_xml, identifier, ref value))
            {
                return value;
            }

            Util.Assert(false, "Unable to find setting '" + identifier + "'");
            return default(TValue);
        }

        public static TValue Get<TValue>(string identifier, TValue defaultValue)
        {
            var value = default(TValue);
            if (TryGet(_xmlOverride, identifier, ref value) || TryGet(_xml, identifier, ref value))
            {
                return value;
            }

            return defaultValue;
        }

        static List<TValue> GetAll<TValue>(XmlDocument xml, string identifier)
        {
            if (xml == null)
            {
                return new List<TValue>();
            }

            var nodeList = xml.SelectNodes("/Config/" + identifier);

            Util.Assert(nodeList != null);

            var valueList = new List<TValue>();
            foreach (XmlNode node in nodeList)
            {
                try
                {
                    TValue value = (TValue)Convert.ChangeType(node.InnerText.Trim(), typeof(TValue));
                    valueList.Add(value);
                }
                catch (Exception ex)
                {
                    Util.Assert(false, "Failed during type conversion while loading setting '" + identifier + "': " + ex.Message);
                }
            }

            return valueList;
        }

        public static List<TValue> GetAll<TValue>(string identifier)
        {
            return GetAll<TValue>(_xmlOverride, identifier).Concat(GetAll<TValue>(_xml, identifier)).ToList();
        }
    }
}
