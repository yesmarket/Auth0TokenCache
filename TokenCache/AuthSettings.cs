using System.Collections.Generic;

namespace TokenCache
{
    public class AuthSettings
    {
        public virtual Dictionary<string, Tenant> Tenants { get; } = new Dictionary<string, Tenant>();
    }
}
