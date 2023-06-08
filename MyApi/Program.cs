#region usings
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using DataAccess.DAL;
using DataAccess.DAL.Book_related;
using DataAccess.DAL.Common;
using DataAccess.DAL.Security;
using DataAccess.DAL.Transaction;
using DataAccess.Interface;
using DataAccess.Interface.Book_related;
using DataAccess.Interface.Security;
using DataAccess.Interface.Transaction;
using DataAccess.Tool;
using DataModel.ViewModel.Common;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyApi.Helpers;
using NSwag.Generation.Processors.Security;
using Serilog;
using Services.Helpers;
#endregion


#region builder
var builder = WebApplication.CreateBuilder(args);
{
    //1.Log4Net
    //2.Microsoft.Extensions.Logging.Log4Net.AspNetCore
    builder.Logging.AddLog4Net("log4net.Config");
}

//WebApplicationBuilder builder = WebApplication.CreateBuilder(args);//var builder=>update==> WebApplicationBuilder builder
//{
//    //1.Log4Net
//    //2.Microsoft.Extensions.Logging.Log4Net.AspNetCore
//    builder.Logging.AddLog4Net("log4net.Config");
//}
#endregion

#region Services
// Add services to the container.
//builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<DbEntityObject>();
builder.Services.AddScoped<IBookDal, BookDal>();
builder.Services.AddScoped<IWalletDal, WalletDal>();
builder.Services.AddScoped<IEmailDal, EmailDal>();
builder.Services.AddScoped<IMessagesDal, MessagesDal>();
builder.Services.AddScoped<IRolePolicyDal, RolePolicyDal>();
builder.Services.AddScoped<IReviewDal, ReviewDal>();
builder.Services.AddScoped<IInviteHistoryDal, InviteHistoryDal>();
builder.Services.AddScoped<IAuthorBookDal, AuthorBookDal>();
builder.Services.AddScoped<ITranslatorBookDal, TranslatorBookDal>();
builder.Services.AddScoped<IBookSubjectDal, BookSubjectDal>();
builder.Services.AddScoped<IBookCategoryDal, BookCategoryDal>();
builder.Services.AddScoped<IFileSystem, FileSystemDal>();
builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<ISubjectDal, SubjectDal>();
builder.Services.AddScoped<IUser, UserDal>();
builder.Services.AddScoped<IAuthorDal, AuthorDal>();
builder.Services.AddScoped<ITranslatorDal, TranslatorDal>();
builder.Services.AddScoped<IResourcesDal, ResourcesDal>();
builder.Services.AddScoped<IRoleDal, RoleDal>();
builder.Services.AddScoped<IRoleMemberDal, RoleMemberDal>();
builder.Services.AddScoped<IRolePolicyDal, RolePolicyDal>();
builder.Services.AddScoped<IUserToken, UserTokenDal>();
builder.Services.AddSingleton<ISecurityService, SecurityService>();
builder.Services.AddScoped<ITokenStoreService, TokenStoreService>();
builder.Services.AddScoped<ITokenValidatorService, TokenValidatorService>();
builder.Services.AddScoped<ITokenFactoryService, TokenFactoryService>();
builder.Services.AddScoped<ICurrencyRateDal, CurrencyRateDal>();
builder.Services.AddScoped<ICurrencyDal, CurrencyDal>();
builder.Services.AddScoped<IGatewayDal, GatewayDal>();
builder.Services.AddScoped<ITransactionInfoDal, TransactionInfoDal>();
builder.Services.AddScoped<IDepositRequestDal, DepositRequestDal>();
builder.Services.AddScoped<IWithdrawalDal, WithdrawalDal>();
builder.Services.AddScoped<IUserBookDal, UserBookDal>();
builder.Services.AddScoped<IRateDal, RateDal>();
builder.Services.AddScoped<ILikeDal, LikeDal>();

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddMemoryCache();

//builder.Services.AddMvc(options =>
//{
//    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

//});

#endregion

builder.Services.Configure<IISServerOptions>(options => {
    options.AllowSynchronousIO = true;
});


#region FluentValidation

builder.Services.AddControllers()
    .AddFluentValidation(s => {
        s.RegisterValidatorsFromAssemblyContaining<ConfigurationHelper>();
        s.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
    });

#endregion

#region Setup Token

builder.Services.Configure<BearerTokensOptions>(options => builder.Configuration.GetSection("BearerTokens").Bind(options));

//builder.Services.AddAuthorization(options =>
//{
//    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
//    options.AddPolicy("User", policy => policy.RequireRole("User"));
//    options.AddPolicy("Editor", policy => policy.RequireRole("Editor"));
//});


builder.Services.AddAuthorization(options =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme);

    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

    options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
});

builder.Services.AddAuthentication(options => {
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(cfg => {
    cfg.RequireHttpsMetadata = false;
     cfg.SaveToken = true;
     cfg.TokenValidationParameters = new TokenValidationParameters
     {
         ValidIssuer = builder.Configuration["BearerTokens:Issuer"], // site that makes the token
         ValidateIssuer = false, // TODO: change this to avoid forwarding attacks
         ValidAudience = builder.Configuration["BearerTokens:Audience"], // site that consumes the token
         ValidateAudience = false, // TODO: change this to avoid forwarding attacks
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["BearerTokens:Key"])),
         ValidateIssuerSigningKey = true, // verify signature to avoid tampering
         ValidateLifetime = true, // validate the expiration
         ClockSkew = TimeSpan.Zero // tolerance for the expiration date
     };
     cfg.Events = new JwtBearerEvents
     {
         OnAuthenticationFailed = context => Task.CompletedTask,

         OnMessageReceived = context => {
             if (string.IsNullOrEmpty(context.Token))
             {
                 var accessToken = context.Request.Query["token"];
                 var path = context.HttpContext.Request.Path;

                 if (!string.IsNullOrEmpty(accessToken) &&
                     (path.StartsWithSegments("/gameHub") || path.StartsWithSegments("/panelHub")))
                 {
                     context.Token = accessToken;
                 }
             }

             return Task.CompletedTask;
         },

         OnTokenValidated = context => {
             var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
             var accessToken = context.SecurityToken as JwtSecurityToken;
             return tokenValidatorService.ValidateAsync(context);
         },
         OnChallenge = context => {
             return Task.CompletedTask;
         }
     };
 });

//builder.Services.AddOptions<BearerTokensOptions>()
//              .Bind(builder.Configuration.GetSection("BearerTokens"))
//              .Validate(
//                  bearerTokens =>
//                      bearerTokens.AccessTokenExpirationMinutes < bearerTokens.RefreshTokenExpirationMinutes,
//                  "RefreshTokenExpirationMinutes is less than AccessTokenExpirationMinutes. Obtaining new tokens using the refresh token should happen only if the access token has expired.");
//builder.Services.AddOptions<ApiSettings>()
//        .Bind(builder.Configuration.GetSection("ApiSettings"));

//*****************************************************************************************************************************
//builder.Services.AddAuthentication("bearer")
//.AddJwtBearer("bearer", options => {
//    options.Authority = builder.Configuration["Authority"];
//    options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
//    {
//        OnMessageReceived = context => {

//            var accessToken = context.Request.Query["access_token"];

//            var path = context.HttpContext.Request.Path;
//            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chathub")))
//            {
//                context.Token = accessToken;
//            }
//            return Task.CompletedTask;
//        },
//        OnTokenValidated = context => {
//            var tokenValidatorService = context.HttpContext.RequestServices.GetRequiredService<ITokenValidatorService>();
//            return tokenValidatorService.ValidateAsync(context);
//        }
//    };
//});
//******************************************************************************************************************************

#endregion

#region cors
builder.Services.AddCors(options => {
    options.AddPolicy("AllowOrigins",
        builder => builder
            .WithOrigins("http://localhost:55688", "http://localhost:2807", "http://localhost:4200", "http://localhost:4300") //Note:  The URL must be specified without a trailing slash (/).
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed((host) => true)
            .AllowCredentials());
});
#endregion

#region Swagger
//builder.Services.AddSwaggerGen(c => {
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "MyApi", Version = "v1" });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter the token in the field",
//        Name = "Authorization",
//        Type = SecuritySchemeType.ApiKey
//    });
//    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
//                    {
//                        new OpenApiSecurityScheme
//                        {
//                            Reference = new OpenApiReference
//                            {
//                                Type = ReferenceType.SecurityScheme,
//                                Id = "Bearer"
//                            }
//                        },
//                        new string[] { }
//                    }
//                });
//});


builder.Services.AddOpenApiDocument(config => {
    config.OperationProcessors.Add(new OperationSecurityScopeProcessor("JWT token"));
    config.AddSecurity("JWT token", new NSwag.OpenApiSecurityScheme
    {
        Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
        Name = "Authorization",
        Description = "Copy 'Bearer ' + valid JWT token into field",
        In = NSwag.OpenApiSecurityApiKeyLocation.Header,
    });
    config.PostProcess = (document) => {
        document.Info.Version = "v1.1.1";
        document.Info.Title = "PokeNet API";
        document.Info.Description = "Developed with ASP.NET Core 3.1";
    };
});

#endregion

#region Configuration Serilog
var configuration = new ConfigurationBuilder()
    // Read from your appsettings.json.
    .AddJsonFile("appsettings.json")
    // Read from your secrets.
    .AddUserSecrets<Program>(optional: true)
    .AddEnvironmentVariables()
    .Build();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

#endregion

#region app
var app = builder.Build();


app.UseSerilogRequestLogging();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUi3();
    //app.UseSwagger();
    app.UseDeveloperExceptionPage();
    app.UseHttpsRedirection();
    app.UseSwaggerUI();
}
else
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseExceptionHandler("/Home/Error");
}
//app.UseSwaggerUi3();

#endregion

#region appUses
app.UseOpenApi();
app.UseCors("AllowOrigins");
app.UseHttpsRedirection();
app.UseAuthorization();
app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
});

ConfigurationHelper.Configure(app.Configuration);
app.MapControllers();
app.Run();
#endregion