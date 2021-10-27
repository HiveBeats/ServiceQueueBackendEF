FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 5000

# build
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["WebApi.csproj", "./"]
RUN dotnet restore "./WebApi.csproj"
COPY . .
#WORKDIR "/src/"
RUN dotnet build "WebApi.csproj" -c Release -o /app/build

# publish
FROM build AS publish
RUN dotnet publish "WebApi.csproj" -c Release -o /app/publish

# prod
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
