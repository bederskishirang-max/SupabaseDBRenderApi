FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ Replace with your correct folder and project name
COPY ["PostSQLgreAPI/PostSQLgreAPI.csproj", "PostSQLgreAPI/"]
RUN dotnet restore "PostSQLgreAPI/PostSQLgreAPI.csproj"

COPY . .
WORKDIR "/src/PostgreApi"
RUN dotnet publish "PostSQLgreAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostSQLgreAPI.dll"]
