FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7243

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["web-api.csproj", "."]
RUN dotnet restore "./web-api.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "web-api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "web-api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web-api.dll"]
