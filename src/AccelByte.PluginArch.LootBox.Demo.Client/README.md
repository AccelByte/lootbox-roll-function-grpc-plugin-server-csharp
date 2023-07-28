# Platform Service's Custom Lootbox Roll Plugin gRPC Demo App

A CLI demo app to prepare required data and execute Custom Lootbox Roll Plugin gRPC for AGS's Platform Service.
Following diagram will explain how this CLI demo app works.
```mermaid
sequenceDiagram
    participant A as CLI Demo App
    participant I as AGS IAM
    participant P as AGS Platform
    participant G as Grpc Plugin Server
    
    A ->> I: user login
    I -->> A: auth token
    A ->> P: Set gRpc server target
    A ->> P: Create draft store and publish it
    A ->> P: Create item's category    
    A ->> P: Create lootbox item(s)
    A ->> P: Grant item entitlement to user
    A ->> P: Consume entitlement    
    P ->> G: Call lootbox roll function
    G -->> P: Returns rewards
    P -->> A: Return rewards
    A ->> P: Delete store
    A ->> P: Unset gRpc server target
```

## Prerequsites

* .net 6 sdk

## Usage

### Setup

The following environment variables are used by this CLI demo app.
```
export AB_BASE_URL='https://demo.accelbyte.io'
export AB_CLIENT_ID='xxxxxxxxxx'
export AB_CLIENT_SECRET='xxxxxxxxxx'

export AB_NAMESPACE='namespace'
export AB_USERNAME='USERNAME'
export AB_PASSWORD='PASSWORD'
```
If these variables aren't provided, you'll need to supply the required values via command line arguments.

Also, you will need `Custom Lootbox Roll Plugin gRPC` server already deployed and accessible. If you want to use your local development environment, you can use tunneling service like `ngrok` to tunnel your grpc server port so it can be accessed by AGS.
> Current AGS deployment does not support mTLS and authorization for custom grpc plugin. Make sure you disable mTls and authorization in your deployed Grpc server.


### Example
CLI demo app requires only the grpc server url as a parameter.

- Without any environment variables
```bash
$ dotnet run -- --baseurl https://demo.accelbyte.io --client <CLIENT-ID-VALUE> --secret <CLIENT-SECRET-VALUE> --namespace <NAMESPACE-VALUE> --username <USERNAME> --password <PASSWORD> --grpc-target <GRPC_PLUGIN_SERVER_URL>
```

- With basic environment variables setup
```bash
$ dotnet run -- --username <USERNAME> --password <PASSWORD> --namespace <NAMESPACE-VALUE> --grpc-target <GRPC_PLUGIN_SERVER_URL>
```

- With all environment variables setup
```bash
$ dotnet run -- --grpc-target <GRPC_PLUGIN_SERVER_URL>
```

- With all environment variables setup and using extend app instead of grpc target
```bash
$ dotnet run -- --extend-app <EXTEND_APP_NAME>
```

- With all environment variables setup and use custom store category path (category path must be prefixed by `/`)
```bash
$ dotnet run -- --category <CATEGORY_PATH> --grpc-target <GRPC_PLUGIN_SERVER_URL>
```

- Show usage help
```bash
 $ dotnet run -- --help
```

## Build

To build this CLI sample app, execute the following command.

```bash
$ dotnet build
```