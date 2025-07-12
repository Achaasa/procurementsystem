# Use the official .NET 9.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the official .NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["procurementsystem.csproj", "./"]
RUN dotnet restore "procurementsystem.csproj"

# Copy the rest of the source code
COPY . .
WORKDIR "/src/."

# Build the application
RUN dotnet build "procurementsystem.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "procurementsystem.csproj" -c Release -o /app/publish

# Final stage/image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Set environment variables for production
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

ENTRYPOINT ["dotnet", "procurementsystem.dll"] 