FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 5000

# build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
COPY wait-for.sh .
ENTRYPOINT ["dotnet", "WebApi.dll"]
