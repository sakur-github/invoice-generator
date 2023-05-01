using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace InvoiceGenerator.Models.Configuration
{
    public class InvoiceInstanceConfiguration
    {
        public string? Number { get; set; }
        public bool? CanBeConverted { get; set; } = true;

        //method for serializing the object to json
        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }

        //method for deserializing the object from json
        public static InvoiceInstanceConfiguration? FromJson(string? json)
        {
            if (json == null)
                return null;

            return JsonSerializer.Deserialize<InvoiceInstanceConfiguration>(json);
        }
    }
}
