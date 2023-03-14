#!/bin/sh

#nswag openapi2csclient /namespace:MainApp /input:http://localhost:5102/swagger/v1/swagger.json /classname:PasswordStoreClient /output:PasswordStoreClient.cs
nswag openapi2csclient /namespace:MainApp /input:https://rest.api.tomwimmenhove.com/password/swagger/v1/swagger.json /classname:PasswordStoreClient /output:PasswordStoreClient.cs

