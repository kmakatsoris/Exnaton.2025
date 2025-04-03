FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Install necessary libraries via apt
RUN apt-get update && apt-get install -y \
    libharfbuzz-dev \
    libharfbuzz0b \
    libfreetype6 \
    libfontconfig1 \
    && rm -rf /var/lib/apt/lists/*

# Set the environment variable to expose the application on port 5142
ENV ASPNETCORE_URLS=http://+:5142

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy Exnaton.csproj and restore dependencies
COPY ./*.csproj ./Exnaton/
RUN dotnet restore ./Exnaton/Exnaton.csproj

# Copy the rest of the source code and build the application
COPY ./ ./Exnaton/
WORKDIR /src/Exnaton
COPY ./logo.png /app/
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Expose the port that your application will listen to
EXPOSE 5142

ENTRYPOINT ["dotnet", "Exnaton.dll"]

