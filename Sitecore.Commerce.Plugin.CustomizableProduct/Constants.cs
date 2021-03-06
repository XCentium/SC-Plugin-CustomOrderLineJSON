namespace Sitecore.Commerce.Plugin.CustomizableProduct
{
    public class Constants
    {
        public struct Settings
        {
            public const string EndpointUrl = "{0}/api/Carts('{1}')?$expand=Lines($expand=CartLineComponents($expand=ChildComponents)),Components($expand=ChildComponents)";

            public const string AppJson = "application/json";
            public const string ShopperId = "ShopperId";
            public const string CartRoles = "sitecore\\Pricer Manager|sitecore\\Promotioner Manager";
            public const string CartId = "cartId";
            public const string CartLineProperties = "cartLineProperties";
            public const string ShopName = "ShopName";
            public const string Language = "Language";
            public const string Environment = "Environment";
            public const string GeoLocation = "GeoLocation";
            public const string CustomerId = "CustomerId";
            public const string Currency = "Currency";
            public const string Roles = "Roles";

        }
    }
}