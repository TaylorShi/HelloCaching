#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["demoForCachingRedis31/demoForCachingRedis31.csproj", "demoForCachingRedis31/"]
RUN dotnet restore "demoForCachingRedis31/demoForCachingRedis31.csproj"
COPY . .
WORKDIR "/src/demoForCachingRedis31"
RUN dotnet build "demoForCachingRedis31.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "demoForCachingRedis31.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "demoForCachingRedis31.dll"]