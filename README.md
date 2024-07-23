# Financial Service

## Setup

1. Clone the repository
2. Navigate to the project directory
3. Run `dotnet restore` to install dependencies
4. Run `dotnet run` to start the service

## Environment Variables

- **TiingoServiceSettings:Authorization**: Set this environment variable with your Tiingo API key.

## REST API

- **GET /api/financialinstruments**: Get a list of available financial instruments.
- **GET /api/financialinstruments/{symbol}**: Get the current price of a specific financial instrument.

## WebSocket

- Connect to `wss://localhost:7171/ws` to subscribe to price updates.
- Send a message with the format: `{"ticker": "EURUSD"}` to subscribe to a specific symbol.

## Note

Replace `localhost` with the appropriate server address if deploying remotely.

## Symbol Validation

Supported tickers:
- EURUSD
- USDJPY
- BTCUSD
