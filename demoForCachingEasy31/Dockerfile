#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["demoForCachingEasy31/demoForCachingEasy31.csproj", "demoForCachingEasy31/"]
RUN dotnet restore "demoForCachingEasy31/demoForCachingEasy31.csproj"
COPY . .
WORKDIR "/src/demoForCachingEasy31"
RUN dotnet build "demoForCachingEasy31.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "demoForCachingEasy31.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "demoForCachingEasy31.dll"]