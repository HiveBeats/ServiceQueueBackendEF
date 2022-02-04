

FROM mcr.microsoft.com/dotnet/runtime-deps:6.0-alpine-amd64 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS publish
WORKDIR /src
COPY ["UserFiller.csproj", "./"]
RUN dotnet restore "./UserFiller.csproj" -r linux-musl-x64
COPY . .
#WORKDIR "/src/UserFiller"
RUN dotnet publish "UserFiller.csproj" -r linux-musl-x64 --self-contained true -c Release -o /app/publish /p:PublishTrimmed=true /p:PublishReadyToRun=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish ./
COPY wait-for.sh ./
ENTRYPOINT ["./UserFiller"]
