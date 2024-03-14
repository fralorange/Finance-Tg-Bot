FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

WORKDIR /app

COPY *.sln .
COPY src/FreelanceBotBase.Bot/*.csproj ./src/FreelanceBotBase.Bot/
COPY src/FreelanceBotBase.Domain/*.csproj ./src/FreelanceBotBase.Domain/
COPY src/FreelanceBotBase.Contracts/*.csproj ./src/FreelanceBotBase.Contracts/
COPY src/FreelanceBotBase.Infrastructure/*.csproj ./src/FreelanceBotBase.Infrastructure/
COPY src/FreelanceBotBase.Infrastructure.DataAccess/*.csproj ./src/FreelanceBotBase.Infrastructure.DataAccess/
COPY src/FreelanceBotBase.Infrastructure.ComponentRegistrar/*.csproj ./src/FreelanceBotBase.Infrastructure.ComponentRegistrar/
RUN dotnet restore *.sln

COPY src/FreelanceBotBase.Bot/. ./src/FreelanceBotBase.Bot/
COPY src/FreelanceBotBase.Domain/. ./src/FreelanceBotBase.Domain/
COPY src/FreelanceBotBase.Contracts/. ./src/FreelanceBotBase.Contracts/
COPY src/FreelanceBotBase.Infrastructure/. ./src/FreelanceBotBase.Infrastructure/
COPY src/FreelanceBotBase.Infrastructure.DataAccess/. ./src/FreelanceBotBase.Infrastructure.DataAccess/
COPY src/FreelanceBotBase.Infrastructure.ComponentRegistrar/. ./src/FreelanceBotBase.Infrastructure.ComponentRegistrar/
RUN dotnet publish src/FreelanceBotBase.Bot/FreelanceBotBase.Bot.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "FreelanceBotBase.Bot.dll"]