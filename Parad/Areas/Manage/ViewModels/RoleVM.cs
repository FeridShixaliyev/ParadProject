using Parad.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parad.Areas.Manage.ViewModels
{
    public class RoleVM
    {
        public AppUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
