using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace libs.Converters
{
    public class MappedNameContractResolver : CamelCasePropertyNamesContractResolver
    {
        public MappedNameContractResolver()
        {
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);
            var map = member.GetCustomAttribute(typeof(MappedNameAttribute)) as MappedNameAttribute;

            if (map != null)
            {
                property.PropertyName = map.NewName;
            }
                           

            return property;
        }
    }
}
