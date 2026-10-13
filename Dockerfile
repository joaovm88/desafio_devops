# ---- Build (SDK) ----
FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app
COPY src/EcoCity.Api/EcoCity.Api.csproj src/EcoCity.Api/
RUN dotnet restore src/EcoCity.Api/EcoCity.Api.csproj
COPY src/ src/
RUN dotnet publish src/EcoCity.Api/EcoCity.Api.csproj -c Release -o /out --no-restore

# ---- Runtime (imagem enxuta, usuário não-root) ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
RUN apk add --no-cache curl
COPY --from=build /out .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
USER $APP_UID
HEALTHCHECK --interval=15s --timeout=3s --retries=5 CMD curl -fs http://localhost:8080/health || exit 1
ENTRYPOINT ["dotnet", "EcoCity.Api.dll"]
