using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestListener
{
    public class LocalConfig
    {
        public List<ConfigItem> Configs { get; set; }
    }

    public class ConfigItem
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
