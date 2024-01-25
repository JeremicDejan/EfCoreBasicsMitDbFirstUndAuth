using System;
using System.Collections.Generic;

#nullable disable

namespace EfCoreBasicsMitDbFirstUndAuth.Data
{
    public partial class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public byte[] PwHash { get; set; }
        public byte[] Salt { get; set; }
        public int RoleId { get; set; }

        public virtual AppRole Role { get; set; }
    }
}
