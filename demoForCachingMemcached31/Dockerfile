#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["demoForCachingMemcached31/demoForCachingMemcached31.csproj", "demoForCachingMemcached31/"]
RUN dotnet restore "demoForCachingMemcached31/demoForCachingMemcached31.csproj"
COPY . .
WORKDIR "/src/demoForCachingMemcached31"
RUN dotnet build "demoForCachingMemcached31.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "demoForCachingMemcached31.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "demoForCachingMemcached31.dll"]