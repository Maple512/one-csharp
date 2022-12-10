#!/bin/bash
git config --global core.quotepath false

dotnet pack --no-restore -c Release -o ./packages
