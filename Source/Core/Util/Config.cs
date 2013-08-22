using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;

namespace ModestTree
{
    public static class Config
    {
        static XmlDocument _xml;

        static Config()
        {
            LoadConfigFile();
            Log.Info("Loaded config file");
        }

        static void LoadConfigFile()
        {
            Util.Assert(_xml == null);

            try
            {
                _xml = new XmlDocument();
                // TODO: Change this to use resources.load so it works with web builds
                _xml.Load( Application.dataPath + @"\core_config.xml" );
            }
            catch (Exception)
            {
                Log.Warn("No Assets/core_config.xml found.");
                _xml = null;
            }
        }

        static bool TryGet<TValue>(string identifier, ref TValue value)
        {
            if (_xml == null)
            {
                return false;
            }

            var child = _xml.SelectSingleNode("/Config/" + identifier);

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

            if (TryGet(identifier, ref value))
            {
                return value;
            }

            Util.Assert(false, "Unable to find setting '" + identifier + "'");
            return default(TValue);
        }

        public static TValue Get<TValue>(string identifier, TValue defaultValue)
        {
            var value = defaultValue;

            TryGet(identifier, ref value);
            return value;
        }

        public static List<TValue> GetAll<TValue>(string identifier)
        {
            if (_xml == null)
            {
                return new List<TValue>();
            }

            var nodeList = _xml.SelectNodes("/Config/" + identifier);

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
    }
}
