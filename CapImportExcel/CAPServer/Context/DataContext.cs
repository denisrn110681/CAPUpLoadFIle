using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CAPServer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.InMemory;

namespace CAPServer.Context
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<DeliveredFile> DeliveredFile { get; set; }
    }
}
