FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /source

# copy csproj and restore as distinct layers
COPY *.csproj .
RUN dotnet restore -r linux-musl-x64

# copy and publish app and libraries
COPY . .
RUN dotnet publish -c release -o /app -r linux-musl-x64 --self-contained false --no-restore

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0-alpine
WORKDIR /app
COPY --from=build /app .

# create folder to store the db
RUN mkdir -p /app/data

# run the bot
ENTRYPOINT ["./BoggedPrice"]
