## Onboarding
* This project requires [Node](https://nodejs.org/en/download) >= 22, although latest LTS is recommended.
* Install `pnpm`: `npm install -g pnpm`

## Usage

Those templates dependencies are maintained via [pnpm](https://pnpm.io) via `pnpm up -Lri`.

```bash
$ pnpm install
```

### Learn more on the [Solid Website](https://solidjs.com) and come chat with us on our [Discord](https://discord.com/invite/solidjs)

## Available Scripts

In the project directory, you can run:

### `pnpm dev` or `pnpm start`

Runs the app in the development mode.<br>
Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

The page will reload if you make edits.<br>

### `pnpm run build`

Builds the app for production to the `dist` folder.<br>
It correctly bundles Solid in production mode and optimizes the build for the best performance.

The build is minified and the filenames include the hashes.<br>
Your app is ready to be deployed!

## Deployment

You can deploy the `dist` folder to any static host provider (netlify, surge, now, etc.)

## Generated API types

We have the ability to generate type-safe clients and models for the api using `openapi-ts`. These are contained in `./src/api`

### Usage

```typescript
// Import generated types
import type { MessageDto } from '@api';

// Import generated clients
import { getApiV1ChannelByIdMessages } from "@api";

const response = await getApiV1ChannelByIdMessages({
  path: { id: "myChannelId" },
  query: {
    LastMessageId: "MessageId",
  },
});

```

### Regenerating

1. Run the server. Default configuration points to localhost:5194
2. Run the generation command: `pnpm generate`. This will update all types in `src/api`
