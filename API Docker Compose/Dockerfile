FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["Código Fonte/Netrin.Antifraude.slnx", "Código Fonte/"]
COPY ["Código Fonte/Netrin.Antifraude.Core/Netrin.Antifraude.Core.csproj", "Código Fonte/Netrin.Antifraude.Core/"]
COPY ["Código Fonte/Netrin.Antifraude.Infrastructure/Netrin.Antifraude.Infrastructure.csproj", "Código Fonte/Netrin.Antifraude.Infrastructure/"]
COPY ["Código Fonte/Netrin.Antifraude.Application/Netrin.Antifraude.Application.csproj", "Código Fonte/Netrin.Antifraude.Application/"]
COPY ["Código Fonte/Netrin.Antifraude.Api/Netrin.Antifraude.Api.csproj", "Código Fonte/Netrin.Antifraude.Api/"]
COPY ["Código Fonte/Netrin.Antifraude.Worker/Netrin.Antifraude.Worker.csproj", "Código Fonte/Netrin.Antifraude.Worker/"]
RUN dotnet restore "Código Fonte/Netrin.Antifraude.slnx"

COPY ["Código Fonte/", "Código Fonte/"]
RUN dotnet publish "Código Fonte/Netrin.Antifraude.Api/Netrin.Antifraude.Api.csproj" --configuration Release --no-restore --output /out/api /p:UseAppHost=false
RUN dotnet publish "Código Fonte/Netrin.Antifraude.Worker/Netrin.Antifraude.Worker.csproj" --configuration Release --no-restore --output /out/worker /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS api
WORKDIR /app
COPY --from=build /out/api .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "Netrin.Antifraude.Api.dll"]

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS worker
WORKDIR /app
COPY --from=build /out/worker .
ENTRYPOINT ["dotnet", "Netrin.Antifraude.Worker.dll"]
