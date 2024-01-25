using System;
using System.Collections.Generic;

#nullable disable

namespace EfCoreBasicsMitDbFirstUndAuth.Data
{
    public partial class AppRole
    {
        public AppRole()
        {
            AppUsers = new HashSet<AppUser>();
        }

        public int Id { get; set; }
        public string RoleName { get; set; }

        public virtual ICollection<AppUser> AppUsers { get; set; }
    }
}
