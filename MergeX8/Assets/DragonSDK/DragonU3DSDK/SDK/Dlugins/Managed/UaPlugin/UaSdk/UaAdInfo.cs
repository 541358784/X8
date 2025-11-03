using System.Collections.Generic;

namespace DragonPlus.Ad.UA
{
    public class UaAdInfo
    {
        public string AdUnitIdentifier   { get; private set; } = string.Empty;
        public string AdFormat           { get; private set; } = string.Empty;
        public string NetworkName        { get; private set; } = string.Empty;
        public string NetworkPlacement   { get; private set; } = string.Empty;
        public string Placement          { get; private set; } = string.Empty;
        public double Revenue            { get; private set; }
        public string RevenuePrecision   { get; private set; } = string.Empty;
        public string CreativeIdentifier { get; private set; } = string.Empty;
        public string AppUrl             { get; private set; } = string.Empty;
        public string ExtraData          { get; private set; } = string.Empty;
        public string InstanceId         { get; private set; } = string.Empty;

        public UaAdInfo(Dictionary<string, string> properties)
        {
            if (properties.TryGetValue("Id", out var value))
            {
                InstanceId = value;
            }
            
            if (properties.TryGetValue("AdUnitIdentifier", out value))
            {
                AdUnitIdentifier = value;
            }

            if (properties.TryGetValue("AdFormat", out value))
            {
                AdFormat = value;
            }

            if (properties.TryGetValue("NetworkName", out value))
            {
                NetworkName = value;
            }

            if (properties.TryGetValue("NetworkPlacement", out value))
            {
                NetworkPlacement = value;
            }

            if (properties.TryGetValue("Placement", out value))
            {
                Placement = value;
            }

            if (properties.TryGetValue("Revenue", out value))
            {
                if (double.TryParse(value, out var revenue))
                {
                    Revenue = revenue;
                }
            }

            if (properties.TryGetValue("RevenuePrecision", out value))
            {
                RevenuePrecision = value;
            }

            if (properties.TryGetValue("CreativeIdentifier", out value))
            {
                CreativeIdentifier = value;
            }

            if (properties.TryGetValue("AppUrl", out value))
            {
                AppUrl = value;
            }

            if (properties.TryGetValue("ExtraData", out value))
            {
                ExtraData = value;
            }
        }
    }
}