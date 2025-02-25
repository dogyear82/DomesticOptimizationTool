FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app
COPY ["Dot.Client/Dot.Client.csproj", "Dot.Client/"]
COPY ["Dot.Models/Dot.Models.csproj", "Dot.Models/"]
COPY ["Dot.API.Gateway/Dot.API.Gateway.csproj", "Dot.API.Gateway/"]
COPY ["Dot.Services/Dot.Services.csproj", "Dot.Services/"]
COPY ["Dot.UI.Models/Dot.UI.Models.csproj", "Dot.UI.Models/"]
COPY ["Dot.Utilities/Dot.Utilities.csproj", "Dot.Utilities/"]
RUN dotnet restore "./Dot.Client/./Dot.Client.csproj"
COPY . .
RUN dotnet publish "./Dot.Client/./Dot.Client.csproj" -c Release -o out

FROM nginx:alpine
WORKDIR /app
EXPOSE 7070
RUN rm /etc/nginx/nginx.conf
COPY Dot.Client/nginx.conf /etc/nginx/nginx.conf
COPY --from=build /app/out/wwwroot /usr/share/nginx/html