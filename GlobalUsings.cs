global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.AspNetCore.Identity.EntityFrameworkCore; // Identity with EF Core store used in AppDbContext
global using Microsoft.AspNetCore.Mvc;
global using AutoMapper;
global using SchoolApp.Data; // DbContext access everywhere
global using SchoolApp.Models.Entities; // Access entities globally
global using Microsoft.AspNetCore.Identity; // Identity core classes  it add all the rolles and user management features - then create controllers and views for registration and login
global using Microsoft.AspNetCore.Authorization;

global using Microsoft.IdentityModel.Tokens;
global using Microsoft.EntityFrameworkCore; // For DbSet and EF features
global using System.Text;


global using SchoolApp.Models.Identity;

global using SchoolApp.DTOs;

global using SchoolApp.ViewModels.Courses;

global using SchoolApp.Services.Interfaces;

global using SchoolApp.Models.Common; // For ServiceResult<T> and other common models

global using SchoolApp.Repositories.Interfaces; // For IUnitOfWork and IR
                                                // Repository interfaces
global using SchoolApp.Common; // ✅ ApiResponse<T> now needs this


global using SchoolApp.ViewModels.Dashboard;

