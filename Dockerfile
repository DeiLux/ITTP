#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["/src/ITTP.Server/ITTP.Server.csproj", "ITTP.Server/"]
COPY ["/src/Datatransfer/ITTP.Datatransfer.HttpDto/ITTP.Datatransfer.HttpDto.csproj", "Datatransfer/ITTP.Datatransfer.HttpDto/"]
COPY ["/src/Database/ITTP.Database.Repositories/ITTP.Database.Repositories.csproj", "Database/ITTP.Database.Repositories/"]
COPY ["/src/ITTP.Core/ITTP.Core.csproj", "ITTP.Core/"]
COPY ["/src/Database/ITTP.Database.Context/ITTP.Database.Context.csproj", "Database/ITTP.Database.Context/"]
COPY ["/src/Database/ITTP.Database.Models/ITTP.Database.Models.csproj", "Database/ITTP.Database.Models/"]
COPY ["/src/Services/ITTP.Services.AuthService/ITTP.Services.AuthService.csproj", "Services/ITTP.Services.AuthService/"]
COPY ["/src/Services/ITTP.Services.UserService/ITTP.Services.UserService.csproj", "Services/ITTP.Services.UserService/"]
RUN dotnet restore "/src/ITTP.Server/ITTP.Server.csproj"
COPY . .
RUN dotnet build "/src/src/ITTP.Server/ITTP.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "/src/src/ITTP.Server/ITTP.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ITTP.Server.dll"]