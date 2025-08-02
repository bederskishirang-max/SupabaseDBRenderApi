FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ✅ Fix: Correct path to your csproj
COPY ["PostSQLgreAPI.csproj", "."]
RUN dotnet restore "PostSQLgreAPI.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet publish "PostSQLgreAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostSQLgreAPI.dll"]
