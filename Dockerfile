FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code
COPY . .
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
# Copy configuration files
COPY appsettings*.json ./
ENV ASPNETCORE_URLS=http://+:5050
ENTRYPOINT ["dotnet", "ReflexSharp-BE.dll"]
