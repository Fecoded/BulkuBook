using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public new readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) :base(context)
        {
            _context = context;
        }

        public void Update(ApplicationUser applicationUser)
        {
            _context.Update(applicationUser);
        }
    }
}
