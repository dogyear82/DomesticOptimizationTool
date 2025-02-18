FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Dot.Client/Dot.Client.csproj", "Dot.Client/"]
COPY ["Dot.Models/Dot.Models.csproj", "Dot.Models/"]
COPY ["Dot.API.Gateway/Dot.API.Gateway.csproj", "Dot.API.Gateway/"]
COPY ["Dot.Services/Dot.Services.csproj", "Dot.Services/"]
COPY ["Dot.UI.Models/Dot.UI.Models.csproj", "Dot.UI.Models/"]
COPY ["Dot.Utilities/Dot.Utilities.csproj", "Dot.Utilities/"]
RUN dotnet restore "./Dot.Client/./Dot.Client.csproj"
COPY . .
WORKDIR "/src/Dot.Client"
RUN dotnet build "./Dot.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "./Dot.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY Dot.Client/nginx.conf /etc/nginx/nginx.conf