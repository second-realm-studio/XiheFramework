﻿using System;

namespace XiheFramework.Core.Config {
    [AttributeUsage(AttributeTargets.Field)]
    public class ConfigAttribute : Attribute {
        public string Path { get; }
        public object DefaultValue { get; set; }

        public ConfigAttribute(string path = null) {
            Path = path;
        }
    }
}