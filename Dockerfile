# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["NewProjectFromScratch.csproj", "./"]
RUN dotnet restore "NewProjectFromScratch.csproj"

# Copy the entire project and build
COPY . .
RUN dotnet build "NewProjectFromScratch.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "NewProjectFromScratch.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose the port the app runs on
EXPOSE 5140

# Set environment
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:5140

# Run the application
ENTRYPOINT ["dotnet", "NewProjectFromScratch.dll"]
