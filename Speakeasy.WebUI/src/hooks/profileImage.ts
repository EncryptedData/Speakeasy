import { createResource } from "solid-js";

import { getApiV1DownloadById } from "@api";

const cache = new Map<string, string>();
const inflight = new Map<string, Promise<string | undefined>>();

export function clearImageCache() {
  cache.forEach((url) => URL.revokeObjectURL(url));
  cache.clear();
  inflight.clear();
}

async function fetchImage(id: string): Promise<string | undefined> {
  const response = await getApiV1DownloadById({
    path: { id },
    parseAs: "blob",
  });

  if (!(response.data instanceof Blob)) return undefined;

  const objectUrl = URL.createObjectURL(response.data);
  cache.set(id, objectUrl);
  return objectUrl;
}

export function useProfileImage(imageId: () => string | null | undefined) {
  const [url] = createResource(
    () => imageId() ?? undefined,
    async (id) => {
      if (cache.has(id)) return cache.get(id)!;

      if (!inflight.has(id)) {
        inflight.set(
          id,
          fetchImage(id).finally(() => inflight.delete(id)),
        );
      }

      return inflight.get(id)!;
    },
  );

  return url;
}
