#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dot.Api/Dot.API.csproj", "Dot.Api/"]
COPY ["HttpUtilities/HttpUtilities.csproj", "HttpUtilities/"]
COPY ["Dot.Models/Dot.Models.csproj", "Dot.Models/"]
COPY ["Dot.DataAccess/Dot.DataAccess.csproj", "Dot.DataAccess/"]
COPY ["Dot.Services/Dot.Services.csproj", "Dot.Services/"]
COPY ["Dot.UI.Models/Dot.UI.Models.csproj", "Dot.UI.Models/"]
RUN dotnet restore "./Dot.Api/./Dot.API.csproj"
COPY . .
WORKDIR "/src/Dot.Api"
RUN dotnet build "./Dot.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Dot.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Dot.API.dll"]