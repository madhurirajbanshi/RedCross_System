# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore
COPY *.csproj ./
RUN dotnet restore

# Copy everything else
COPY . ./

# Publish the app
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published files
COPY --from=build /app/publish ./

# Copy SQLite database file (adjust path if different)
COPY ./redcross.db ./

ENTRYPOINT ["dotnet", "RedCross_System.dll"]
