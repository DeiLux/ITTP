FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /app
COPY ./ ./
RUN dotnet publish "src/ITTP.Server/ITTP.Server.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
ENV ASPNETCORE_URLS=http://*:5000
EXPOSE 5000
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ITTP.Server.dll"]