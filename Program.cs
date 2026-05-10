using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchoolApp.Common.Seed;
using SchoolApp.Filters;
using SchoolApp.Mappings;
using SchoolApp.Middlewares;
using SchoolApp.Models.Identity;
using SchoolApp.Repositories.Implementations;
using SchoolApp.Services.Implementations;
using SchoolApp.Validators.Students;
using Serilog;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// =========================
// 🪵 SERILOG
// =========================
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// =========================
// 📊 SWAGGER
// =========================
// ✅ AFTER — Swagger with JWT Bearer support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Basic API info shown at the top of Swagger UI
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "SchoolApp API",
        Version = "v1",
        Description = "REST API for SchoolApp — manage students, teachers, courses and enrollments"
    });

    // ✅ Tell Swagger that this API uses Bearer token authentication
    // WHY: Without this, Swagger UI has no Authorize button.
    //      You can't set the JWT token, so every protected endpoint
    //      returns 401 and you can't test anything meaningful.
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        // This is the header Swagger will send: Authorization: Bearer {token}
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token below.\n\nGet a token first from: POST /api/auth/login\n\nThen paste it here — no need to type 'Bearer', it's added automatically."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
        Array.Empty<string>()
    }
});

});



// =========================
// 🗄️ DATABASE
// =========================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// =========================
// 🔐 IDENTITY (ONLY ONCE)
// =========================
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // ❌ BEFORE — dangerously weak, min length 4, no complexity
    // This allows passwords like "1234" or "aaaa" — unacceptable in production

    // ✅ AFTER — reasonable minimum for a school management system
    options.Password.RequireDigit = true;
    // WHY: Forces at least one number — "password1" beats "password"

    options.Password.RequireLowercase = true;
    // WHY: Forces mixed case awareness

    options.Password.RequireUppercase = false;
    // WHY: Research shows forcing uppercase just makes users do "Password1"
    //      which is no stronger. Keep it false to avoid frustrating users.

    options.Password.RequireNonAlphanumeric = false;
    // WHY: Special chars cause copy-paste issues on mobile.
    //      Length matters more than special chars for real security.

    options.Password.RequiredLength = 8;
    // WHY: 8 is the industry minimum. NIST recommends 8+ over complexity rules.
    //      Going from 4 to 8 makes brute force exponentially harder.

    options.Password.RequiredUniqueChars = 4;
    // WHY: Prevents "aaaaaaaa" or "11111111" — forces some variety
    //      without being annoying to users.

    // ✅ Lockout — already good, keep it
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
    // WHY: After 3 wrong attempts, account locks for 15 minutes.
    //      This stops brute force attacks completely.
    //      An attacker can only try 3 passwords per 15 minutes = 12/hour.
    //      Cracking an 8-char password at that rate would take centuries.
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();
// =========================
// 🍪 COOKIE SETTINGS
// =========================
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";

    // API → return status code instead of redirect
    options.Events.OnRedirectToLogin = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        else
            context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    };

    options.Events.OnRedirectToAccessDenied = context =>
    {
        if (context.Request.Path.StartsWithSegments("/api"))
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
        else
            context.Response.Redirect(context.RedirectUri);

        return Task.CompletedTask;
    };
});

// =========================
// 🔑 JWT AUTH (API ONLY)
// =========================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
    throw new InvalidOperationException(
        "JWT key is not configured. Set Jwt:Key in appsettings.Development.json or as an environment variable.");

builder.Services.AddAuthentication()
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey)
            )
        };
    });

// =========================
// 🎯 MVC + FILTERS
// =========================
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<LogActionFilter>();
});

// =========================
// ✅ FLUENT VALIDATION
// =========================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<StudentCreateValidator>();

// =========================
// 🔄 AUTOMAPPER
// =========================
builder.Services.AddAutoMapper(typeof(StudentProfile).Assembly);

// =========================
// 🌐 HTTP CLIENT
// =========================
builder.Services.AddHttpClient();

// =========================
// 🧩 DEPENDENCY INJECTION
// =========================
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<ITeacherCourseService, TeacherCourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAppLogger, AppLogger>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IParentService, ParentService>();
builder.Services.AddApplicationInsightsTelemetry();

//Docker Health Checks it is a good practice to add health checks to your application, especially when running in a containerized environment like Docker. Health checks allow you to monitor the health of your application and ensure that it is running correctly. You can configure health checks to check the status of your application's dependencies, such as the database connection, or to perform custom checks specific to your application's functionality.
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDbContext>();

// =========================
// 🚀 BUILD APP
// =========================
var app = builder.Build();

// =========================
// 🌱 SEED DATABASE
// =========================

app.Use(async (context, next) =>
{
    var requestId = context.TraceIdentifier;

    using (Serilog.Context.LogContext.PushProperty("RequestId", requestId))
    {
        await next();
    }
});

app.UseSerilogRequestLogging();

// =========================
// ⚙️ MIDDLEWARE PIPELINE
// =========================

// 1. Global exception handler — always first so it catches all errors
app.UseMiddleware<ExceptionHandlingMiddleware>();

// 2. Swagger — enabled via config flag (set Swagger:Enabled=true in appsettings)
if (app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "SchoolApp API v1");
        options.DisplayRequestDuration();
    });
}



if (!app.Environment.IsDevelopment())
{
    
    // 👇 Security Header
    app.UseHsts();
}
else
{
    // 👇 في Development عادي نشوف الخطأ
    app.UseDeveloperExceptionPage();
}
// 3. Core pipeline

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. Auth — order matters: Authentication before Authorization
app.UseAuthentication();
// 5. Health Checks endpoint — no auth, no logging, no custom middleware
app.MapHealthChecks("/health");
app.UseMiddleware<ActiveUserMiddleware>();
app.UseAuthorization();

// =========================
// 🧭 ENDPOINTS
// =========================
// MVC conventional routes must be registered before MapControllers
// so that /TeacherCourses hits the MVC controller, not the API catch-all
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllers();

using (var scope = app.Services.CreateScope())  // ✅ Ensure database is created and seeded on startup
{
    var services = scope.ServiceProvider;  // ✅ Get AppDbContext from DI

    var db = services.GetRequiredService<AppDbContext>();   // ✅ Get the DbContext

    // 1. Create database and apply migrations first
    await db.Database.MigrateAsync();

    // 2. Then seed roles, admin, test data
    await DbSeeder.SeedAsync(services);
}

// =========================
// DATABASE SEEDING
// =========================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    await DbSeeder.SeedAsync(services);
}

app.Run();