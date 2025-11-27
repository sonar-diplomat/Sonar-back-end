# Base runtime image for running the app
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Create app user and data directory with proper permissions
RUN groupadd -r appuser && useradd -r -g appuser appuser && \
    mkdir -p /app/data && \
    chown -R appuser:appuser /app

USER appuser
EXPOSE 8080
EXPOSE 8081

# Build the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy only csproj files first — improves restore caching
COPY ["Sonar/Sonar.csproj", "Sonar/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Entities/Entities.csproj", "Entities/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Logging/Logging.csproj", "Logging/"]

# Restore once (this layer will be cached until csproj changes)
RUN dotnet restore "./Sonar/Sonar.csproj"

# Copy the full source code
COPY . .
WORKDIR "/src/Sonar"

# Build without doing another restore
RUN dotnet build "./Sonar.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/build \
    --no-restore

# Publish the application (no extra restore/build)
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/Sonar"
RUN dotnet publish "./Sonar.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false \
    --no-restore

# Final image for production
FROM base AS final
WORKDIR /app
# Switch to root temporarily to copy files and set permissions
USER root
COPY --from=publish /app/publish .
# Ensure data directory exists and has correct permissions
RUN mkdir -p /app/data && chown -R appuser:appuser /app
# Switch back to appuser
USER appuser
VOLUME ["/app/data"]
ENTRYPOINT ["dotnet", "Sonar.dll"]
