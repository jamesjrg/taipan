using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Configuration;

namespace TaiPan.Common
{
    public class ServersSection : ConfigurationSection
    {
        public ServersSection()
        {
        }

        [ConfigurationProperty("", IsDefaultCollection = true)]
        public ServersCollection Servers
        {
            get
            {
                return (ServersCollection)base[""];
            }
        }
    }

    public class ServersCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ServerElement();
        }
        protected override object GetElementKey( ConfigurationElement element )
        {
            return ( (ServerElement)element ).Name;
        }
        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }
        protected override string ElementName
        {
            get
            {
                return "server";
            }
        }
    }

    public sealed class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            { this["name"] = value; }
        }

        [ConfigurationProperty("address", IsRequired = true)]
        public string Address
        {
            get
            {
                return (string)this["address"];
            }
            set
            { this["address"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get
            {
                return (int)this["port"];
            }
            set
            { this["port"] = value; }
        }
    }
}
