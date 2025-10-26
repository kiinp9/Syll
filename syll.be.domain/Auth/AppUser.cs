﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace syll.be.domain.Auth
{
    public class AppUser : IdentityUser
    {
        [MaxLength(250)]
        public string FullName { get; set; } = String.Empty;

        [MaxLength(250)]
        public string MsAccount { get; set; } = String.Empty;
    }
}
