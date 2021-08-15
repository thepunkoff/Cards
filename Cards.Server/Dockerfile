FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine
COPY ../build/Server App/
WORKDIR /App
ENTRYPOINT ["dotnet", "Cards.Server.dll"]