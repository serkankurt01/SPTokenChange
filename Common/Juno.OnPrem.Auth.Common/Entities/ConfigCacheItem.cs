using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juno.OnPrem.Auth.Common.Entities
{
    [Serializable]
    public class ConfigCacheItem
    {
        public Guid WebId { get; set; }
        public string WebUrl { get; set; }
        public string StsDiscoveryUrl { get; set; }
        public string Audiance { get; set; }
        public string Issuer { get; set; }
        public string SPRealm { get; set; }
        public string SPIssuerId { get; set; }
        public string SPClientId { get; set; }
        public string UPAuthType { get; set; }
        public string AuthType { get; set; }
        public string CertificatePath { get; set; }
        public string CertificatePassword { get; set; }
        public bool LifeTimeCheck { get; set; }
        public bool DebugMode { get; set; }
        public bool IsActive { get; set; }
        public string AuthProviderName { get; set; }

    }
}
