#!/bin/sh

nswag openapi2csclient /namespace:MainApp /input:http://localhost:5102/swagger/v1/swagger.json /classname:PasswordStoreClient /output:PasswordStoreClient.cs

