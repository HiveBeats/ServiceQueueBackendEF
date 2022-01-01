

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["UserFiller.csproj", "./"]
RUN dotnet restore "./UserFiller.csproj"
COPY . .
#WORKDIR "/src/UserFiller"
RUN dotnet build "UserFiller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserFiller.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY wait-for.sh .
ENTRYPOINT ["dotnet", "UserFiller.dll"]
