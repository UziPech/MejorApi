# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copiamos el contenido del proyecto
COPY . .

# Restauramos el proyecto
RUN dotnet restore "ParInpar/ParInpar.csproj"
RUN dotnet publish "ParInpar/ParInpar.csproj" -c Release -o /app/build

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/build .

ENTRYPOINT ["dotnet", "ParInpar.dll"]
