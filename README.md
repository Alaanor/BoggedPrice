# BoggedPrice

A small discord bot that display the price of BOG/USD in his username

## Features

- Show price directly in the username
- User can set their wallet address and query it later to show how much they own, both in BOG and USD.

## Build

```bash
docker build -t bogged-price . && docker run --name bogged-price --env-file .env bogged-price
```

## Configuration

Configuration can be done through environment variables.

|Env name   |Description|default|
|-----------|-----------|-------|
|DISCORD_BOT_TOKEN|Set the discord bot token| |
|REFRESH_RATE|In second, the delay between price update|60|
|DB_PATH|The path of the db file|/app/data/db|