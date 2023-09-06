using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Repository;
    public class RolRepository : GenericRepo<Rol>, IRol
    {
        protected readonly DbApiContext _context;
        
        public RolRepository(DbApiContext context) : base (context)
        {
            _context = context;
        }
    
        public override async Task<IEnumerable<Rol>> GetAllAsync()
        {
            return await _context.Rols
                .Include(p => p.Users)
                .ToListAsync();
        }
    
        public override async Task<Rol> GetByIdAsync(int id)
        {
            return await _context.Rols
            .Include(p => p.Users)
            .FirstOrDefaultAsync(p =>  p.Id == id);
        }
}