# Client

> This directory contains all the necessary files to run KeiPai frontend

## Project structure

```
├───app
│   ├───components
│   ├───guards
│   ├───models
│   ├───pages
│   ├───pipes
│   ├───services
│   └───validators
└───environments
```

`components` - parts of the pages that are used in multiple pages

`guards` - prevent users from from navigating to parts of an application without authorization

`models` - interfaces for all the objects retrieved from the server

`pages` - all the web pages that user can access

`pipes` - custom template expressions that transform data

`sevices` - used to retrieve and send data to server

`validators` - stores `file-validator` that validates uploaded files on the client part

`environments` - place to store all the environment variables (mostly ClientIDs)