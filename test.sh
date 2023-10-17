#!/usr/bin/env bash

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./coverage.opencover.xml

reportgenerator -reports:tests/coverage.opencover.xml -targetdir:coverage-report