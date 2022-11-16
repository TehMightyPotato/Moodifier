FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["PnP.Moodifier.Server/PnP.Moodifier.Server.csproj", "PnP.Moodifier.Server/"]
RUN dotnet restore "PnP.Moodifier.Server/PnP.Moodifier.Server.csproj"
COPY . .
WORKDIR "/src/PnP.Moodifier.Server"
RUN dotnet build "PnP.Moodifier.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PnP.Moodifier.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PnP.Moodifier.Server.dll"]
