using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Formatting.Compact;
using System.Reflection;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


#region Lendo o appsettings
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

string pathAppsettings = "appsettings.json";

if (env == "Development")
{
    pathAppsettings = "appsettings.Development.json";
}

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile(pathAppsettings)
    .Build();

Environment.SetEnvironmentVariable("STRING_CONEXAO", config.GetSection("stringConexao").Value);
#endregion

#region Configurando Log
var logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs");
Directory.CreateDirectory(logFolder);


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()
    .WriteTo.File(new CompactJsonFormatter(),
           Path.Combine(logFolder, ".json"),
            retainedFileCountLimit: 10,
            rollingInterval: RollingInterval.Day)
    .WriteTo.File(Path.Combine(logFolder, ".log"),
            retainedFileCountLimit: 10,
            rollingInterval: RollingInterval.Day)
    .WriteTo.MySQL(
        connectionString: Environment.GetEnvironmentVariable("STRING_CONEXAO"),
        tableName: "Logs")
    .CreateLogger();

#endregion




var builder = WebApplication.CreateBuilder(args);

#region Habilitando o uso do Swagger (Projeto)

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GATEWAY DE PAGAMENTO",
        Version = "v1",
        Description = $@"<h3>GATEWAY DE<b> PAGAMENTO</b></h3>
                                      <p>
                                          Aplica��o em ASP.NET Core API.
                                      </p>",
        Contact = new OpenApiContact
        {
            Name = "Suporte Unoeste",
            Email = string.Empty,
            Url = new Uri("https://www.unoeste.br"),
        },
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor, insira o token JWT no formato *Bearer {token}*",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    // Adiciona a seguran�a ao Swagger
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
 }
});

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
}); ;

#endregion



builder.Services
    .AddAuthentication(x =>
    {
        //Especificando o Padr�o do Token

        //para definir que o esquema de autentica��o que queremos utilizar � o Bearer e o
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

        //Diz ao asp.net que utilizamos uma autentica��o interna,
        //ou seja, ela � gerada neste servidor e vale para este servidor apenas.
        //N�o � gerado pelo google/fb
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(x =>
    {
        //Lendo o Token

        // Obriga uso do HTTPs
        x.RequireHttpsMetadata = false;


        // Configura��es para leitura do Token
        x.TokenValidationParameters = new TokenValidationParameters
        {
            // Chave que usamos para gerar o Token
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("d8Nix-yreGa-Qopla-xithu-Wenvy-Oplim-xetyF")),
            ValidAudience = "Usu�rios da API",
            ValidIssuer = "Unoeste",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
        };
    });

//pol�tica
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("APIAuth", new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser().Build());
});

//Habilitar o uso do serilog.
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();

#region IOC 

//adicionado ao IOC por requisi��o
builder.Services.AddScoped(typeof(Projeto1Bimestre.Services.CartaoService));
builder.Services.AddScoped(typeof(Projeto1Bimestre.Services.PagamentoService));
//adicionar ao IOC inst�ncia �nicas (singleton)
builder.Services.AddSingleton<Projeto1Bimestre.BD>(new Projeto1Bimestre.BD());


#endregion




var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    c.RoutePrefix = ""; //habilitar a p�gina inicial da API ser a doc.
    c.DocumentTitle = "Gateway de Pagamento - API V1";
});

// Configure the HTTP request pipeline.

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

//Cont�m as Afirma��o (claims) do token.
//As Claimns s�o declara��es sobre uma entidade (geralmente o usu�rio) e metadados adicionais.
//S�o objetos do tipo �Chave/Valor� que definem uma afirma��o,
//como por exemplo, dizer que o nome do usu�rio � "Andr�".
var userClaims = new List<Claim>();
userClaims.Add(new Claim(ClaimTypes.Name, "humberto.stuani")); //Claim padr�o
userClaims.Add(new Claim(ClaimTypes.Role, "Administrador")); //Claim padr�o
userClaims.Add(new Claim("id", "123456"));
userClaims.Add(new Claim("cpf", "11111111111")); //sens�vel (?)  / pode-se criptografar
userClaims.Add(new Claim("data", DateTime.Now.ToString()));
userClaims.Add(new Claim("email", "humbertostuani@hotmail.com"));
userClaims.Add(new Claim("contratoId", "11111111"));

//Juntar as Claims em um conjunto.
var identidade = new ClaimsIdentity(userClaims);
//Iniciando a cri���o do token
var handler = new JwtSecurityTokenHandler();

//Cria��o da Chave de Assinatura (Signing Key):
string minhaKey = "d8Nix-yreGa-Qopla-xithu-Wenvy-Oplim-xetyF"; // >= 32 caracteres
SecurityKey key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.ASCII.GetBytes(minhaKey));
SigningCredentials signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

//criando um descriptor: Objeto que reune o cabe�alho, payload e assinatura do token.
var tokenDescriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
{
    Audience = "Usu�rios da API", //Quem vai usar o token, p�blico-alvo
    Issuer = "Unoeste", //Emisssor
    NotBefore = DateTime.Now, //Data de in�cio
    Expires = DateTime.Now.AddYears(1), //Data fim 
    Subject = identidade, // credenciais de assinatura + Claims
    SigningCredentials = signingCredentials //a chave para criptografar os dados
};

//Cria��o do Token JWT
var dadosToken = handler.CreateToken(tokenDescriptor);

//gerando o token (encripta e gera ao token)
string jwt = handler.WriteToken(dadosToken);
Console.Write(jwt);


//.......
app.Run();
