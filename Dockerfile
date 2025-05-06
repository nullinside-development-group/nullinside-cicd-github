FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["src/Nullinside.Cicd.GitHub/Nullinside.Cicd.GitHub.csproj", "Nullinside.Cicd.GitHub/"]
RUN dotnet restore "Nullinside.Cicd.GitHub/Nullinside.Cicd.GitHub.csproj"
COPY . .
WORKDIR "/src/src"
RUN dotnet build "Nullinside.Cicd.GitHub/Nullinside.Cicd.GitHub.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Nullinside.Cicd.GitHub/Nullinside.Cicd.GitHub.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nullinside.Cicd.GitHub.dll"]
