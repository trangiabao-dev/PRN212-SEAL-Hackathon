using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN212_SEAL.Contracts.Teams
{
    public sealed class CreateTeamMemberRequest
    {
        public string? Fullname { get; set; }
        public string? StudentCode { get; set; }
    }
}
