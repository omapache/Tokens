using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aplicacion.Repository;
using Dominio.Interfaces;
using Persistencia;

namespace Aplicacion.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbApiContext context;
        private UserRepository _users;
        private RolRepository _roles;
        public UnitOfWork(DbApiContext _context)
        {
            context = _context;
        }
        public IUser Users  
        {
            get{
                if(_users== null){
                    _users= new UserRepository(context);
                }
                return _users;
            }
        }
        public IRol Rols   
        {
            get{
                if(_roles== null){
                    _roles= new RolRepository(context);
                }
                return _roles;
            }
        }
        public void Dispose()
        {
            context.Dispose();
        }
        public async Task<int> SaveAsync()
        {
            return await context.SaveChangesAsync();
        }
    }
    
}