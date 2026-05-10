# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

# Copy solution file
COPY *.sln .

# Copy ONLY main application project
COPY SchoolApp/*.csproj ./SchoolApp/

# Restore dependencies
RUN dotnet restore SchoolApp/SchoolApp.csproj

# Copy all application files
COPY SchoolApp/. ./SchoolApp/

# Build & publish
WORKDIR /src/SchoolApp

RUN dotnet publish -c Release -o /app/publish


# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "SchoolApp.dll"]