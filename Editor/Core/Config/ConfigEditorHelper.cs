﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using XiheFramework.Core.Config;

namespace XiheFramework.Editor.Core.Config {
    public static class ConfigEditorHelper {
        public static ConfigInfo[] FindAllConfigInfo() {
            var result = new List<ConfigInfo>();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies) {
                Type[] types;
                try {
                    types = assembly.GetTypes();
                }
                catch {
                    continue;
                }

                foreach (var type in types) {
                    var fields = type.GetFields(BindingFlags.Public
                                                | BindingFlags.NonPublic
                                                | BindingFlags.Instance
                                                | BindingFlags.Static);

                    foreach (var field in fields) {
                        if (field.DeclaringType == null) {
                            Debug.LogWarning($"Null declaring type for {field.Name}. Dynamic fields generated by TypeBuilder are not supported. Skipping...");
                            continue;
                        }

                        if (field.GetCustomAttributes(typeof(ConfigAttribute), false).FirstOrDefault() is ConfigAttribute configAttr) {
                            string path = configAttr.Path;
                            if (string.IsNullOrEmpty(path)) {
                                string ns = field.DeclaringType.Namespace ?? "Global";
                                string className = field.DeclaringType.Name;
                                string fieldName = field.Name;
                                path = $"{ns}.{className}.{fieldName}";
                            }

                            var configInfo = new ConfigInfo(path, ConfigHelper.GetConfigType(field.FieldType), configAttr.DefaultValue);
                            result.Add(configInfo);
                        }
                    }
                }
            }

            return result.ToArray();
        }
    }
}