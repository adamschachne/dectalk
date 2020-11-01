# Dectalk

Azure Function hosted http trigger application for generating voice phrases with Dectalk

Once the code is deployed to the function app, go to Settings -> Configuration and set WEBSITE_RUN_FROM_PACKAGE = 0. This will allow the function to write wav files to the local file system.