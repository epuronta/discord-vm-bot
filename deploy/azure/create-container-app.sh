#!/bin/bash
set -ue

az containerapp create \
    --resource-group "$AZURE_RESOURCE_GROUP" \
    --name "$AZURE_CAPP_NAME" \
    --environment "$AZURE_CAPP_ENVIRONMENT" \
    --image "$AZURE_CAPP_IMAGE" \
    --secrets discord-token="$DISCORD_TOKEN" az-cli-id="$AZURE_CLIENT_ID" az-cli-secret="$AZURE_CLIENT_SECRET" az-tenant-id="$AZURE_TENANT_ID" az-vm-id="$AZURE_VIRTUAL_MACHINE_ID" \
    --env-vars DISCORD_TOKEN="secretref:discord-token" AZURE_CLIENT_ID="secretref:az-cli-id" AZURE_CLIENT_SECRET="secretref:az-cli-secret" AZURE_TENANT_ID="secretref:az-tenant-id" AZURE_VIRTUAL_MACHINE_ID="secretref:az-vm-id" \
    --min-replicas 1 --max-replicas 1
