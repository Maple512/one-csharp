#!/bin/bash
git config --global core.quotepath false

dotnet test --no-restore -c Release
