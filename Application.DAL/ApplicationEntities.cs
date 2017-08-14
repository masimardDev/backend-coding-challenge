using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DAL
{
    public partial class ApplicationEntities : DbContext
    {
        public ApplicationEntities(string connectingString)
            : base(connectingString)
        {


        }
    }
}
