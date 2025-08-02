FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Adjust this if the .csproj file is inside a folder
COPY ["PostSQLgreAPI/PostSQLgreAPI.csproj", "PostSQLgreAPI/"]
RUN dotnet restore "PostSQLgreAPI/PostSQLgreAPI.csproj"

COPY . .
WORKDIR "/src/PostSQLgreAPI"
RUN dotnet publish "PostSQLgreAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "PostSQLgreAPI.dll"]
