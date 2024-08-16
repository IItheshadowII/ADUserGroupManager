namespace ADUserGroupManager
{
    public static class DomainSettings
    {
        public static string BaseDN { get; set; } = "DC=praxisclouds,DC=labs"; // Valor por defecto
        public static string DomainController { get; set; } = "dc01.praxisclouds.labs"; // Valor por defecto
        public static string DomainName { get; set; } = "praxisclouds.labs"; // Valor por defecto
    }
}
