namespace Mango.Web.Utility
{
    public class StaticDetails
    {
        public static string CouponAPIBase { get; set; } = string.Empty;

        public static string AuthAPIBase { get; set;} = string.Empty;
        public static string RoleAdmin = "ADMIN";
        public static string RoleCustomer = "CUSTOMER";

        public enum ApiType
        {
            GET, POST, PUT, DELETE,
        }
    }
}
