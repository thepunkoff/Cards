FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["Cards/Cards.csproj", "Cards/"]
RUN dotnet restore "Cards/Cards.csproj"
COPY . .
WORKDIR "/src/Cards"
RUN dotnet build "Cards.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Cards.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Cards.dll"]
