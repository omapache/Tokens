using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Dtos;
using API.Helpers;
using Dominio.Entities;
using Dominio.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;
    public class UserService : IUserService
    {
        private readonly JWT _jwt;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;
        public UserService(IUnitOfWork unitOfWork, IOptions<JWT> jwt, IPasswordHasher<User> passwordHasher)
        {
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }
        public async Task<string> RegisterAsync(RegisterDto registerDto)
        {
            var User = new User
            {
                Email = registerDto.Email,
                Username = registerDto.UserName,

            };

            User.Password = _passwordHasher.HashPassword(User, registerDto.Password);

            var UserExiste = _unitOfWork.Users
                                                .Find(u => u.Username.ToLower() == registerDto.UserName.ToLower())
                                                .FirstOrDefault();

            if (UserExiste == null)
            {
                /* var rolPredeterminado = _unitOfWork.Rols
                                                     .Find(u => u.Name_Rol == Autorizacion.Rol_PorDefecto.ToString())
                                                     .First();*/
                try
                {
                    //User.Rols.Add(rolPredeterminado);
                    _unitOfWork.Users.Add(User);
                    await _unitOfWork.SaveAsync();

                    return $"El User {registerDto.UserName} ha sido registrado exitosamente";
                }

                catch (Exception ex)
                {
                    var message = ex.Message;
                    return $"Error: {message}";
                }
            }
            else
            {

                return $"El User con {registerDto.UserName} ya se encuentra resgistrado.";
            }

        }

        public async Task<string> AddRoleAsync(AddRoleDto model)
        {
            var User = await _unitOfWork.Users
                            .GetByUsernameAsync(model.UserName);

            if (User == null)
            {
                return $"No existe algun User registrado con la cuenta olvido algun caracter?{model.UserName}.";
            }

            var resultado = _passwordHasher.VerifyHashedPassword(User, User.Password, model.Password);

            if (resultado == PasswordVerificationResult.Success)
            {
                var rolExiste = _unitOfWork.Rols
                    .Find(u => u.Nombre.ToLower() == model.Rol.ToLower())
                    .FirstOrDefault();

                if (rolExiste != null)
                {
                    var UserTieneRol = User.Rols
                        .Any(u => u.Id == rolExiste.Id);

                    if (UserTieneRol == false)
                    {
                        User.Rols.Add(rolExiste);
                        _unitOfWork.Users.Update(User);
                        await _unitOfWork.SaveAsync();
                    }

                    return $"Rol {model.Rol} agregado a la cuenta {model.UserName} de forma exitosa.";
                }

                return $"Rol {model.Rol} no encontrado.";
            }

            return $"Credenciales incorrectas para el ususario {User.Username}.";
        }
        public async Task<DatosUsuarioDto> GetTokenAsync(LoginDto model)
        {
            DatosUsuarioDto datosUserDto = new DatosUsuarioDto();
            var User = await _unitOfWork.Users
                            .GetByUsernameAsync(model.UserName);

            if (User == null)
            {
                datosUserDto.EstaAutenticado = false;
                datosUserDto.Mensaje = $"No existe ningun User con el username {model.UserName}.";
                return datosUserDto;
            }

            var result = _passwordHasher.VerifyHashedPassword(User, User.Password, model.Password);
            if (result == PasswordVerificationResult.Success)
            {
                datosUserDto.Mensaje = "OK";
                datosUserDto.EstaAutenticado = true;
                if (User != null)
                {
                    JwtSecurityToken jwtSecurityToken = CreateJwtToken(User);
                    datosUserDto.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                    datosUserDto.UserName = User.Username;
                    datosUserDto.Email = User.Email;
                    datosUserDto.Roles = User.Rols
                                                        .Select(p => p.Nombre)
                                                        .ToList();


                    return datosUserDto;
                }
                else
                {
                    datosUserDto.EstaAutenticado = false;
                    datosUserDto.Mensaje = $"Credenciales incorrectas para el User {User.Username}.";

                    return datosUserDto;
                }
            }

            datosUserDto.EstaAutenticado = false;
            datosUserDto.Mensaje = $"Credenciales incorrectas para el User {User.Username}.";
            return datosUserDto;

        }
        private JwtSecurityToken CreateJwtToken(User User)
        {
            if (User == null)
            {
                throw new ArgumentNullException(nameof(User), "El User no puede ser nulo.");
            }

            var Rols = User.Rols;
            var roleClaims = new List<Claim>();
            foreach (var role in Rols)
            {
                roleClaims.Add(new Claim("Rols", role.Nombre));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, User.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("uid", User.Id.ToString())
            }
            .Union(roleClaims);

            if (string.IsNullOrEmpty(_jwt.Key) || string.IsNullOrEmpty(_jwt.Issuer) || string.IsNullOrEmpty(_jwt.Audience))
            {
                throw new ArgumentNullException("La configuración del JWT es nula o vacía.");
            }

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));

            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature);

            var JwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                signingCredentials: signingCredentials);

            return JwtSecurityToken;
        }

    }