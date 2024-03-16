
using ErmitApi.DAL.Models;
using ErmitApi.DAL;
using ErmitApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using NUlid;

namespace ErmitApi.BLL;


public class AuthService
{
    // private IConnectionMultiplexer RedisConnection { get; }
    private ILogger<AuthService> Logger { get; }
    private IConfiguration Configuration { get; }
    private JwtSecurityTokenHandler TokenHandler { get; }
    private IServiceProvider ServiceProvider { get; }
    private SigningCredentials SigningCredentials { get; }
    private readonly RSA rsa;

    private readonly int TokenLifeTime = 60;

    public AuthService(ILogger<AuthService> logger,
            // IConnectionMultiplexer connection, 
            IConfiguration configuration,
            IServiceProvider serviceProvider)
    {
        Logger = logger;
        // RedisConnection = connection;
        Configuration = configuration;
        ServiceProvider = serviceProvider;

        TokenHandler = new JwtSecurityTokenHandler();

        TokenLifeTime = Configuration.GetValue<int>("Jwt:LifeTime");

        rsa = RSA.Create(4096);
        rsa.ImportRSAPrivateKey( // Convert the loaded key from base64 to bytes.
                source: Convert.FromBase64String(Configuration["Jwt:PrivateKey"]), // Use the private key to sign tokens
                bytesRead: out int _);

        SigningCredentials = new SigningCredentials(
        key: new RsaSecurityKey(rsa),
                algorithm: SecurityAlgorithms.RsaSha256
            );
    }

    // генерация случайной соли
    private static string GenerateSalt(int size = 16)
    {
        byte[] saltBytes = new byte[size];
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(saltBytes);
        }
        return Convert.ToBase64String(saltBytes);
    }

    // Вычисление хэша пароля с использованием соли
    private static string ComputeHash(string password, string salt)
    {
        byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
        byte[] saltBytes = Convert.FromBase64String(salt);

        using (var sha256 = SHA256.Create())
        {
            byte[] combinedBytes = new byte[passwordBytes.Length + saltBytes.Length];
            Buffer.BlockCopy(passwordBytes, 0, combinedBytes, 0, passwordBytes.Length);
            Buffer.BlockCopy(saltBytes, 0, combinedBytes, passwordBytes.Length, saltBytes.Length);

            byte[] hashBytes = sha256.ComputeHash(combinedBytes);
            return Convert.ToBase64String(hashBytes);
        }
    }

    private static bool CheckPassword(string inputPassword, User user)
    {
        string salt = user.Salt;
        string passwordHash = user.PasswordHash;

        // вычисляем хэш входного пароля пароля
        string inputPasswordHash = ComputeHash(inputPassword, salt);

        return inputPasswordHash.Equals(passwordHash);
    }

    public async Task<string> GenerateTokenAsync(User user, Guid sessionId)
    {
        List<Claim> claims = new()
        {
            new Claim("session", sessionId.ToString()),
            new Claim("user_info", JsonConvert.SerializeObject(new UserInfo(user)))
        };

        var token = new JwtSecurityToken(
                issuer: "ERMIT",
                audience: "REST_CLIENT",
                expires: DateTime.Now.AddSeconds(TokenLifeTime),
                claims: claims,
                signingCredentials: SigningCredentials
            );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public JwtSecurityToken ParseToken(string token)
    {
        return TokenHandler.ReadJwtToken(token);
    }

    public async Task<string> Login(LoginModel model)
    {
        await using var context = GetContext<DataBaseContext>();

        User dbUser = await context.Users.FirstOrDefaultAsync(user => user.Login == model.Login);

        if (dbUser == null) { throw new ArgumentException($"user с login {model.Login} не существует"); }

        if (!CheckPassword(model.Password, dbUser)) { throw new ArgumentException("некорректный пароль"); }

        Guid newSessionId = Guid.NewGuid();
        string token = await GenerateTokenAsync(dbUser, newSessionId);

        // await SaveSessionCache(newSessionId.ToString(), dbUser.Id, Guid.NewGuid().ToString());

        return token;
    }

    public async Task<string> Registrate(RegistrationModel model)
    {
        if (model.Password != model.RepeatPassword)
            throw new ArgumentException("пароли не совпадают");

        if (model.Password.Length < 6)
            throw new ArgumentException("длина пароля должна быть не менее 6 символов");

        if (string.IsNullOrWhiteSpace(model.Login) || string.IsNullOrEmpty(model.Name))
            throw new ArgumentException("пароль и имя не могут быть пустыми");

        await using var context = GetContext<DataBaseContext>();

        if (await context.Users.AnyAsync(user => user.Login == model.Login))
            throw new ArgumentException("пользователь с таким Login уже существует");

        string salt = GenerateSalt();
        string passwordHash = ComputeHash(model.Password, salt);

        User newUser = new User
        {
            Id = Ulid.NewUlid().ToGuid(),
            Login = model.Login,
            Name = model.Name,
            Salt = salt,
            PasswordHash = passwordHash,
            IsAdmin = model.IsAdmin
        };

        await context.Users.AddAsync(newUser);
        await context.SaveChangesAsync();

        Guid newSessionId = Guid.NewGuid();
        string token = await GenerateTokenAsync(newUser, newSessionId);

        // await SaveSessionCache(newSessionId.ToString(), newUser.Id, Guid.NewGuid().ToString());

        return token;
    }

    public class UserInfo
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsAdmin { get; set; }
        public UserInfo() { }
        public UserInfo(User user)
        {
            Id = user.Id;
            Name = user.Name;
            IsAdmin = user.IsAdmin;
        }
    }


    protected ContextType GetContext<ContextType>()
    where ContextType : DbContext
    {
        return ServiceProvider.GetRequiredService<ContextType>();
    }

}
