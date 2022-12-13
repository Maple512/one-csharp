#!/bin/bash
git config --global --replace-all core.quotepath false

dotnet test *.sln --no-restore -c Release
