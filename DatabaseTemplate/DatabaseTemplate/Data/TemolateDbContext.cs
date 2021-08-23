using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InformationCard.Data
{
    public class TemolateDbContext : DbContext
    {
        public TemolateDbContext(DbContextOptions options)
           : base(options)
        {
        }
    }
}
