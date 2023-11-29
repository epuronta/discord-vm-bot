



## Configuring the app and creating the Azure Service Principal

To programmatically manage the virtual machine, Azure requires you to create a Service Principal. This can be terribly convoluted,
but for this simple purpose, we create it in a simple way.

First, log in using `az`.

```
$ az login
...
$ az account list

{
    ...
    "id": "12345678-1234-1234-1234-111122223333",
    "name": "My subscription"
    ...
  }
```

Note the id of the subscription your virtual machine is in.
This goes to your `.env` file to `AZURE_SUBSCRIPTION_ID`.


Get the id of your virtual machine.

```
$ az vm list

[
  {
    ...
    "id": "/subscriptions/12345678-1234-1234-1234-111122223333/resourceGroups/my-rg/providers/Microsoft.Compute/virtualMachines/my-server-name"
    ...
  }
]
```

Note the id, you'll need it in a moment. Also put it to the `.env` file `AZURE_VIRTUAL_MACHINE_ID`.

Then, create a service principal 

```
$ az ad sp create-for-rbac --name discord-bot

{
    "appId": "my-app-id",
    "password": "some-random-password",
    "tenant": "my-tenant-id"
}
```

Note the return variables. These go to the `.env` file:

- `appId` -> `AZURE_CLIENT_ID`
- `password` -> `AZURE_CLIENT_SECRET`
- `tenant` -> `AZURE_TENANT_ID`

Finally, assign the Service Principal with the appropriate role, targeting the appropriate virtual machine.

```
$ az role assignment create --assignee "appIdHere" --role "Virtual Machine Contributor" --scope "virtualMachineIdHere"
```


You should end up with an `.env` file like

```
AZURE_CLIENT_ID="my-app-id"
AZURE_CLIENT_SECRET="some-random-password"
AZURE_TENANT_ID="my-tenant-id"
AZURE_SUBSCRIPTION_ID="12345678-1234-1234-1234-111122223333"
AZURE_VIRTUAL_MACHINE_ID="/subscriptions/12345678-1234-1234-1234-111122223333/resourceGroups/my-rg/providers/Microsoft.Compute/virtualMachines/my-server-name"
```