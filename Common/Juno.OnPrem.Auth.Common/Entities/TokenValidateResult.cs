using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Juno.OnPrem.Auth.Common.Entities
{
    public class TokenValidateResult
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public ClaimsPrincipal User { get; set; }
    }
}
