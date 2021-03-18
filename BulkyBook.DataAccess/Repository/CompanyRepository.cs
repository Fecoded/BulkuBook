using BulkyBook.DataAccess.Data;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BulkyBook.DataAccess.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        public new readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) :base(context)
        {
            _context = context;
        }

        public void Update(Company company)
        {
            _context.Update(company);
        }
    }
}
