dotnet build -c Release -o build/Server .\Cards.Server\Cards.Server.csproj
dotnet build -c Release -o build/Telegram .\Cards.Telegram\Cards.Telegram.csproj

docker login -u thepunkoff -p kotopes360605811
docker build -t thepunkoff/cardsserver:latest -f .\Cards.Server\Dockerfile .
docker build -t thepunkoff/cardstelegram:latest -f .\Cards.Telegram\Dockerfile .

docker push thepunkoff/cardsserver:latest
docker push thepunkoff/cardstelegram:latest

docker images -a