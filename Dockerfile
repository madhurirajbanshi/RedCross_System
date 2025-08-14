# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY ./RedCross_System/*.csproj ./
RUN dotnet restore

# Copy everything else
COPY ./RedCross_System/ ./

# Publish the app
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish ./

# Copy SQLite database file from the correct location
COPY ./RedCross_System/Database/app.db ./redcross.db

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

EXPOSE 80

ENTRYPOINT ["dotnet", "RedCross_System.dll"]
